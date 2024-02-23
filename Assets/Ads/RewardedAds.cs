using _Game._Scripts.Managers;
using _Game.Data;
using _Game.Managers;
using _Game.Resource;
using Newtonsoft.Json.Bson;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using VinhLB;

public class RewardedAds : MonoBehaviour
{
    string adUnitId = "e9cf2be5a77927b0";
    int retryAttempt;
    bool showImmediate = false;

    List<BoosterType> boosterTypes;
    List<RESOURCE_TYPE> resourceTypes;

    List<int> boosterAmounts;
    List<int> resourceAmounts;

    // Start is called before the first frame update
    void Start()
    {
        // Attach callback
        MaxSdkCallbacks.Rewarded.OnAdLoadedEvent += OnRewardedAdLoadedEvent;
        MaxSdkCallbacks.Rewarded.OnAdLoadFailedEvent += OnRewardedAdLoadFailedEvent;
        MaxSdkCallbacks.Rewarded.OnAdDisplayedEvent += OnRewardedAdDisplayedEvent;
        MaxSdkCallbacks.Rewarded.OnAdClickedEvent += OnRewardedAdClickedEvent;
        MaxSdkCallbacks.Rewarded.OnAdRevenuePaidEvent += OnRewardedAdRevenuePaidEvent;
        MaxSdkCallbacks.Rewarded.OnAdHiddenEvent += OnRewardedAdHiddenEvent;
        MaxSdkCallbacks.Rewarded.OnAdDisplayFailedEvent += OnRewardedAdFailedToDisplayEvent;
        MaxSdkCallbacks.Rewarded.OnAdReceivedRewardEvent += OnRewardedAdReceivedRewardEvent;
    }
    public void Load(bool show = false)
    {
        MaxSdk.LoadRewardedAd(adUnitId);
        showImmediate = show;
    }

    public void Show(List<RESOURCE_TYPE> resourceTypes = null, List<int> resourceAmount = null,
        List<BoosterType> boosterTypes = null, List<int> boosterAmount = null)
    {
        this.resourceTypes = resourceTypes;
        this.resourceAmounts = resourceAmount;
        this.boosterTypes = boosterTypes;
        this.boosterAmounts = boosterAmount;
        Show();
    }

    private void Show()
    {
        if (MaxSdk.IsRewardedAdReady(adUnitId))
        {
            MaxSdk.ShowRewardedAd(adUnitId);
        }
        else
        {
            Debug.LogWarning("Rewarded ad is not ready. Make sure to load it before showing.");
            Load(true); // Load a new rewarded ad
        }
    }

    private void OnRewardedAdLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Rewarded ad is ready for you to show. MaxSdk.IsRewardedAdReady(adUnitId) now returns 'true'.
        if (showImmediate)
            MaxSdk.ShowRewardedAd(adUnitId);
        // Reset retry attempt
        retryAttempt = 0;
    }

    private void OnRewardedAdLoadFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
    {
        // Rewarded ad failed to load
        // AppLovin recommends that you retry with exponentially higher delays, up to a maximum delay (in this case 64 seconds).

        retryAttempt++;
        double retryDelay = Math.Pow(2, Math.Min(6, retryAttempt));

        Invoke("LoadRewardedAd", (float)retryDelay);
    }

    private void OnRewardedAdDisplayedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) { }

    private void OnRewardedAdFailedToDisplayEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
    {
        // Rewarded ad failed to display. AppLovin recommends that you load the next ad.
        Load();
    }

    private void OnRewardedAdClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) { }

    private void OnRewardedAdHiddenEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Rewarded ad is hidden. Pre-load the next ad
    }

    private void OnRewardedAdReceivedRewardEvent(string adUnitId, MaxSdk.Reward reward, MaxSdkBase.AdInfo adInfo)
    {
        if (resourceTypes != null)
        {

        }

        if (boosterTypes != null)
        {
            for (int i = 0; i < boosterTypes.Count; i++)
            {
                EventGlobalManager.Ins.OnChangeBoosterAmount.Dispatch(boosterTypes[i], boosterAmounts[i]);
            }
        }
    }

    private void OnRewardedAdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Ad revenue paid. Use this callback to track user revenue.
    }

    // Update is called once per frame
    void Update()
    {
        // Update logic here
    }
}