using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using _Game.DesignPattern;
using Firebase.Analytics;
using _Game.Data;
using _Game.Resource;
using _Game.Ads;
using _Game.Utilities.Timer;
using System;
using _Game._Scripts.InGame;

namespace _Game.Managers
{
    public class AnalysticManager : Singleton<AnalysticManager>
    {
        bool _firstAdsSession = false;
        STimer _firstAdsSessionTimer;
        int _firstAdsSessionTime = 0;
        private void Awake()
        {
            DontDestroyOnLoad(this);
            _firstAdsSessionTime = 0;
            _firstAdsSessionTimer = TimerManager.Ins.PopSTimer();
            _firstAdsSessionTimer.Start(1, () =>
            {
                _firstAdsSessionTime += 1;
            }, true);
        }

        public void LevelStart(LevelType type)
        {
            switch (type)
            {
                case LevelType.Normal:
                    FirebaseAnalytics.LogEvent("level_start", "level", DataManager.Ins.GameData.user.normalLevelIndex);
                    break;
                case LevelType.DailyChallenge:
                    FirebaseAnalytics.LogEvent("level_start", new Parameter[] 
                    { 
                        new Parameter("level_dailyChallenge", DataManager.Ins.GameData.user.currentDailyChallengerDay),
                        new Parameter("day", DateTime.UtcNow.Date.ToString())
                    });
                    break;
                case LevelType.Secret:
                    FirebaseAnalytics.LogEvent("level_start", "level_expedition", DataManager.Ins.GameData.user.secretLevelIndex);
                    break;
            }
        }

        public void LevelComplete(LevelType type)
        {
            Parameter[] param = new Parameter[3];
            switch (type)
            {
                case LevelType.Normal:
                    param[0] = new Parameter("level", DataManager.Ins.GameData.user.normalLevelIndex);
                    param[1] = new Parameter("retry", DataManager.Ins.GameData.user.retryTime);
                    param[2] = new Parameter("duration", GameplayManager.Ins.GameDuration);
                    break;
                case LevelType.DailyChallenge:
                    param[0] = new Parameter("level_dailyChallenge", DataManager.Ins.GameData.user.currentDailyChallengerDay);
                    param[2] = new Parameter("duration", GameplayManager.Ins.GameDuration);
                    break;
                case LevelType.Secret:
                    param[0] = new Parameter("level_expedition", DataManager.Ins.GameData.user.secretLevelIndex);
                    param[2] = new Parameter("duration", GameplayManager.Ins.GameDuration);
                    break;
            }

            FirebaseAnalytics.LogEvent("level_complete", param);
        }

        public void LevelFail(LevelType type, LevelLoseCondition loseCondition)
        {
            Parameter[] param = new Parameter[3];
            switch (type)
            {
                case LevelType.Normal:
                    param[0] = new Parameter("level", DataManager.Ins.GameData.user.normalLevelIndex);
                    param[1] = new Parameter("duration", GameplayManager.Ins.GameDuration);
                    param[2] = new Parameter("reason", loseCondition.ToString());
                    break;
                case LevelType.DailyChallenge:
                    param[0] = new Parameter("level_dailyChallenge", DataManager.Ins.GameData.user.currentDailyChallengerDay);
                    param[1] = new Parameter("duration", GameplayManager.Ins.GameDuration);
                    param[2] = new Parameter("reason", loseCondition.ToString());
                    break;
                case LevelType.Secret:
                    param[0] = new Parameter("level_expedition", DataManager.Ins.GameData.user.secretLevelIndex);
                    param[1] = new Parameter("duration", GameplayManager.Ins.GameDuration);
                    param[2] = new Parameter("reason", loseCondition.ToString());
                    break;
            }

            FirebaseAnalytics.LogEvent("level_fail", param);
        }

        public void BoosterSpend(BoosterType type)
        {
            FirebaseAnalytics.LogEvent("booster_spend", "name", type.ToString());
        }

        public void RewardAdsComplete(Ads.Placement place)
        {
            FirebaseAnalytics.LogEvent("ads_reward_complete", "placement", place.ToString());
            CheckFirstAdsSession(place);           
        }

        public void InterAdsShow(Ads.Placement place)
        {
            FirebaseAnalytics.LogEvent("ad_inter_show", "placement", place.ToString());
            CheckFirstAdsSession(place);
        }

        private void CheckFirstAdsSession(Ads.Placement place)
        {
            if (!_firstAdsSession)
            {
                _firstAdsSession = true;
                FirebaseAnalytics.LogEvent("first_ads_session", new Parameter[]
                {
                    new Parameter("placement", place.ToString()),
                    new Parameter("duration", _firstAdsSessionTime)
                });
                _firstAdsSessionTimer.Stop();
            }
        }
        public void ResourceEarn(BoosterType type,Resource.Placement place, int amount)
        {
            FirebaseAnalytics.LogEvent("resource_earn", new Parameter[]
            {
                new Parameter("name", type.ToString()),
                new Parameter("placement", place.ToString()),
                new Parameter("value", amount)
            });
        }

        public void ResourceEarn(CurrencyType type, Resource.Placement place, int amount)
        {
            FirebaseAnalytics.LogEvent("resource_earn", new Parameter[]
            {
                new Parameter("name", type.ToString()),
                new Parameter("placement", place.ToString()),
                new Parameter("value", amount)
            });
        }

        public void ResourceSpend(BoosterType type, Resource.Placement place, int amount)
        {
            FirebaseAnalytics.LogEvent("resource_spend", new Parameter[]
            {
                new Parameter("name", type.ToString()),
                new Parameter("placement", place.ToString()),
                new Parameter("value", amount)
            });
        }

        public void ResourceSpend(CurrencyType type, Resource.Placement place, int amount)
        {
            FirebaseAnalytics.LogEvent("resource_spend", new Parameter[]
            {
                new Parameter("name", type.ToString()),
                new Parameter("placement", place.ToString()),
                new Parameter("value", amount)
            });
        }

        public void FireUserProps()
        {
            FirebaseAnalytics.LogEvent("UserProps","Level",DataManager.Ins.GameData.user.normalLevelIndex);
        }

        public void Day()
        {
            FirebaseAnalytics.LogEvent("Day", "day", DataManager.Ins.GameData.user.playedDay);
        }
    }
}