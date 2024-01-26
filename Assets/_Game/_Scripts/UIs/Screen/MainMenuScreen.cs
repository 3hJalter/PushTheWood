using _Game.Camera;
using _Game.DesignPattern;
using _Game.GameGrid;
using _Game.Managers;
using _Game.UIs.Popup;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VinhLB;

namespace _Game.UIs.Screen
{
    public class MainMenuScreen : UICanvas
    {
        [SerializeField] private CanvasGroup _canvasGroup;

        [SerializeField] private Image _blockPanel;

        [SerializeField] private TabGroup _bottomNavigationTabGroup;

        [SerializeField] private TMP_Text _goldValueText;

        [SerializeField] private TMP_Text _gemValueText;

        private bool _isFirstOpen;

        private Tween goldChangeTween;

        private void Awake()
        {
            GameManager.Ins.RegisterListenerEvent(EventID.OnGoldMoneyChange, value => ChangeGoldValue((int)value));
            GameManager.Ins.RegisterListenerEvent(EventID.OnGemMoneyChange, value => ChangeGemValue((int)value));
        }

        private void OnDestroy()
        {
            GameManager.Ins.UnregisterListenerEvent(EventID.OnGoldMoneyChange, value => ChangeGoldValue((int)value));
            GameManager.Ins.UnregisterListenerEvent(EventID.OnGemMoneyChange, value => ChangeGemValue((int)value));
        }

        private void ChangeGoldValue(int value)
        {
            // If screen not open yet, just set value
            if (!gameObject.activeSelf) return;
            // If screen is open, play tween
            goldChangeTween?.Kill();
            goldChangeTween = DOTween.To(() => int.Parse(_goldValueText.text.Replace(",", "")),
                    x => _goldValueText.text = x.ToString("#,#"), value, 0.5f)
                .OnKill(() =>
                {
                    // Complete if be kill
                    _goldValueText.text = value.ToString("#,#");
                });
        }

        private void ChangeGemValue(int value)
        {
            if (!gameObject.activeSelf) return;
            goldChangeTween?.Kill();
            goldChangeTween = DOTween.To(() => int.Parse(_gemValueText.text.Replace(",", "")),
                    x => _gemValueText.text = x.ToString("#,#"), value, 0.5f)
                .OnKill(() =>
                {
                    _gemValueText.text = value.ToString("#,#");
                });
        }

        public override void Setup()
        {
            base.Setup();
            _blockPanel.gameObject.SetActive(true);
            _goldValueText.text = $"{DataManager.Ins.GameData.user.gold}";
            _gemValueText.text = $"{DataManager.Ins.GameData.user.gems}";
            _canvasGroup.alpha = 0f;
        }

        public override void Open()
        {
            base.Open();
            // CameraFollow.Ins.ChangeCamera(ECameraType.MainMenuCamera);
            // FxManager.Ins.PlayTweenFog();
            GameManager.Ins.ChangeState(GameState.MainMenu);
            CameraManager.Ins.ChangeCamera(ECameraType.MainMenuCamera);
            _bottomNavigationTabGroup.ResetSelectedTab();

            DOVirtual.Float(0, 1, 1f, value => _canvasGroup.alpha = value)
                .OnComplete(() => _blockPanel.gameObject.SetActive(false));
        }

        public void OnClickStart()
        {
            UIManager.Ins.OpenUI<InGameScreen>();
            LevelManager.Ins.InitLevel();
            Close();
        }

        public void OnClickSettingButton()
        {
            UIManager.Ins.OpenUI<SettingsPopup>();
        }
    }
}
