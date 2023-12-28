using System.Collections.Generic;
using _Game._Scripts.InGame;
using _Game._Scripts.UIs.Tutorial;
using _Game.Camera;
using _Game.DesignPattern;
using _Game.GameGrid.Unit.DynamicUnit.Player;
using _Game.Managers;
using _Game.UIs.Screen;
using UnityEngine;

namespace _Game.GameGrid
{
    public class LevelManager : Singleton<LevelManager>
    {
        [SerializeField] private int levelIndex;
        private List<Level> _activeLevels = new (); // All the level that currently show in world map 
        private Level _currentLevel;
        public Level CurrentLevel => _currentLevel;
        
        private int _tutorialIndex;

        public Player player;

        private void Start()
        {
            // TEST
            PlayerPrefs.SetInt(Constants.TUTORIAL_INDEX, 0);
            // PlayerPrefs.SetInt(Constants.LEVEL_INDEX, 0);
            levelIndex = PlayerPrefs.GetInt(Constants.LEVEL_INDEX, 0);
            _tutorialIndex = PlayerPrefs.GetInt(Constants.TUTORIAL_INDEX, 0);
            OnInit();
            // Test -> Load 3 next level
            //for (int i = levelIndex + 1; i < levelIndex + 4; i++)
            //{
            //    Level level = new(i);
            //    _activeLevels.Add(level);
            //}
        }

        public void OnShowTutorial()
        {
            UIManager.Ins.OpenUI<TutorialScreen>()
                .LoadContext(Instantiate(DataManager.Ins.GetTutorial(_tutorialIndex)));
            _tutorialIndex++;
            if (_tutorialIndex >= DataManager.Ins.CountTutorial) _tutorialIndex = 0;
        }

        public void OnInit()
        {
            _currentLevel = new Level(levelIndex);
            _currentLevel.OnInitLevel();
            // SetCameraToPlayer();
            SetCameraToPlayerIsland();
            // CameraManager.Ins.ChangeCameraTargetPosition(GetCenterPos());
        }

        public void SetCameraToPlayerIsland()
        {
            CameraManager.Ins.ChangeCameraTargetPosition(CurrentLevel.GetIsland(player.islandID).GetCenterIslandPos());
        }
    
        public void OnWin()
        {
            // Show win screen
            UIManager.Ins.OpenUI<WinScreen>();
            // +1 LevelIndex and save
            levelIndex++;
            // Temporary handle when out of level
            if (levelIndex >= DataManager.Ins.CountLevel) levelIndex = 0;
            PlayerPrefs.SetInt(Constants.LEVEL_INDEX, levelIndex);
            // Future: Add reward collected in-game
        }

        public void OnNextLevel()
        {
            // Load next level
            _currentLevel.OnDeSpawnLevel();
            OnInit();      
                
            OnChangeTutorialIndex();
        }

        private void OnChangeTutorialIndex()
        {
            PlayerPrefs.SetInt(Constants.TUTORIAL_INDEX, _tutorialIndex);
        }

        public void OnLose()
        {
            // Show lose screen
        }

        public void OnRestart()
        {
            CurrentLevel.ResetAllIsland();
            player.OnDespawn();
            player = SimplePool.Spawn<Player>(DataManager.Ins.GetGridUnit(PoolType.Player));
            player.OnInit(CurrentLevel.firstPlayerInitCell);

            // FxManager.Ins.ResetTrackedTrampleObjectList();
        }
        
        private void SetCameraToPlayer()
        {
            // CameraFollow.Ins.SetTarget(Player.Tf);`
            CameraManager.Ins.ChangeCameraTarget(ECameraType.InGameCamera, player.Tf);
        }
    }

    
}
