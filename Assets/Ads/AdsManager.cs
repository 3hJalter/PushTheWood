using _Game.DesignPattern;
using _Game.Utilities.Timer;
using System;
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
        public RewardedAds RewardedAds => Reward;
        public BannerAds BannerAds => Banner;

        STimer cooldownTimer;
        Action interCallBack;

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
            GameManager.Ins.RegisterListenerEvent(EventID.OnCheckShowInterAds, CheckShowInterAds);
            cooldownTimer = new STimer();
        }
        
        private void CheckShowInterAds(object callBack = null)
        {
            int levelIndex = DataManager.Ins.GameData.user.normalLevelIndex;
            interCallBack = (Action)callBack;

            if (levelIndex < DataManager.Ins.ConfigData.startInterAdsLevel || cooldownTimer.IsStart)
            {
                interCallBack?.Invoke();
                interCallBack = null;
                return;
            }

            if((levelIndex - DataManager.Ins.ConfigData.startInterAdsLevel) % DataManager.Ins.ConfigData.winLevelCountInterAds == 0)
            {
                Interstitial.Show(OnInterAdsDone);
                return;
            }
            interCallBack?.Invoke();
            interCallBack = null;
        }
        private void OnInterAdsStepCount(object value)
        {
            int levelIndex = DataManager.Ins.GameData.user.normalLevelIndex;
            if (levelIndex < DataManager.Ins.ConfigData.startInterAdsLevel || cooldownTimer.IsStart) return;

            intAdsStepCount += (int)value;
            if(intAdsStepCount >= DataManager.Ins.ConfigData.stepInterAdsCountMax)
            {
                Interstitial.Show(OnInterAdsDone);
                intAdsStepCount -= DataManager.Ins.ConfigData.stepInterAdsCountMax;
            }
        }
        void OnInterAdsDone() { 
            cooldownTimer.Start(DataManager.Ins.ConfigData.interAdsCooldownTime);
            interCallBack?.Invoke();
            interCallBack = null;
        }

        private void OnDestroy()
        {
            GameManager.Ins.UnregisterListenerEvent(EventID.OnInterAdsStepCount, OnInterAdsStepCount);
            GameManager.Ins.UnregisterListenerEvent(EventID.OnCheckShowInterAds, CheckShowInterAds);
        }
    }
}