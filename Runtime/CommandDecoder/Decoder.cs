using System.Text;
using Unity.Collections.LowLevel.Unsafe;

namespace ECSFX
{
    public abstract unsafe class Decoder<T> : IDecoder where T : unmanaged, IFXCommand
    {
        public abstract Command Type { get; }

        protected abstract void Decode(ref T data, FXSingleton fx);


        public int DataSize => sizeof(T);

        public void Decode(byte* ptr, FXSingleton fx) => Decode(ref UnsafeUtility.AsRef<T>(ptr), fx);
    }

    public unsafe interface IDecoder
    {
        public void Decode(byte* ptr, FXSingleton fx);
        public int DataSize { get; }
    }
}