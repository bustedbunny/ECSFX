using Unity.Entities;
using Unity.Mathematics;

namespace ECSFX
{
    internal struct EmitData : IFXCommand
    {
        public Entity e;
        public int count;
        public float3 pos;
        public quaternion rot;
    }

    public interface IFXCommand { }
}