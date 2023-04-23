using Unity.Collections;
using Unity.Entities;

namespace ECSFX
{
    public struct ECSFXSingleton : IComponentData
    {
        [ReadOnly] internal NativeList<byte> buf;
    }
}