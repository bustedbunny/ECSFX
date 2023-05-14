using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace ECSFX
{
    internal class PlayDecoder : Decoder<PlayData>
    {
        protected override void Decode(CommandData data, PlayData command, FXSingleton fx)
        {
            var state = fx.map[data.e];
            state.main.transform.SetPositionAndRotation(data.pos, data.rot);
            EntityManager.SetComponentData(data.e, LocalTransform.FromPositionRotation(data.pos, data.rot));
            state.main.Play(command.includeChildren);
        }

        public PlayDecoder(EntityManager entityManager) : base(entityManager) { }
    }

    public struct PlayData : IFXCommand
    {
        public bool includeChildren;
    }
}