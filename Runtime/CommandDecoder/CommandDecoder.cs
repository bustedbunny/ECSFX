using System.Collections.Generic;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace ECSFX
{
    public class CommandDecoder
    {
        private readonly Dictionary<Command, IDecoder> _decoders = new();

        public CommandDecoder()
        {
            _decoders.Add(Command.Emit, new EmitDecoder());
        }

        public unsafe void Decode(NativeList<byte> buf, FXSingleton fx)
        {
            var ptr = buf.GetUnsafeReadOnlyPtr();
            var iterator = 0;
            while (iterator < buf.Length)
            {
                var command = UnsafeUtility.AsRef<Command>(ptr + iterator);
                var decoder = _decoders[command];
                decoder.Decode(ptr + iterator + sizeof(Command), fx);
                iterator += sizeof(Command) + decoder.DataSize;
            }

            buf.Clear();
        }
    }
}