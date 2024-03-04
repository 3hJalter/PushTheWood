using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using _Game.DesignPattern;
using _Game.Managers;
using _Game.UIs.Screen;
using _Game.Utilities;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace VinhLB
{
    public class CollectingResourceManager : Singleton<CollectingResourceManager>
    {
        [System.Serializable]
        private class CollectingResourceConfig
        {
            public PoolType PrefabPoolType;
            public Vector3 MinSpreadPosition;
            public Vector3 MaxSpreadPosition;
            public float SpreadDuration;
            public float MoveDuration;
            public Vector3 ReactPunchScale;
            public float ReactDuration;

            [HideInInspector]
            public Transform CollectingResourceParentTF;
            public Tween ReactionTween;
        }

        [Title("Gold")]
        [SerializeField]
        private CollectingResourceConfig _collectingCoinConfig;

        [Title("Ad Tickets")]
        [SerializeField]
        private CollectingResourceConfig _collectingAdTicketConfig;

        [Title("Reward Keys")]
        [SerializeField]
        private CollectingResourceConfig _collectingRewardKeyConfig;

        private const float HEIGHT_OFFSET = 60;
        private const float HEIGHT_INCREASE = 50;

        public void SpawnCollectingCoins(int amount, Vector3 startPosition, Transform endPoint,
            Action<float> onEachReachEnd = null)
        {
            if (_collectingCoinConfig.CollectingResourceParentTF == null)
            {
                _collectingCoinConfig.CollectingResourceParentTF =
                    UIManager.Ins.OpenUI<OverlayScreen>().UICoinParentRectTF;
            }
            
            SpawnCollectingResource(_collectingCoinConfig, amount, startPosition, endPoint, onEachReachEnd);
        }

        public void SpawnCollectingAdTickets(int amount, Vector3 startPosition, Transform endPoint,
            Action<float> onEachReachEnd = null)
        {
            if (_collectingAdTicketConfig.CollectingResourceParentTF == null)
            {
                _collectingAdTicketConfig.CollectingResourceParentTF =
                    UIManager.Ins.OpenUI<OverlayScreen>().UIAdTicketParentRectTF;
            }
            
            SpawnCollectingResource(_collectingAdTicketConfig, amount, startPosition, endPoint, onEachReachEnd);
        }

        public void SpawnCollectingRewardKey(int amount, Transform objectTransform)
        {
            UIRewardKey unit = SimplePool.Spawn<UIRewardKey>(DataManager.Ins.GetUIUnit(PoolType.UIRewardKey));
            Vector2 viewPortPoint =
                CameraManager.Ins.WorldToViewportPoint(objectTransform.position) - Vector3.one * 0.5f;
            unit.Text.text = $"+{amount}";
            unit.CanvasGroup.alpha = 0;
            
            if (_collectingRewardKeyConfig.CollectingResourceParentTF == null)
            {
                _collectingRewardKeyConfig.CollectingResourceParentTF =
                    UIManager.Ins.OpenUI<OverlayScreen>().UIRewardKeyRectTF;
            }
            if (unit.Tf.parent != _collectingRewardKeyConfig.CollectingResourceParentTF)
            {
                unit.Tf.SetParent(_collectingRewardKeyConfig.CollectingResourceParentTF, false);
            }
            RectTransform parentRectTransform = _collectingRewardKeyConfig.CollectingResourceParentTF as RectTransform;

            Sequence s = DOTween.Sequence();
            s.Append(unit.CanvasGroup.DOFade(1, _collectingRewardKeyConfig.MoveDuration))
                .Join(DOVirtual.Float(0, HEIGHT_INCREASE, _collectingRewardKeyConfig.MoveDuration, y =>
                {
                    Vector2 viewPortPoint = CameraManager.Ins.WorldToViewportPoint(objectTransform.position) -
                                            Vector3.one * 0.5f;
                    unit.RectTf.anchoredPosition = new Vector2(parentRectTransform.rect.width * viewPortPoint.x,
                        parentRectTransform.rect.height * viewPortPoint.y + 60 + y);
                }).SetEase(Ease.OutQuart))
                .Append(unit.CanvasGroup.DOFade(0, _collectingRewardKeyConfig.MoveDuration * 1.5f).SetEase(Ease.InQuint))
                .Join(DOVirtual.Float(0, HEIGHT_INCREASE, _collectingRewardKeyConfig.MoveDuration * 1.5f, y =>
                {
                    Vector2 viewPortPoint = CameraManager.Ins.WorldToViewportPoint(objectTransform.position) -
                                            Vector3.one * 0.5f;
                    unit.RectTf.anchoredPosition = new Vector2(parentRectTransform.rect.width * viewPortPoint.x,
                        parentRectTransform.rect.height * viewPortPoint.y + HEIGHT_OFFSET + HEIGHT_INCREASE);
                }).SetEase(Ease.OutQuart))
                .OnComplete(() => OnDespawnUnit(unit));
            s.Play();

            void OnDespawnUnit(UIUnit uiUnit)
            {
                uiUnit.Despawn();
            }
        }

        private async void SpawnCollectingResource(CollectingResourceConfig config, int amount,
            Vector3 startPosition, Transform endPoint, Action<float> onEachReachEnd)
        {
            if (amount <= 0)
            {
                DevLog.Log(DevId.Vinh, $"Amount must be positive");
                return;
            }

            if (endPoint == null)
            {
                DevLog.Log(DevId.Vinh, $"EndPoint must be not null");
                return;
            }

            List<UIUnit> unitList = new List<UIUnit>();
            // Spawn units with random value
            for (int i = 0; i < amount; i++)
            {
                UIUnit unit = SimplePool.Spawn<UIUnit>(DataManager.Ins.GetUIUnit(config.PrefabPoolType));
                if (unit.transform.parent != config.CollectingResourceParentTF)
                {
                    unit.transform.SetParent(config.CollectingResourceParentTF, false);
                    unit.transform.position = startPosition;
                }
                
                Vector3 targetPosition = new Vector3(
                    startPosition.x + Random.Range(config.MinSpreadPosition.x, config.MaxSpreadPosition.x),
                    startPosition.y + Random.Range(config.MinSpreadPosition.y, config.MaxSpreadPosition.y),
                    startPosition.z);
                // unit.Tf.DOMove(targetPosition, config.SpreadDuration).SetEase(Ease.OutCirc);
                unit.Tf.DOJump(targetPosition, 1f, 1, config.SpreadDuration).SetEase(Ease.OutCirc);

                unitList.Add(unit);
            }

            // Delay before moving
            await Task.Delay(500);

            // Move units to the their corresponding label
            for (int i = 0; i < unitList.Count; i++)
            {
                UIUnit unit = unitList[i];
                float progress = (float)(i + 1) / unitList.Count;
                unit.Tf.DOMove(endPoint.position, config.MoveDuration).SetEase(Ease.InBack).OnComplete(() =>
                {
                    unit.Despawn();

                    if (config.ReactionTween == null)
                    {
                        // React to units
                        config.ReactionTween = endPoint.DOPunchScale(config.ReactPunchScale, config.ReactDuration)
                            .SetEase(Ease.InOutElastic).OnComplete(() => { config.ReactionTween = null; });
                    }

                    onEachReachEnd?.Invoke(progress);
                });

                await Task.Delay(50);
            }
        }
    }

    public class ResourceChangeData
    {
        public object Source;
        public float ChangedAmount;
        public float NewValue;
        public float OldValue;
    }
}