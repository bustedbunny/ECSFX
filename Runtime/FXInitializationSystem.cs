using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace ECSFX
{
    public partial class FXInitializationSystem : SystemBase
    {
        private EntityQuery _registerQuery;
        private EntityQuery _unregisterQuery;

        private NativeList<byte> _buf;
        private FXSingleton _map;

        protected override void OnCreate()
        {
            _registerQuery = SystemAPI.QueryBuilder().WithAll<FXTag>().WithNone<FXState>()
                .WithOptions(EntityQueryOptions.IncludePrefab).Build();
            _unregisterQuery = SystemAPI.QueryBuilder().WithNone<FXTag>().WithAll<FXState>().Build();

            RequireAnyForUpdate(_registerQuery);

            _buf = new(1024, Allocator.Persistent);
            var singleton = EntityManager.CreateSingleton<ECSFXSingleton>();
            SystemAPI.SetComponent(singleton, new ECSFXSingleton
            {
                buf = _buf
            });

            var e = EntityManager.CreateSingleton<FXSingleton>();
            _map = new()
            {
                map = new()
            };
            EntityManager.AddComponentObject(e, _map);
        }

        protected override void OnDestroy()
        {
            _buf.Dispose();
        }

        protected override void OnUpdate()
        {
            var list = _registerQuery.ToEntityArray(CheckedStateRef.WorldUpdateAllocator);
            foreach (var e in list)
            {
                var ps = SystemAPI.ManagedAPI.GetComponent<ParticleSystem>(e);
                var emissionModule = ps.emission;
                emissionModule.enabled = false;
                ps.gameObject.SetActive(true);
                EntityManager.AddComponentObject(e, new FXState());
                _map.map[e] = ps;
            }

            EntityManager.RemoveComponent<Prefab>(list);

            list = _unregisterQuery.ToEntityArray(CheckedStateRef.WorldUpdateAllocator);
            foreach (var e in list)
            {
                _map.map.Remove(e);
            }

            EntityManager.RemoveComponent<FXState>(list);
        }
    }

    public class FXState : ICleanupComponentData { }

    public class FXSingleton : IComponentData
    {
        public Dictionary<Entity, ParticleSystem> map;
    }
}