using System;
using _Game._Scripts.InGame;
using _Game._Scripts.Managers;
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
using UnityEngine.Serialization;
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

        [SerializeField] private Button undoButton;

        [SerializeField] private Button resetIslandButton;
        
        [SerializeField] private HButton growTreeButton;
        [SerializeField] private GameObject activeGrowTreeImage;
        [SerializeField] private GameObject growTreeAmountFrame;
        
        [SerializeField] private TMP_Text timeText;

        [SerializeField] private TMP_Text _levelText;

        [SerializeField] private TextMeshProUGUI objectiveText;

        public TextMeshProUGUI undoCountText;
        public TextMeshProUGUI resetCountText;
        public TextMeshProUGUI hintCountText;
        public TextMeshProUGUI growTreeCountText;
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

            undoTimer = TimerManager.Ins.PopSTimer();
            resetIslandTimer = TimerManager.Ins.PopSTimer();
        }

        private void OnDestroy()
        {
            LevelManager.Ins.OnLevelNext -= LevelManager_OnLevelNext;
            TimerManager.Ins.PushSTimer(undoTimer);
            TimerManager.Ins.PushSTimer(resetIslandTimer);
        }

        public event Action OnUndo;
        public event Action OnResetIsland;
        public event Action OnHint;
        public event Action OnCancelHint;
        public event Action OnGrowTree;

        public override void Setup(object param = null)
        {
            base.Setup(param);
            GameManager.Ins.ChangeState(GameState.InGame);
            // Log param
            if (param is null) MoveInputManager.Ins.ShowContainer(true);
            else MoveInputManager.Ins.ShowContainer(true, (bool)param);
            OnHideIfTutorial();
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
            undoButton.interactable = active;
        }

        public void SetActiveResetIsland(bool active)
        {
            resetIslandTimer.Stop();
            resetIslandButton.interactable = active;
        }
        
        public void OnBoughtGrowTree(bool active)
        {
            DevLog.Log(DevId.Hoang, "On Bought Grow Tree: " + active);
            activeGrowTreeImage.SetActive(active);
            growTreeAmountFrame.SetActive(!active);
        }
        
        public void SetActiveGrowTree(bool active)
        {
            DevLog.Log(DevId.Hoang, "Set Active Grow Tree: " + active);
            growTreeButton.interactable = active;
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
            objectiveText.text = LevelManager.Ins.CurrentLevel.LevelWinCondition switch
            {
                LevelWinCondition.FindingChest => Constants.FIND_CHEST,
                LevelWinCondition.DefeatAllEnemy => Constants.DEFEAT_ENEMY,
                LevelWinCondition.CollectAllStar => Constants.COLLECT_ALL_STARS,
                _ => objectiveText.text
            };
        }

        private void LevelManager_OnLevelNext()
        {
            UpdateLevelText();
            UpdateObjectiveText();
        }

        private void OnHideIfTutorial()
        {
            bool isTutorial = LevelManager.Ins.IsTutorialLevel;
            // Hide all booster
            undoButton.gameObject.SetActive(!isTutorial);
            resetIslandButton.gameObject.SetActive(!isTutorial);
            growTreeButton.gameObject.SetActive(!isTutorial);
            // Hide time
            timerContainer.SetActive(!isTutorial);
            // hide setting
            settingButton.gameObject.SetActive(!isTutorial);
            
        }
        
        #region Booster

        public void OnClickUndo()
        {
            // Check number of ticket to use
            OnUndo?.Invoke();
            undoButton.interactable = false;
            undoTimer.Start(UNDO_CD_TIME, () => undoButton.interactable = true);
        }

        public void OnClickResetIslandButton()
        {
            OnResetIsland?.Invoke();
            resetIslandButton.interactable = false;
            resetIslandTimer.Start(UNDO_CD_TIME, () => resetIslandButton.interactable = true);
        }

        public void OnShowHint()
        {
            OnHint?.Invoke();
        }

        public void OnHideHint()
        {
            OnCancelHint?.Invoke();
        }

        public void OnClickGrowTree()
        {
            OnGrowTree?.Invoke();
        }
        
        #endregion
    }
}
