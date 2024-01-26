using _Game.DesignPattern;
using Sigtrap.Relays;

namespace _Game._Scripts.Managers
{
    public class EventGlobalManager : Singleton<EventGlobalManager>
    {
        // Global events
        public Relay OnStartLoadScene;
        public Relay OnFinishLoadScene;
        public Relay OnUpdateSetting;
        public Relay OnGameInit;
        public Relay OnPurchaseNoAds;
        
        // In-game events
        // Unit Button
        // ReSharper disable once Unity.RedundantHideInInspectorAttribute
        public Relay<bool> OnButtonUnitEnter;

        private void Awake()
        {
            OnStartLoadScene = new Relay();
            OnFinishLoadScene = new Relay();
            OnUpdateSetting = new Relay();
            OnGameInit = new Relay();
            OnPurchaseNoAds = new Relay();
            OnButtonUnitEnter = new Relay<bool>();
        }
    }
}
