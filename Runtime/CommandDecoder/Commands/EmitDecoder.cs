using Unity.Entities;

namespace ECSFX
{
    internal class EmitDecoder : Decoder<Emit>
    {
        protected override void Decode(CommandData data, Emit command, FXSingleton fx)
        {
            var state = fx.map[data.e];
            state.main.transform.SetPositionAndRotation(data.pos, data.rot);

            var count = state.main.particleCount;
            state.main.Emit(command.count);
        }

        public EmitDecoder(EntityManager entityManager) : base(entityManager) { }
    }

    public struct Emit : IFXCommand
    {
        public int count;
    }
}