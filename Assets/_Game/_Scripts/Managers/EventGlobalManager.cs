using _Game._Scripts.InGame;
using _Game.DesignPattern;
using _Game.Managers;
using _Game.Resource;
using Sigtrap.Relays;
using UnityEngine;

namespace _Game._Scripts.Managers
{
    [DefaultExecutionOrder(-100)]
    public class EventGlobalManager : Singleton<EventGlobalManager>
    {
        private Relay _onChangeLevelCollectingObjectNumber; // For calculate how many objects need to collect to win the game
        private Relay<bool> _onButtonUnitEnter; // For Unit Button
        private Relay _onEnemyDie; // For Enemy
        private Relay<BoosterType, int> _onChangeBoosterAmount;
        private Relay _onGrowTree; // For Tree Seed
        private Relay<bool> _onPlayerChangeIsland; // For When Player go to the other Island
        private Relay<PlayerStep> _onPlayerPushStep; // For Push Hint
        private Relay<int> _onGrowTreeOnIsland; // For Grow Tree on Island
        
        public Relay OnChangeLevelCollectingObjectNumber => _onChangeLevelCollectingObjectNumber ??= new Relay();
        public Relay<bool> OnButtonUnitEnter => _onButtonUnitEnter ??= new Relay<bool>();
        public Relay OnEnemyDie => _onEnemyDie ??= new Relay();
        public Relay<BoosterType, int> OnChangeBoosterAmount => _onChangeBoosterAmount ??= new Relay<BoosterType, int>();
        public Relay<bool> OnPlayerChangeIsland => _onPlayerChangeIsland ??= new Relay<bool>();
        public Relay OnGrowTree => _onGrowTree ??= new Relay();
        
        public Relay<int> OnGrowTreeOnIsland => _onGrowTreeOnIsland ??= new Relay<int>();
        
        public Relay<PlayerStep> OnPlayerPushStep => _onPlayerPushStep ??= new Relay<PlayerStep>();
        
        private void Awake()
        {
            _onChangeLevelCollectingObjectNumber = new Relay();
            _onButtonUnitEnter = new Relay<bool>();
            _onEnemyDie = new Relay();
            _onChangeBoosterAmount = new Relay<BoosterType, int>();
            _onGrowTree = new Relay();
            _onPlayerChangeIsland = new Relay<bool>();
            _onPlayerPushStep = new Relay<PlayerStep>();
            _onGrowTreeOnIsland = new Relay<int>();
            DontDestroyOnLoad(gameObject);
        }
    }
}
