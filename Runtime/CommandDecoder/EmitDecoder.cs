using System.Threading;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Random = UnityEngine.Random;

namespace ECSFX
{
    internal class EmitDecoder : Decoder<EmitData>
    {
        public override Command Type => Command.Emit;

        protected override void Decode(ref EmitData data, FXSingleton fx)
        {
            var particleSystem = fx.map[data.e];
            particleSystem.transform.SetPositionAndRotation(data.pos, data.rot);
            particleSystem.Emit(data.count);
        }
    }

    public static unsafe class EmitExtension
    {
        public static void Emit(this ref ECSFXSingleton fx, Entity e, int count, float3 pos, quaternion rot)
        {
            var type = Command.Emit;
            var totalSize = sizeof(Command) + sizeof(EmitData);
            var data = new EmitData
            {
                e = e,
                count = count,
                pos = pos,
                rot = rot
            };

            var idx = Interlocked.Add(ref fx.buf.m_ListData->m_length, totalSize) - totalSize;
            var ptr = fx.buf.m_ListData->Ptr + idx;
            UnsafeUtility.MemCpy(ptr, UnsafeUtility.AddressOf(ref type), sizeof(Command));
            UnsafeUtility.MemCpy(ptr + sizeof(Command), UnsafeUtility.AddressOf(ref data), sizeof(EmitData));
        }
    }
}