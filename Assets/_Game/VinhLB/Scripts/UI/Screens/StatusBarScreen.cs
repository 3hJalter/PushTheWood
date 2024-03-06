using System.Globalization;
using _Game.DesignPattern;
using _Game.Managers;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace VinhLB
{
    public class StatusBarScreen : UICanvas
    {
        [SerializeField]
        private CanvasGroup _canvasGroup;
        [SerializeField]
        private Image _blockPanel;
        [SerializeField]
        private TMP_Text _goldValueText;
        [SerializeField]
        private Transform _goldIconTF;
        [SerializeField]
        private TMP_Text _adTicketValueText;
        [SerializeField]
        private Transform _adTicketIconTF;

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

            if (param is not true)
            {
                DOVirtual.Float(0f, 1f, 1f, value => _canvasGroup.alpha = value)
                    .OnComplete(() => _blockPanel.gameObject.SetActive(false));
            }
        }

        private void ChangeGoldValue(ResourceChangeData data)
        {
            // If screen not open yet, just set value
            if (!gameObject.activeSelf)
            {
                return;
            }

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