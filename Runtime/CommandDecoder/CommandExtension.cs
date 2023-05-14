using System.Threading;
using Unity.Burst;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Mathematics;

namespace ECSFX
{
    public static unsafe class CommandExtension
    {
        public static void Command<T>(this ECSFXSingleton fx, CommandData data, T command)
            where T : unmanaged, IFXCommand
        {
            var type = BurstRuntime.GetHashCode32<T>();
            const int hashSize = sizeof(int);
            var dataSize = sizeof(CommandData);
            var commandSize = sizeof(T);

            var totalSize = hashSize + dataSize + commandSize;

            var idx = Interlocked.Add(ref fx.buf.m_ListData->m_length, totalSize) - totalSize;
            var ptr = fx.buf.m_ListData->Ptr + idx;
            UnsafeUtility.MemCpy(ptr, UnsafeUtility.AddressOf(ref type), hashSize);
            UnsafeUtility.MemCpy(ptr + hashSize, UnsafeUtility.AddressOf(ref data), dataSize);
            UnsafeUtility.MemCpy(ptr + hashSize + dataSize, UnsafeUtility.AddressOf(ref command), commandSize);
        }
    }

    public struct CommandData
    {
        public Entity e;
        public float3 pos;
        public quaternion rot;
    }
}