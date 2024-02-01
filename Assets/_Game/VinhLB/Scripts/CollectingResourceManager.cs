using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using _Game.DesignPattern;
using _Game.Managers;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

namespace VinhLB
{
    public class CollectingResourceManager : Singleton<CollectingResourceManager>
    {
        [System.Serializable]
        private struct CollectingResourceConfig
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
        
        public void SpawnCollectingCoins(int amount, Vector3 startPosition, Transform endPoint, Action onReachEnd = null)
        {
            SpawnCollectingResource(_collectingCoinConfig, amount, startPosition, endPoint, onReachEnd);
        }

        private async void SpawnCollectingResource(CollectingResourceConfig config, int amount,
            Vector3 startPosition, Transform endPoint, Action onReachEnd)
        {
            List<Coin> coinList = new List<Coin>();
            // Spawn coins with random value
            for (int i = 0; i < amount; i++)
            {
                Coin coin = SimplePool.Spawn<Coin>(DataManager.Ins.GetUIUnit(config.PrefabPoolType));
                coin.Tf.SetParent(config.CollectingResourceParentTF, false);

                startPosition.z = 0f;
                Vector3 targetPosition = new Vector3(
                    startPosition.x + Random.Range(config.MinSpreadPosition.x, config.MaxSpreadPosition.x),
                    startPosition.y + Random.Range(config.MinSpreadPosition.y, config.MaxSpreadPosition.y));
                coin.Tf.DOMove(targetPosition, config.SpreadDuration).From(startPosition).SetEase(Ease.Linear);

                coinList.Add(coin);
            }
            
            // Delay before moving
            await Task.Delay(500);
            
            // Move coins to the coin label
            foreach (Coin coin in coinList)
            {
                coin.Tf.DOMove(endPoint.position, config.MoveDuration).SetEase(Ease.InBack).OnComplete(() =>
                {
                    coin.Despawn();
                    
                    if (config.ReactionTween == null)
                    {
                        // React to coins
                        config.ReactionTween = endPoint.DOPunchScale(config.ReactPunchScale, config.ReactDuration)
                            .SetEase(Ease.InOutElastic).OnComplete(() => { config.ReactionTween = null; });
                    }
                    
                    onReachEnd?.Invoke();
                });

                await Task.Delay(50);
            }
        }
    }
}