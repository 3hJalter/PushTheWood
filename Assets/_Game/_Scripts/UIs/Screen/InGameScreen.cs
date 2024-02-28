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
        
        // UNDO
        [SerializeField] private Button undoButton;
        // RESET ISLAND
        [SerializeField] private Button resetIslandButton;
        // GROW TREE
        [SerializeField] private HButton growTreeButton;
        [SerializeField] private GameObject activeGrowTreeImage;
        [SerializeField] private GameObject growTreeAmountFrame;
        // PUSH HINT
        [SerializeField] private HButton pushHintButton;
        [SerializeField] private GameObject activePushHintImage;
        [SerializeField] private GameObject inActivePushHintImage;
        [SerializeField] private GameObject pushHintAmountFrame;
        [SerializeField] private Transform tryAgainPushHintImage;
        
        // Time & Level Text 
        [SerializeField] private TMP_Text timeText;
        [SerializeField] private TMP_Text _levelText;

        [SerializeField] private TextMeshProUGUI objectiveText;

        public TextMeshProUGUI undoCountText;
        public TextMeshProUGUI resetCountText;
        public TextMeshProUGUI hintCountText;
        public TextMeshProUGUI growTreeCountText;
        public TextMeshProUGUI pushHintCountText;
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
            growTreeButton.interactable = active;
        }

        public  void OnBoughtPushHint(bool active)
        {
            activePushHintImage.SetActive(active);
            pushHintAmountFrame.SetActive(!active);
        }
        
        public void ActivePushHintIsland(bool active)
        {
            inActivePushHintImage.gameObject.SetActive(!active);
            pushHintButton.interactable = active;
        }
        
        public void OnBoughtPushHintOnIsland(int islandID, bool active)
        {
            // Check if the current island is the island that the player is on
            if (LevelManager.Ins.player.islandID != islandID) return;
            activePushHintImage.SetActive(active);
            pushHintAmountFrame.SetActive(!active);
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

        public void OnHideIfTutorial()
        {
            bool isTutorial = LevelManager.Ins.IsTutorialLevel;
            // Hide all booster
            undoButton.gameObject.SetActive(!isTutorial);
            resetIslandButton.gameObject.SetActive(!isTutorial);
            growTreeButton.gameObject.SetActive(!isTutorial);
            pushHintButton.gameObject.SetActive(!isTutorial);
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
            AudioManager.Ins.PlaySfx(SfxType.Undo);
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
        
        public void OnClickPushHint()
        {
            OnUsePushHint?.Invoke();
        }
        
        #endregion

        
        private Tween _tryAgainImageTween;
        public void OnShowTryHintAgain(bool show)
        {
           // Tween local position from 0 160 0 to 0 130 0 in a yo-yo loop
           tryAgainPushHintImage.gameObject.SetActive(show);
           _tryAgainImageTween?.Kill();
           tryAgainPushHintImage.localPosition = new Vector3(0, 160, 0);
           if (show)
           {
               _tryAgainImageTween = tryAgainPushHintImage.DOLocalMoveY(130, 0.5f).SetLoops(-1, LoopType.Yoyo);
           }
        }
    }
}
