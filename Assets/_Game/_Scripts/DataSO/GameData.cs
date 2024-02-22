﻿using System;
using System.Collections.Generic;
using _Game.Utilities;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Game.Data
{
    [Serializable]
    public class GameData
    {
        public SettingData setting = new();
        public UserData user = new();

        [Serializable]
        public class UserData
        {
            // Level Progress Data
            public int normalLevelIndex;
            public int secretLevelIndex;
            public int secretLevelUnlock;
            
            // daily challenge
            public bool isCompleteDailyChallengerTutorial;
            public int currentDailyChallengerDay = 1;
            public List<int> dailyLevelIndexComplete = new();
            
            // daily reward            
            public int dailyRewardClaimedCount;
            public DateTime startDailyRewardClaimTime = DateTime.Now.Date;
            public DateTime lastDailyRewardClaimTime = DateTime.Now.AddHours(-24);
            
            // Income Progress Data
            public int gold;
            public int gems; // REMOVE: Later
            public int ticketAd;
            public int keyRewards;
            public int secretMapPieces;

            // Booster Data
            public int hintCount;
            public int undoCount;
            public int resetCount;
            public int growTreeCount;
            
            // Purchase & First rate Data
            public bool purchasedNoAds;
            public bool rated;

            // TODO: Shop data
            
            //Other Data
            public int sessionPlayed;
            
            
            public DateTime lastTimeLogOut = DateTime.Now;
            public bool isFirstDayOfWeekCheck;
            
            
        }

        [Serializable]
        public class SettingData
        {
            public bool enablePn;
            public bool requestedPn;
            public bool haptic = true;
            public float soundVolume = 1;
            public float musicVolume = 1;
            public int highPerformance = 1;
            public bool iOsTrackingRequested;
            public int moveChoice = 0;
        }
    }

    public static class Database
    {
        private const string DATA_KEY = "GameData";

        public static void SaveData(GameData data)
        {
            string dataString = JsonConvert.SerializeObject(data);
            PlayerPrefs.SetString(DATA_KEY, dataString);
            PlayerPrefs.Save();
        }

        public static GameData LoadData()
        {

            if (PlayerPrefs.HasKey(DATA_KEY))
            {
                return JsonConvert.DeserializeObject<GameData>(PlayerPrefs.GetString(DATA_KEY));
            }
            // If no game data can be loaded, create new one
            DevLog.Log(DevId.Hoang, "No game data can be loaded, create new one");
            GameData gameData = new();
            SaveData(gameData);
            return gameData;
        }
    }
}
