using _Game.DesignPattern;
using _Game.Utilities.Timer;
using AppsFlyerSDK;
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

        STimer cooldownTimer;
        Action interCallBack;

        int intAdsStepCount = 0;
        public bool IsBannerOpen => Banner.IsBannerOpen;
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
            MaxSdkCallbacks.Rewarded.OnAdRevenuePaidEvent += OnRewardedAdRevenuePaidEvent;
            cooldownTimer = new STimer();
        }
        public void ShowBannerAds()
        {
            int levelIndex = DataManager.Ins.GameData.user.normalLevelIndex;
            if (levelIndex < DataManager.Ins.ConfigData.startBannerAds)
                return;
            Banner.Show();
        }

        public void HideBannerAds()
        {
            if(IsBannerOpen)
                Banner.Hide();
        }
        private void CheckShowInterAds(object callBack = null)
        {
            int levelIndex = DataManager.Ins.GameData.user.normalLevelIndex;
            interCallBack = (Action)callBack;

            if (levelIndex < DataManager.Ins.ConfigData.startInterAdsLevel || cooldownTimer.IsStart 
                || (DebugManager.Ins && !DebugManager.Ins.IsShowAds))
            {
                interCallBack?.Invoke();
                interCallBack = null;
                return;
            }

            AnalysticManager.Ins.AppsFlyerTrackEvent("af_inters_logicgame");
            if ((levelIndex - DataManager.Ins.ConfigData.startInterAdsLevel) % DataManager.Ins.ConfigData.winLevelCountInterAds == 0)
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
            if (levelIndex < DataManager.Ins.ConfigData.startInterAdsLevel || cooldownTimer.IsStart
                || (DebugManager.Ins && !DebugManager.Ins.IsShowAds)) return;

            AnalysticManager.Ins.AppsFlyerTrackEvent("af_inters_logicgame");
            intAdsStepCount += (int)value;
            if(intAdsStepCount >= DataManager.Ins.ConfigData.stepInterAdsCountMax)
            {
                Interstitial.Show(OnInterAdsDone);
            }
        }
        private void OnInterAdsDone() {
            intAdsStepCount -= DataManager.Ins.ConfigData.stepInterAdsCountMax;
            cooldownTimer.Start(DataManager.Ins.ConfigData.interAdsCooldownTime);
            interCallBack?.Invoke();
            interCallBack = null;
        }
        private void OnRewardedAdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            //NOTE: Ad revenue paid. Use this callback to track user revenue.
            Dictionary<string,string> parameters = new Dictionary<string, string>();
            parameters.Add("ad_platform", "appLovin");
            parameters.Add("ad_source", adInfo.NetworkName);
            parameters.Add("ad_unit_name", adInfo.AdUnitIdentifier);
            parameters.Add("ad_format", adInfo.AdFormat);
            parameters.Add("placement", adInfo.Placement);
            parameters.Add("value", adInfo.Revenue.ToString());
            parameters.Add("currency", "USD");

            AppsFlyerAdRevenue.logAdRevenue(adInfo.NetworkName,
                AppsFlyerAdRevenueMediationNetworkType.AppsFlyerAdRevenueMediationNetworkTypeApplovinMax,
                adInfo.Revenue,
                "USD",
                parameters);
        }
        private void OnDestroy()
        {
            GameManager.Ins.UnregisterListenerEvent(EventID.OnInterAdsStepCount, OnInterAdsStepCount);
            GameManager.Ins.UnregisterListenerEvent(EventID.OnCheckShowInterAds, CheckShowInterAds);
        }
    }
}