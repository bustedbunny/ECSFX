using Unity.Entities;
using UnityEngine;

namespace ECSFX.Authoring
{
    public class FXEmitterAuthoring : MonoBehaviour
    {
        public FXAuthoring fx;

        public float rateOverTime;

        public class FXEmitterAuthoringBaker : Baker<FXEmitterAuthoring>
        {
            public override void Bake(FXEmitterAuthoring authoring)
            {
                var fxEmitter = new FXEmitter(fx: GetEntity(authoring.fx, TransformUsageFlags.Dynamic),
                    rate: 1f / authoring.rateOverTime);
                AddComponent(GetEntity(TransformUsageFlags.WorldSpace), fxEmitter);
            }
        }
    }
}