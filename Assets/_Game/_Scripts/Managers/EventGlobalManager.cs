using _Game.DesignPattern;
using _Game.Managers;
using Sigtrap.Relays;

namespace _Game._Scripts.Managers
{
    public class EventGlobalManager : Singleton<EventGlobalManager>
    {
        // Global events
        // public Relay OnStartLoadScene;
        // public Relay OnFinishLoadScene;
        // public Relay OnUpdateSetting;
        // public Relay OnGameInit;
        // public Relay OnPurchaseNoAds;
        
        // In-game events

        public Relay<bool> OnButtonUnitEnter { get; private set; } // For Unit Button
        public Relay OnEnemyDie { get; private set; } // For Enemy
        public Relay<BoosterType, int> OnChangeBoosterAmount { get; private set; }
        
        public Relay OnGrowTree { get; private set; } // For Tree Seed
        
        private void Awake()
        {
            // OnStartLoadScene = new Relay();
            // OnFinishLoadScene = new Relay();
            // OnUpdateSetting = new Relay();
            // OnGameInit = new Relay();
            // OnPurchaseNoAds = new Relay();
            OnButtonUnitEnter = new Relay<bool>();
            OnEnemyDie = new Relay();
            OnChangeBoosterAmount = new Relay<BoosterType, int>();
            OnGrowTree = new Relay();
        }
    }
}
