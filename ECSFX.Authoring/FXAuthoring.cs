using Unity.Entities;
using UnityEngine;

namespace ECSFX.Authoring
{
    public class FXAuthoring : MonoBehaviour
    {
        private class Baker : Baker<FXAuthoring>
        {
            public override void Bake(FXAuthoring authoring)
            {
                AddComponent<FXTag>(GetEntity(TransformUsageFlags.Dynamic));
            }
        }
    }
}