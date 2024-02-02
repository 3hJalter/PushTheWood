﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using _Game.DesignPattern;
using _Game.Managers;
using _Game.Utilities;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
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

            public Transform CollectingResourceParentTF;
            public Tween ReactionTween;
        }

        [Title("Gold")]
        [SerializeField]
        private CollectingResourceConfig _collectingCoinConfig;

        [Title("Gems")]
        [SerializeField]
        private CollectingResourceConfig _collectingGemConfig;

        public void SpawnCollectingCoins(int amount, Vector3 startPosition, Transform endPoint,
            Action<float> onEachReachEnd = null)
        {
            SpawnCollectingResource(_collectingCoinConfig, amount, startPosition, endPoint, onEachReachEnd);
        }

        public void SpawnCollectingGems(int amount, Vector3 startPosition, Transform endPoint,
            Action<float> onEachReachEnd = null)
        {
            SpawnCollectingResource(_collectingGemConfig, amount, startPosition, endPoint, onEachReachEnd);
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

                if (unit.Tf.parent != config.CollectingResourceParentTF)
                {
                    unit.Tf.SetParent(config.CollectingResourceParentTF, false);
                }

                Vector3 targetPosition = new Vector3(
                    startPosition.x + Random.Range(config.MinSpreadPosition.x, config.MaxSpreadPosition.x),
                    startPosition.y + Random.Range(config.MinSpreadPosition.y, config.MaxSpreadPosition.y));
                unit.Tf.DOMove(targetPosition, config.SpreadDuration).From(startPosition).SetEase(Ease.OutCirc);

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