using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
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
            if (!_registerQuery.IsEmpty)
            {
                var list = _registerQuery.ToEntityArray(CheckedStateRef.WorldUpdateAllocator);
                foreach (var e in list)
                {
                    var ps = SystemAPI.ManagedAPI.GetComponent<ParticleSystem>(e);
                    ps.gameObject.SetActive(true);

                    var all = ps.GetComponentsInChildren<ParticleSystem>();
                    var totalCount = 0;
                    foreach (var particleSystem in all)
                    {
                        totalCount += particleSystem.main.maxParticles;
                    }

                    var state = new FXState
                    {
                        main = ps,
                        all = all,
                        particles = new(totalCount, Allocator.Persistent)
                    };
                    EntityManager.AddComponentObject(e, state);
                    _map.map[e] = state;
                }

                EntityManager.RemoveComponent<Prefab>(list);
            }

            if (!_unregisterQuery.IsEmpty)
            {
                var list = _unregisterQuery.ToEntityArray(CheckedStateRef.WorldUpdateAllocator);
                foreach (var e in list)
                {
                    _map.map.Remove(e);
                }

                EntityManager.RemoveComponent<FXState>(list);
                Debug.Log("Cleaned");
            }
        }
    }

    public class FXState : ICleanupComponentData, IDisposable
    {
        public ParticleSystem main;
        public ParticleSystem[] all;
        public NativeArray<ParticleSystem.Particle> particles;
        public void Dispose() => particles.Dispose();
    }

    public class FXSingleton : IComponentData
    {
        public Dictionary<Entity, FXState> map;
    }
}