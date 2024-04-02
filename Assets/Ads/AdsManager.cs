using _Game.DesignPattern;
using _Game.GameGrid;
using _Game.Utilities.Timer;
using AppsFlyerSDK;
using GoogleMobileAds.Api;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VinhLB;
using static UnityEngine.Rendering.DebugUI;

namespace _Game.Managers
{
    [DefaultExecutionOrder(-100)]
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
        STimer bannerTimer;
        Action interCallBack;

        int intAdsStepCount = 0;
        public bool IsBannerOpen => Banner.IsBannerOpen;
        
        protected void Awake()
        {
            DontDestroyOnLoad(gameObject);
            bannerTimer = TimerManager.Ins.PopSTimer();

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
            if (levelIndex < DataManager.Ins.ConfigData.startBannerAds || (DebugManager.Ins && !DebugManager.Ins.IsShowAds))
                return;
            Banner.Show();
            bannerTimer.Start(DataManager.Ins.ConfigData.reloadBannerTime, Banner.Show, true);
        }

        public void HideBannerAds()
        {
            if (DebugManager.Ins && !DebugManager.Ins.IsShowAds)
                return;
            if(IsBannerOpen)
                Banner.Hide();
        }
        
        private void CheckShowInterAds(object callBack = null)
        {
            if(cooldownTimer.IsStart || (DebugManager.Ins && !DebugManager.Ins.IsShowAds))
            {
                interCallBack?.Invoke();
                interCallBack = null;
                return;
            }
                
            switch (LevelManager.Ins.CurrentLevel.LevelType)
            {
                case Data.LevelType.Normal:
                    int levelIndex = DataManager.Ins.GameData.user.normalLevelIndex;
                    interCallBack = (Action)callBack;

                    if (levelIndex < DataManager.Ins.ConfigData.startInterAdsLevel)
                    {
                        interCallBack?.Invoke();
                        interCallBack = null;
                        return;
                    }

                    AnalysticManager.Ins.AppsFlyerTrackEvent("af_inters_logicgame");
                    if ((levelIndex - DataManager.Ins.ConfigData.startInterAdsLevel) % DataManager.Ins.ConfigData.winLevelCountInterAds == 0)
                    {
                        ShowInterAdsWithSplashScreen();
                        return;
                    }
                    interCallBack?.Invoke();
                    interCallBack = null;
                    break;
                case Data.LevelType.Secret:
                    ShowInterAdsWithSplashScreen();
                    break;
                case Data.LevelType.DailyChallenge:
                    OnInterAdsStepCount(1);
                    break;
            }
            
        }
        
        private void ShowInterAds()
        {
            Interstitial.Show(OnInterAdsDone);
        }

        private void ShowInterAdsWithSplashScreen()
        {
            UIManager.Ins.OpenUI<SplashScreen>((Action)(() => Interstitial.Show(() =>
            {
                OnInterAdsDone();
                UIManager.Ins.CloseUI<SplashScreen>();
            })));
        }
        
        private void OnInterAdsStepCount(object value)
        {
            int levelIndex = DataManager.Ins.GameData.user.normalLevelIndex;
            if (levelIndex < DataManager.Ins.ConfigData.startInterAdsLevel || cooldownTimer.IsStart
                || (DebugManager.Ins && !DebugManager.Ins.IsShowAds)) return;

            AnalysticManager.Ins.AppsFlyerTrackEvent("af_inters_logicgame");
            DataManager.Ins.AddInterAdsStepCount((int)value);
            if (DataManager.Ins.InterAdsStepCount >= DataManager.Ins.ConfigData.stepInterAdsCountMax)
            {
                ShowInterAdsWithSplashScreen();
            }
        }
        
        private void OnInterAdsDone() {
            DataManager.Ins.AddInterAdsStepCount(-DataManager.Ins.ConfigData.stepInterAdsCountMax);
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