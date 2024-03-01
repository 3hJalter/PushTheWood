using System;
using _Game._Scripts.InGame;
using _Game._Scripts.Managers;
using _Game._Scripts.UIs.Component;
using _Game.Camera;
using _Game.Data;
using _Game.GameGrid;
using _Game.Managers;
using _Game.UIs.Popup;
using _Game.Utilities;
using _Game.Utilities.Timer;
using AudioEnum;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VinhLB;

namespace _Game.UIs.Screen
{
    public class InGameScreen : UICanvas
    {
        private const float UNDO_CD_TIME = 0.3f;

        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private HButton settingButton;
        [SerializeField] private GameObject timerContainer;

        [SerializeField] private Image blockPanel;
     
        // Booster Button
        public BoosterButton undoButton;
        public BoosterButton pushHintButton;
        public BoosterButton growTreeButton;
        public BoosterButton resetIslandButton;
        // Time & Level Text 
        [SerializeField] private TMP_Text timeText;
        [SerializeField] private TMP_Text _levelText;

        [SerializeField] private TextMeshProUGUI objectiveText;
        
        private STimer resetIslandTimer;

        private int time;

        private STimer undoTimer;

        public int Time
        {
            get => time;
            set
            {
                time = value;
                int second = time % 60;
                int minute = time / 60;
                timeText.text = $"{minute:00}:{second:00}";
            }
        }

        private void Awake()
        {
            LevelManager.Ins.OnLevelNext += LevelManager_OnLevelNext;
            LevelManager.Ins.OnObjectiveChange += UpdateObjectiveText;
            undoTimer = TimerManager.Ins.PopSTimer();
            resetIslandTimer = TimerManager.Ins.PopSTimer();
            undoButton.AddEvent(OnClickUndo);
            pushHintButton.AddEvent(OnClickPushHint);
            growTreeButton.AddEvent(OnClickGrowTree);
            resetIslandButton.AddEvent(OnClickResetIsland);
        }

        private void OnDestroy()
        {
            LevelManager.Ins.OnLevelNext -= LevelManager_OnLevelNext;
            TimerManager.Ins.PushSTimer(undoTimer);
            TimerManager.Ins.PushSTimer(resetIslandTimer);
            undoButton.RemoveEvent(OnClickUndo);
            pushHintButton.RemoveEvent(OnClickPushHint);
            growTreeButton.RemoveEvent(OnClickGrowTree);
            resetIslandButton.RemoveEvent(OnClickResetIsland);
        }

        public event Action OnUndo;
        public event Action OnGrowTree;
        public event Action OnUsePushHint;
        public event Action OnResetIsland;

        public override void Setup(object param = null)
        {
            base.Setup(param);
            GameManager.Ins.ChangeState(GameState.InGame);
            OnShowTryHintAgain(false);
            // Log param
            if (param is null) MoveInputManager.Ins.ShowContainer(true);
            else MoveInputManager.Ins.ShowContainer(true, (bool)param);
            if (CameraManager.Ins.IsCurrentCameraIs(ECameraType.InGameCamera)) return;
            CameraManager.Ins.ChangeCamera(ECameraType.InGameCamera);
            canvasGroup.alpha = 0f;
            blockPanel.enabled = true;
            UpdateLevelText();
            UpdateObjectiveText();
            
        }

        public override void Open(object param = null)
        {
            base.Open(param);
            AudioManager.Ins.PlayBgm(BgmType.InGame, 1f);
            AudioManager.Ins.PlayEnvironment(EnvironmentType.Ocean, 1f);
            DebugManager.Ins?.OpenDebugCanvas(UI_POSITION.IN_GAME);
            DOVirtual.Float(0f, 1f, 1f, value => canvasGroup.alpha = value)
                .OnComplete(() => { blockPanel.enabled = false; });
        }

        public override void Close()
        {
            OnShowTryHintAgain(false);
            MoveInputManager.Ins.ShowContainer(false);
            base.Close();
        }

        public void OnClickSettingButton()
        {
            UIManager.Ins.OpenUI<SettingsPopup>();
        }

        public void OnClickToggleBuildingMode()
        {
            Close();
            UIManager.Ins.OpenUI<BuildingScreen>();
            GridBuildingManager.Ins.ToggleBuildMode();
        }

