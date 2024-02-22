using _Game.DesignPattern;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace _Game.Managers
{
    public class AdsManager : Singleton<AdsManager>
    {
        [SerializeField]
        BannerAds Banner;
        [SerializeField]
        RewardedAds Reward;
        [SerializeField]
        InterstitialAds Interstitial;
        public BannerAds BannerAds => Banner;
        public RewardedAds RewardedAds => Reward;
        public InterstitialAds InterstitialAds => Interstitial;

        int intAdsStepCount = 0;
        private void Start()
        {
            DontDestroyOnLoad(gameObject);
            MaxSdkCallbacks.OnSdkInitializedEvent += (MaxSdkBase.SdkConfiguration sdkConfiguration) =>
            {
                // AppLovin SDK is initialized, start loading ads
                Reward.Load();
                Interstitial.Load();
            };
            //GameManager.Ins.RegisterListenerEvent(EventID.OnInterAdsStepCount, OnInterAdsStepCount);
        }


        private void OnInterAdsStepCount()
        {
            intAdsStepCount++;
        }

    }
}