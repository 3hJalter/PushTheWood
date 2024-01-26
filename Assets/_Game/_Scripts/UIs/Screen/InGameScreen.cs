using System;
using _Game._Scripts.Managers;
using _Game.Camera;
using _Game.Managers;
using _Game.UIs.Popup;
using _Game.Utilities.Timer;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VinhLB;

namespace _Game.UIs.Screen
{
    public class InGameScreen : UICanvas
    {
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private Image blockPanel;
        [SerializeField] private Button undoButton;
        [SerializeField] private Button resetIslandButton;
        [SerializeField] private TMP_Text timeText;
        
        private const float UNDO_CD_TIME = 0.3f;
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
            undoTimer = TimerManager.Inst.PopSTimer();
            resetIslandTimer = TimerManager.Inst.PopSTimer();
        }

        private void OnDestroy()
        {
            TimerManager.Inst.PushSTimer(undoTimer);
            TimerManager.Inst.PushSTimer(resetIslandTimer);
        }

        public event Action OnUndo;
        public event Action OnResetIsland;
        public event Action OnHint;
        public event Action OnCancelHint;

        public TextMeshProUGUI undoCountText;
        public TextMeshProUGUI resetCountText;
        public TextMeshProUGUI hintCountText;
        
        public override void Setup()
        {
            base.Setup();
            GameManager.Ins.ChangeState(GameState.InGame);
            MoveInputManager.Ins.ShowContainer(true);
            if (CameraManager.Ins.IsCurrentCameraIs(ECameraType.InGameCamera)) return;
            CameraManager.Ins.ChangeCamera(ECameraType.InGameCamera);
            canvasGroup.alpha = 0f;
            blockPanel.enabled = true;
        }

        public override void Open()
        {
            base.Open();

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

        #endregion
    }
}