        public void SetActiveUndo(bool active)
        {
            undoTimer.Stop();
            undoButton.IsInteractable = active;
        }
        
        public void SetActiveResetIsland(bool active)
        {
            resetIslandTimer.Stop();
            resetIslandButton.IsInteractable = active;
        }
        
        public void OnBoughtGrowTree(bool active)
        {
            DevLog.Log(DevId.Hoang, "On Bought Grow Tree: " + active);
            growTreeButton.IsShowAmount = !active;
        }
        
        public void SetActiveGrowTree(bool active)
        {
            growTreeButton.IsInteractable = active;
        }
        
        public void ActivePushHintIsland(bool active)
        {
            pushHintButton.IsInteractable = active;
        }
        
        public void OnBoughtPushHintOnIsland(int islandID, bool active, bool isInit)
        {
            // Check if the current island is the island that the player is on
            if (LevelManager.Ins.player.islandID != islandID) return;
            if (active) pushHintButton.IsShowAds = false;
            if (isInit)
            {
                pushHintButton.HasAlternativeImage = false;
                pushHintButton.IsFocus = false;
            }
            pushHintButton.SetAmount(DataManager.Ins.GameData.user.pushHintCount);
        }
        
        private void UpdateLevelText()
        {
            timerContainer.SetActive(true);
            int levelIndex = 1;
            switch (LevelManager.Ins.CurrentLevel.LevelType)
            {
                case LevelType.Normal:
                    levelIndex += LevelManager.Ins.NormalLevelIndex;
                    _levelText.text = $"Level {levelIndex}";
                    break;
                case LevelType.Secret:
                    levelIndex += LevelManager.Ins.SecretLevelIndex;
                    _levelText.text = $"Level {levelIndex}";
                    break;
                case LevelType.DailyChallenge:
                    if (LevelManager.Ins.DailyLevelIndex == 0)
                    {
                        _levelText.text = "Tutorial";
                        break;
                    }
                    levelIndex += LevelManager.Ins.DailyLevelIndex - 1;
                    _levelText.text = $"Day {levelIndex}";
                    break;
            }
        }

        private void UpdateObjectiveText()
        {
            string constant = LevelManager.Ins.CurrentLevel.LevelWinCondition switch
            {
                LevelWinCondition.FindingFruit => Constants.FIND_FRUIT,
                LevelWinCondition.DefeatAllEnemy => Constants.DEFEAT_ENEMY,
                LevelWinCondition.CollectAllChest => Constants.COLLECT_ALL_CHEST,
                LevelWinCondition.FindingChest => Constants.FIND_CHEST,
                LevelWinCondition.FindingChickenBbq => Constants.FIND_CHICKEN_BBQ,
                _ => objectiveText.text
            };
            DevLog.Log(DevId.Hoang, "Update Objective Counter: " + LevelManager.Ins.ObjectiveCounterLeft() + "/" + LevelManager.Ins.objectiveCounter);
            objectiveText.text = $"{constant}: {LevelManager.Ins.ObjectiveCounterLeft()}/{LevelManager.Ins.objectiveCounter}";
        }

        private void LevelManager_OnLevelNext()
        {
            UpdateLevelText();
            UpdateObjectiveText();
        }
        
        public void OnCheckBoosterLock()
        {
            LevelType type = LevelManager.Ins.CurrentLevel.LevelType;
            if (type is LevelType.Normal)
            {
                int currentLevel = LevelManager.Ins.CurrentLevel.Index;
                bool isLock = currentLevel < DataManager.Ins.ConfigData.boosterConfigList[(int)undoButton.Type].UnlockAtLevel;
                undoButton.IsLock = isLock;
                isLock = currentLevel < DataManager.Ins.ConfigData.boosterConfigList[(int)pushHintButton.Type].UnlockAtLevel;
                pushHintButton.IsLock = isLock;
                isLock = currentLevel < DataManager.Ins.ConfigData.boosterConfigList[(int)growTreeButton.Type].UnlockAtLevel;
                growTreeButton.IsLock = isLock;
                isLock = currentLevel < DataManager.Ins.ConfigData.boosterConfigList[(int)resetIslandButton.Type].UnlockAtLevel;
                resetIslandButton.IsLock = isLock;
            } else
            {
                undoButton.IsLock = false;
                pushHintButton.IsLock = false;
                growTreeButton.IsLock = false;
                resetIslandButton.IsLock = false;
            }
        }
        
