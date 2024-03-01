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
        }

        private void OnDestroy()
        {
            LevelManager.Ins.OnLevelNext -= LevelManager_OnLevelNext;
            TimerManager.Ins.PushSTimer(undoTimer);
            TimerManager.Ins.PushSTimer(resetIslandTimer);
            undoButton.RemoveEvent(OnClickUndo);
            pushHintButton.RemoveEvent(OnClickPushHint);
            growTreeButton.RemoveEvent(OnClickGrowTree);
        }

        public event Action OnUndo;
        public event Action OnGrowTree;
        public event Action OnUsePushHint;

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
            SetActiveGrowTree(true);
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

        public void OnHideIfFirstTutorial()
        {
            bool isTutorial = LevelManager.Ins.IsTutorialLevel;
            // Hide all booster
            undoButton.gameObject.SetActive(!isTutorial);
            growTreeButton.gameObject.SetActive(!isTutorial);
            pushHintButton.gameObject.SetActive(!isTutorial);
            // Hide time
            timerContainer.SetActive(!isTutorial);
            // hide setting
            settingButton.gameObject.SetActive(!isTutorial);
        }

        public void OnShowBooster()
        {
            undoButton.SetAmount(DataManager.Ins.GameData.user.undoCount);
            pushHintButton.SetAmount(DataManager.Ins.GameData.user.pushHintCount);
            growTreeButton.SetAmount(DataManager.Ins.GameData.user.growTreeCount);
            // Get the unlock level of the booster
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
            } else
            {
                undoButton.IsLock = false;
                pushHintButton.IsLock = false;
                growTreeButton.IsLock = false;
            }
        }
        
        #region Booster

        public void OnClickUndo()
        {
            // Check number of ticket to use
            OnUndo?.Invoke();
            undoButton.IsInteractable = false;
            pushHintButton.IsFocus = false;
            AudioManager.Ins.PlaySfx(SfxType.Undo);
            undoTimer.Start(UNDO_CD_TIME, () => undoButton.IsInteractable = true);
        }

        public void OnClickGrowTree()
        {
            OnGrowTree?.Invoke();
            pushHintButton.IsFocus = false;
        }
        
        public void OnClickPushHint()
        {
            OnUsePushHint?.Invoke();
        }
        
        #endregion

        
        private Tween _tryAgainImageTween;
        public void OnShowTryHintAgain(bool show)
        {
           pushHintButton.IsFocus = show;
           pushHintButton.HasAlternativeImage = show;
        }
    }
}
