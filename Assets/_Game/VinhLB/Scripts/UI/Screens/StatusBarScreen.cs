﻿using System;
using System.Collections;
using System.Globalization;
using _Game.DesignPattern;
using _Game.Managers;
using _Game.UIs.Popup;
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
        [SerializeField]
        private HButton _addButton;

        private Coroutine _delayOpenCoroutine;
        
        private event Action _delayCollectingGold;
        private event Action _delayCollectingAdTickets;

        private void Awake()
        {
            GameManager.Ins.RegisterListenerEvent(EventID.OnChangeGold,
                data => ChangeGoldValue((ResourceChangeData)data));
            GameManager.Ins.RegisterListenerEvent(EventID.OnChangeAdTickets,
                data => ChangeAdTicketValue((ResourceChangeData)data));

            _addButton.onClick.AddListener(() =>
            {
                UIManager.Ins.OpenUI<NotificationPopup>(Constants.FEATURE_COMING_SOON);
            });
        }

        private void OnDestroy()
        {
            GameManager.Ins.UnregisterListenerEvent(EventID.OnChangeGold,
                data => ChangeGoldValue((ResourceChangeData)data));
            GameManager.Ins.UnregisterListenerEvent(EventID.OnChangeAdTickets,
                data => ChangeAdTicketValue((ResourceChangeData)data));
        }

        public override void Setup(object param = null)
        {
            base.Setup(param);

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
            
            _goldValueText.text =
                ((int)GameManager.Ins.SmoothGold).ToString(Constants.VALUE_FORMAT, CultureInfo.InvariantCulture);
            _adTicketValueText.text =
                ((int)GameManager.Ins.SmoothAdTickets).ToString(Constants.VALUE_FORMAT, CultureInfo.InvariantCulture);
        }

        public override void Open(object param = null)
        {
            base.Open(param);

            if (param is not true)
            {
                DOVirtual.Float(0f, 1f, 1f, value => _canvasGroup.alpha = value)
                    .OnComplete(() => { _blockPanel.gameObject.SetActive(false); });
            }

            if (_delayOpenCoroutine != null)
            {
                StopCoroutine(_delayOpenCoroutine);
            }
            _delayOpenCoroutine = StartCoroutine(DelayOpenCoroutine());
        }

        private void ChangeGoldValue(ResourceChangeData data)
        {
            if (data.ChangedAmount > 0)
            {
                if (!gameObject.activeInHierarchy)
                {
                    // _delayCollectingGold += SpawnCollectingUICoins;
                    GameManager.Ins.SmoothGold += data.ChangedAmount;
                }
                else
                {
                    SpawnCollectingUICoins();
                }
            }
            else
            {
                _goldValueText.text = data.NewValue.ToString(Constants.VALUE_FORMAT, CultureInfo.InvariantCulture);
            }

            void SpawnCollectingUICoins()
            {
                int collectingCoinAmount = Mathf.Min((int)data.ChangedAmount, Constants.MAX_UI_UNIT);
                Vector3 spawnPosition = data.Source as Vector3? ??
                                        CameraManager.Ins.ViewportToWorldPoint(new Vector3(0.5f, 0.5f));
                CollectingResourceManager.Ins.SpawnCollectingUICoins(collectingCoinAmount, spawnPosition, _goldIconTF,
                    (progress) =>
                    {
                        GameManager.Ins.SmoothGold += data.ChangedAmount / collectingCoinAmount;
                        _goldValueText.text =
                            ((int)GameManager.Ins.SmoothGold).ToString(Constants.VALUE_FORMAT, CultureInfo.InvariantCulture);
                    });
            }
        }

        private void ChangeAdTicketValue(ResourceChangeData data)
        {
            if (data.ChangedAmount > 0)
            {
                if (!gameObject.activeInHierarchy)
                {
                    // _delayCollectingAdTickets += SpawnCollectingUIAdTickets;
                    GameManager.Ins.SmoothAdTickets += data.ChangedAmount;
                }
                else
                {
                    SpawnCollectingUIAdTickets();
                }
            }
            else
            {
                _adTicketValueText.text =
                    data.NewValue.ToString(Constants.VALUE_FORMAT, CultureInfo.InvariantCulture);
            }

            void SpawnCollectingUIAdTickets()
            {
                int collectingAdTicketAmount = Mathf.Min((int)data.ChangedAmount, Constants.MAX_UI_UNIT);
                Vector3 spawnPosition = data.Source as Vector3? ??
                                        CameraManager.Ins.ViewportToWorldPoint(new Vector3(0.5f, 0.5f));
                CollectingResourceManager.Ins.SpawnCollectingUIAdTickets(collectingAdTicketAmount, spawnPosition,
                    _adTicketIconTF,
                    (progress) =>
                    {
                        GameManager.Ins.SmoothAdTickets += data.ChangedAmount / collectingAdTicketAmount;
                        _adTicketValueText.text =
                            ((int)GameManager.Ins.SmoothAdTickets).ToString(Constants.VALUE_FORMAT, CultureInfo.InvariantCulture);
                    });
            }
        }

        private IEnumerator DelayOpenCoroutine()
        {
            while (UIManager.Ins.IsOpened<SplashScreen>())
            {
                yield return null;
            }

            if (_delayCollectingGold != null)
            {
                _delayCollectingGold.Invoke();
                _delayCollectingGold = null;
            }
            if (_delayCollectingAdTickets != null)
            {
                _delayCollectingAdTickets?.Invoke();
                _delayCollectingAdTickets = null;
            }

            _delayOpenCoroutine = null;
        }
    }
}