        public void OnSetBoosterAmount()
        {
            undoButton.SetAmount(DataManager.Ins.GameData.user.undoCount);
            pushHintButton.SetAmount(DataManager.Ins.GameData.user.pushHintCount);
            growTreeButton.SetAmount(DataManager.Ins.GameData.user.growTreeCount);
            resetIslandButton.SetAmount(DataManager.Ins.GameData.user.resetIslandCount);
            // Get the unlock level of the booster
            
        }
        
        #region Booster

        private void OnClickUndo()
        {
            // Check number of ticket to use
            OnUndo?.Invoke();
            undoButton.IsInteractable = false;
            AudioManager.Ins.PlaySfx(SfxType.Undo);
            undoTimer.Start(UNDO_CD_TIME, () => undoButton.IsInteractable = true);
            if (undoButton.IsFocus) undoButton.IsFocus = false;
            if (resetIslandButton.IsFocus) resetIslandButton.IsFocus = false;
        }

        private void OnClickGrowTree()
        {
            OnGrowTree?.Invoke();
            if (undoButton.IsFocus) undoButton.IsFocus = false;
            if (resetIslandButton.IsFocus) resetIslandButton.IsFocus = false;
        }

        private void OnClickResetIsland()
        {
            OnResetIsland?.Invoke();
            if (undoButton.IsFocus) undoButton.IsFocus = false;
            if (resetIslandButton.IsFocus) resetIslandButton.IsFocus = false;
        }
        
        private void OnClickPushHint()
        {
            OnUsePushHint?.Invoke();
            if (undoButton.IsFocus) undoButton.IsFocus = false;
            if (resetIslandButton.IsFocus) resetIslandButton.IsFocus = false;
        }
        
        #endregion

        
        private Tween _tryAgainImageTween;
        public void OnShowTryHintAgain(bool show)
        { 
            undoButton.IsFocus = show;
           resetIslandButton.IsFocus = show;
        }
        
        public void OnHandleTutorial()
        {
            bool isTutorial = LevelManager.Ins.IsFirstTutorialLevel;
            int currentLevel = LevelManager.Ins.CurrentLevel.Index;
            LevelType type = LevelManager.Ins.CurrentLevel.LevelType;
            // Handle Showing time & Setting button
            timerContainer.SetActive(!isTutorial);
            settingButton.gameObject.SetActive(!isTutorial);
            unlimitedUndoButton.gameObject.SetActive(false);
            unlimitedResetIslandButton.gameObject.SetActive(false);
            // Hide Showing Booster
            if (isTutorial && type is LevelType.Normal) {
                undoButton.gameObject.SetActive(false);
                growTreeButton.gameObject.SetActive(false);
                pushHintButton.gameObject.SetActive(false);
                resetIslandButton.gameObject.SetActive(false);
            }
            else
            {
                undoButton.gameObject.SetActive(true);
                growTreeButton.gameObject.SetActive(true);
                pushHintButton.gameObject.SetActive(true);
                resetIslandButton.gameObject.SetActive(true);
            }
            // Other handle for specific level
            unlimitedUndoButton.gameObject.SetActive(false);
            unlimitedResetIslandButton.gameObject.SetActive(false);
            #region Level 3 (index 2)

            if (currentLevel == 2 && type is LevelType.Normal)
            {
                undoButton.gameObject.SetActive(false);
                resetIslandButton.gameObject.SetActive(false);
                unlimitedUndoButton.gameObject.SetActive(true);
                unlimitedResetIslandButton.gameObject.SetActive(true);
                return;
            }
            
            
            #endregion
        }

        #region Unlimited Booster

        public void OnClickUnlimitedUndo()
        {
            GameplayManager.Ins.OnFreeUndo();
        }
        
        public void OnClickUnlimitedResetIsland()
        {
            GameplayManager.Ins.OnFreeResetIsland();
        }
        
        public HButton unlimitedUndoButton;
        public HButton unlimitedResetIslandButton;

        #endregion
        
        
    }
}
