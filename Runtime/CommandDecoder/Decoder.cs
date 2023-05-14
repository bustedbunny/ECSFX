using Unity.Burst;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;

namespace ECSFX
{
    public abstract unsafe class Decoder<T> : IDecoder where T : unmanaged, IFXCommand
    {
        protected Decoder(EntityManager entityManager)
        {
            EntityManager = entityManager;
            TypeHash = BurstRuntime.GetHashCode32<T>();
        }

        protected EntityManager EntityManager { get; }

        protected abstract void Decode(CommandData data, T command, FXSingleton fx);

        public int TypeHash { get; }

        public int DataSize => sizeof(T);

        public void Decode(CommandData data, byte* ptr, FXSingleton fx) =>
            Decode(data, UnsafeUtility.AsRef<T>(ptr), fx);
    }

    public unsafe interface IDecoder
    {
        public void Decode(CommandData data, byte* ptr, FXSingleton fx);
        public int DataSize { get; }
    }
}