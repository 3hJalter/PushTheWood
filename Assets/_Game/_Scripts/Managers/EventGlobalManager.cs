using _Game._Scripts.InGame;
using _Game.DesignPattern;
using _Game.Managers;
using _Game.Resource;
using Sigtrap.Relays;
using UnityEngine;

namespace _Game._Scripts.Managers
{
    [DefaultExecutionOrder(-1)]
    public class EventGlobalManager : Singleton<EventGlobalManager>
    {
        // Global events
        // public Relay OnStartLoadScene;
        // public Relay OnFinishLoadScene;
        // public Relay OnUpdateSetting;
        // public Relay OnGameInit;
        // public Relay OnPurchaseNoAds;
        
        // In-game events

        public Relay OnChangeLevelCollectingObjectNumber { get; private set; } // For calculate how many objects need to collect to win the game
        public Relay<bool> OnButtonUnitEnter { get; private set; } // For Unit Button
        public Relay OnEnemyDie { get; private set; } // For Enemy
        public Relay<BoosterType, int> OnChangeBoosterAmount { get; private set; }
        public Relay OnGrowTree { get; private set; } // For Tree Seed
        public Relay OnPlayerChangeIsland { get; private set; } // For When Player go to the other Island
        public Relay<PlayerStep>  OnPlayerPushStep { get; private set; } // For Push Hint
        
        private void Awake()
        {
            // OnStartLoadScene = new Relay();
            // OnFinishLoadScene = new Relay();
            // OnUpdateSetting = new Relay();
            // OnGameInit = new Relay();
            // OnPurchaseNoAds = new Relay();
            OnChangeLevelCollectingObjectNumber = new Relay();
            OnButtonUnitEnter = new Relay<bool>();
            OnEnemyDie = new Relay();
            OnChangeBoosterAmount = new Relay<BoosterType, int>();
            OnGrowTree = new Relay();
            OnPlayerChangeIsland = new Relay();
            OnPlayerPushStep = new Relay<PlayerStep>();
            DontDestroyOnLoad(gameObject);
        }
    }
}
