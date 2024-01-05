using _Game._Scripts.Managers;
using _Game.Camera;
using _Game.GameGrid;
using _Game.Managers;
using _Game.UIs.Popup;
using _Game.Utilities.Timer;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using VinhLB;

namespace _Game.UIs.Screen
{
    public class InGameScreen : UICanvas
    {
        private float COOL_DOWN_TIME = 0.3f;
        [SerializeField] private Image blockPanel;
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private Button undoButton;
        [SerializeField] private Button resetIslandButton;

        STimer undoTimer;
        STimer resetTimer;
        private void Awake()
        {
            undoTimer = TimerManager.Inst.PopSTimer();
            resetTimer = TimerManager.Inst.PopSTimer();
        }

        public override void Setup()
        {
            base.Setup();
            GameManager.Ins.ChangeState(GameState.InGame);
            MoveInputManager.Ins.OnChangeMoveChoice(MoveInputManager.Ins
                .CurrentChoice); // TODO: Change to use PlayerRef 
            // if (CameraFollow.Ins.IsCurrentCameraIs(ECameraType.InGameCamera)) return;
            // CameraFollow.Ins.ChangeCamera(ECameraType.InGameCamera);
            if (CameraManager.Ins.IsCurrentCameraIs(ECameraType.InGameCamera)) return;
            CameraManager.Ins.ChangeCamera(ECameraType.InGameCamera);
            blockPanel.enabled = true;
        }

        public override void Open()
        {
            base.Open();
            DOVirtual.Float(0, 1, 1f, value => canvasGroup.alpha = value)
                .OnComplete(() => { blockPanel.enabled = false; });
        }

        public override void Close()
        {
            MoveInputManager.Ins.HideButton();
            base.Close();
        }

        public void OnClickSettingButton()
        {
            UIManager.Ins.OpenUI<SettingsPopup>();
        }

        public void OnClickOpenMapButton()
        {
            Close();
            UIManager.Ins.OpenUI<WorldMapScreen>();
        }

        public void OnClickResetIslandButton()
        {
            LevelManager.Ins.CurrentLevel.ResetIslandPlayerOn();
            resetIslandButton.interactable = false;
            resetTimer.Start(COOL_DOWN_TIME, () => resetIslandButton.interactable = true);
        }

        public void OnClickToggleBuildingMode()
        {
            Close();
            UIManager.Ins.OpenUI<BuildingScreen>();

            GridBuildingManager.Ins.ToggleBuildMode();
        }

        public void OnClickUndo()
        {
            LevelManager.Ins.OnUndo();
            undoButton.interactable = false;
            undoTimer.Start(COOL_DOWN_TIME, () => undoButton.interactable = true);
        }

        private void OnDestroy()
        {
            TimerManager.Inst.PushSTimer(undoTimer);
            TimerManager.Inst.PushSTimer(resetTimer);
        }
    }
}
