using System.Globalization;
using _Game.Camera;
using _Game.DesignPattern;
using _Game.GameGrid;
using _Game.Managers;
using _Game.UIs.Popup;
using AudioEnum;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VinhLB;

namespace _Game.UIs.Screen
{
    public class MainMenuScreen : UICanvas
    {
        [SerializeField]
        private CanvasGroup _canvasGroup;
        [SerializeField]
        private Image _blockPanel;
        [SerializeField]
        private TabGroup _bottomNavigationTabGroup;
        [SerializeField]
        private TMP_Text _goldValueText;
        [SerializeField]
        private Transform _goldIconTF;
        [SerializeField]
        private TMP_Text _adTicketValueText;
        [SerializeField]
        private Transform _adTicketIconTF;

        private bool _isFirstOpen;
        private Tween _goldChangeTween;

        private void Awake()
        {
            GameManager.Ins.RegisterListenerEvent(EventID.OnGoldChange,
                data => ChangeGoldValue((ResourceChangeData)data));
            GameManager.Ins.RegisterListenerEvent(EventID.OnAdTicketsChange,
                data => ChangeAdTicketValue((ResourceChangeData)data));
        }

        private void OnDestroy()
        {
            GameManager.Ins.UnregisterListenerEvent(EventID.OnGoldChange,
                data => ChangeGoldValue((ResourceChangeData)data));
            GameManager.Ins.UnregisterListenerEvent(EventID.OnAdTicketsChange,
                data => ChangeAdTicketValue((ResourceChangeData)data));
        }

        public override void Setup(object param = null)
        {
            base.Setup(param);

            _goldValueText.text = $"{GameManager.Ins.Gold}";
            _adTicketValueText.text = $"{GameManager.Ins.AdTickets}";

            if (param is true)
            {
                _canvasGroup.alpha = 1f;
                _blockPanel.gameObject.SetActive(false);
            }
            else
            {
                _canvasGroup.alpha = 0f;
                _blockPanel.gameObject.SetActive(true);
            }
        }

        public override void Open(object param = null)
        {
            base.Open(param);
            // CameraFollow.Ins.ChangeCamera(ECameraType.MainMenuCamera);
            // FxManager.Ins.PlayTweenFog();
            DebugManager.Ins?.OpenDebugCanvas(UI_POSITION.MAIN_MENU);
            GameManager.Ins.ChangeState(GameState.MainMenu);
            CameraManager.Ins.ChangeCamera(ECameraType.MainMenuCamera);
            AudioManager.Ins.PlayBgm(BgmType.MainMenu, 1f);
            AudioManager.Ins.StopEnvironment();
            if (param is true)
            {
                _bottomNavigationTabGroup.ResetSelectedTab(false);
            }
            else
            {
                DOVirtual.Float(0f, 1f, 1f, value => _canvasGroup.alpha = value)
                    .OnComplete(() => _blockPanel.gameObject.SetActive(false));

                _bottomNavigationTabGroup.ResetSelectedTab(true);
            }
        }

        public override void Close()
        {
            base.Close();

            _bottomNavigationTabGroup.ClearSelectedTab();
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

        private void ChangeGoldValue(ResourceChangeData data)
        {
            // If screen not open yet, just set value
            if (!gameObject.activeSelf)
            {
                return;
            }

            // _goldValueText.text = data.NewValue.ToString(Constants.VALUE_FORMAT);

            // If screen is open, play tween
            // _goldChangeTween?.Kill();
            // _goldChangeTween = DOTween.To(() => int.Parse(_goldValueText.text.Replace(",", "")),
            //         x => _goldValueText.text = x.ToString("#,#"), value, 0.5f)
            //     .OnKill(() =>
            //     {
            //         // Complete if be kill
            //         _goldValueText.text = value.ToString("#,#");
            //     });

            if (data.ChangedAmount > 0 && data.Source is Vector3 spawnPosition)
            {
                int collectingCoinAmount = Mathf.Min((int)data.ChangedAmount, 8);
                CollectingResourceManager.Ins.SpawnCollectingCoins(collectingCoinAmount, spawnPosition, _goldIconTF,
                    (progress) =>
                    {
                        GameManager.Ins.SmoothGold += data.ChangedAmount / collectingCoinAmount;
                        _goldValueText.text =
                            GameManager.Ins.SmoothGold.ToString(Constants.VALUE_FORMAT, CultureInfo.InvariantCulture);
                    });
            }
            else
            {
                _goldValueText.text = data.NewValue.ToString(Constants.VALUE_FORMAT, CultureInfo.InvariantCulture);
            }
        }

        private void ChangeAdTicketValue(ResourceChangeData data)
        {
            // If screen not open yet, just set value
            if (!gameObject.activeSelf)
            {
                return;
            }

            // If screen is open, play tween
            // _goldChangeTween?.Kill();
            // _goldChangeTween = DOTween.To(() => int.Parse(_gemValueText.text.Replace(",", "")),
            //         x => _gemValueText.text = x.ToString("#,#"), value, 0.5f)
            //     .OnKill(() => { _gemValueText.text = value.ToString("#,#"); });

            if (data.ChangedAmount > 0 && data.Source is Vector3 spawnPosition)
            {
                float currentValue = data.OldValue;
                int collectingAdTicketAmount = Mathf.Min((int)data.ChangedAmount, 8);
                CollectingResourceManager.Ins.SpawnCollectingAdTickets(collectingAdTicketAmount, spawnPosition,
                    _adTicketIconTF,
                    (progress) =>
                    {
                        currentValue += data.ChangedAmount / collectingAdTicketAmount;
                        _adTicketValueText.text =
                            currentValue.ToString(Constants.VALUE_FORMAT, CultureInfo.InvariantCulture);
                    });
            }
            else
            {
                _adTicketValueText.text = data.NewValue.ToString(Constants.VALUE_FORMAT, CultureInfo.InvariantCulture);
            }
        }
    }
}