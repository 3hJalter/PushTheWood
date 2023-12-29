using System.Collections.Generic;
using System.Linq;
using _Game._Scripts.InGame;
using _Game._Scripts.UIs.Tutorial;
using _Game.Camera;
using _Game.DesignPattern;
using _Game.GameGrid.Unit;
using _Game.GameGrid.Unit.DynamicUnit.Player;
using _Game.Managers;
using _Game.UIs.Screen;
using _Game.Utilities;
using _Game.Utilities.Grid;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UIElements;

namespace _Game.GameGrid
{
    public class LevelManager : Singleton<LevelManager>
    {
        [SerializeField] private int levelIndex;
        private Level _currentLevel;
        [SerializeField]
        private Material OverlayMaterial;
        public Level CurrentLevel => _currentLevel;

        private int _tutorialIndex;
        private CareTaker savingState;
        [HideInInspector]
        public Player player;

        private void Start()
        {
            // TEST
            PlayerPrefs.SetInt(Constants.TUTORIAL_INDEX, 0);
            // PlayerPrefs.SetInt(Constants.LEVEL_INDEX, 0);
            levelIndex = PlayerPrefs.GetInt(Constants.LEVEL_INDEX, 0);
            _tutorialIndex = PlayerPrefs.GetInt(Constants.TUTORIAL_INDEX, 0);
            GridUtilities.OverlayMaterial = OverlayMaterial;
            OnInit();
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
            SetCameraToPlayer();
            savingState = new CareTaker(this);
            savingState.SavingState();
            // SetCameraToPlayerIsland();
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
            savingState = new CareTaker(this);
            // FxManager.Ins.ResetTrackedTrampleObjectList();
        }
        public void OnUndo()
        {
            savingState.Undo();
        }

        private void SetCameraToPlayer()
        {
            // CameraFollow.Ins.SetTarget(Player.Tf);`
            CameraManager.Ins.ChangeCameraTarget(ECameraType.InGameCamera, player.Tf);
        }

        public class CareTaker
        {
            LevelManager main;
            Stack<List<IMemento>> historys = new Stack<List<IMemento>>();
            public CareTaker(LevelManager main)
            {
                this.main = main;
                main.player.OnSavingState += SavingState;
            }
            public void Undo()
            {
                if (historys.Count > 0)
                {
                    List<IMemento> states = historys.Pop();
                    foreach (IMemento state in states)
                    {
                        state.Restore();
                    }
                    DevLog.Log(DevId.Hung, "UNDO_STATE - SUCCESS!!");
                }
                else
                {
                    DevLog.Log(DevId.Hung, "UNDO_STATE - FAILURE!!");
                }
            }
            public void SavingState()
            {
                HashSet<GridUnit> gridUnits = main._currentLevel.Islands[main.player.islandID].GridUnits;
                List<IMemento> states = new List<IMemento>() { main._currentLevel.GridMap.Save(), main.player.Save() };
                foreach (GridUnit gridUnit in gridUnits)
                {
                    states.Add(gridUnit.Save());
                }
                historys.Push(states);
            }
        }

    }

}
