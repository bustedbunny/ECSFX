using Unity.Entities;

namespace ECSFX
{
    [UpdateInGroup(typeof(PresentationSystemGroup))]
    [CreateAfter(typeof(FXInitializationSystem))]
    public partial class FxCommandSystem : SystemBase
    {
        private FXSingleton _fx;
        private CommandDecoder _decoder;

        protected override void OnCreate()
        {
            _fx = SystemAPI.ManagedAPI.GetSingleton<FXSingleton>();
            _decoder = new(EntityManager);
        }

        protected override void OnUpdate()
        {
            EntityManager.CompleteDependencyBeforeRW<ECSFXSingleton>();
            var data = SystemAPI.GetSingleton<ECSFXSingleton>();
            _decoder.Decode(data.buf, _fx);
        }
    }
}