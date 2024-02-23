using _Game.DesignPattern;
using _Game.Utilities.Timer;
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

        STimer cooldownTimer;

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
            GameManager.Ins.RegisterListenerEvent(EventID.OnInterAdsStepCount, OnInterAdsStepCount);
            cooldownTimer = new STimer();
        }


        private void OnInterAdsStepCount(object value)
        {
            intAdsStepCount += (int)value;
            if(intAdsStepCount >= DataManager.Ins.ConfigData.stepInterAdsCountMax && !cooldownTimer.IsStart)
            {
                InterstitialAds.Show(StartTimer);
                intAdsStepCount = 0;
            }

            void StartTimer() { cooldownTimer.Start(DataManager.Ins.ConfigData.interAdsCooldownTime); }
        }

    }
}