using System.Collections.Generic;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;

namespace ECSFX
{
    public class CommandDecoder
    {
        private readonly Dictionary<int, IDecoder> _decoders = new();

        public CommandDecoder(EntityManager em)
        {
            var emit = new EmitDecoder(em);
            _decoders.Add(emit.TypeHash, emit);
            var play = new PlayDecoder(em);
            _decoders.Add(play.TypeHash, play);
        }

        public unsafe void Decode(NativeList<byte> buf, FXSingleton fx)
        {
            var ptr = buf.GetUnsafeReadOnlyPtr();
            var iterator = 0;
            while (iterator < buf.Length)
            {
                var offset = ptr + iterator;
                var hash = UnsafeUtility.AsRef<int>(offset);
                var decoder = _decoders[hash];
                offset += sizeof(int);
                var data = UnsafeUtility.AsRef<CommandData>(offset);
                offset += sizeof(CommandData);
                decoder.Decode(data, offset, fx);
                iterator += sizeof(int) + sizeof(CommandData) + decoder.DataSize;
            }

            buf.Clear();
        }
    }
}