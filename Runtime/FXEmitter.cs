using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

namespace ECSFX
{
    public struct FXEmitter : IComponentData
    {
        public Entity fx;

        public float rate;
        public float state;

        public FXEmitter(Entity fx, float rate) : this()
        {
            this.fx = fx;
            this.rate = rate;
        }
    }

    public partial struct FXEmitterSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            new Job
            {
                fx = SystemAPI.GetSingleton<ECSFXSingleton>(),
                delta = SystemAPI.Time.DeltaTime
            }.Schedule();
        }

        [BurstCompile]
        private partial struct Job : IJobEntity
        {
            public ECSFXSingleton fx;
            public float delta;

            private void Execute(ref FXEmitter emitter, in LocalToWorld ltw)
            {
                emitter.state -= delta;
                if (emitter.state <= emitter.rate)
                {
                    emitter.state += emitter.rate;
                    fx.Emit(emitter.fx, 1, ltw.Position, ltw.Rotation);
                }
            }
        }
    }
}