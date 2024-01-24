﻿using System;
using _Game.Managers;
using Newtonsoft.Json;
using UnityEngine;

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
            public int inGameLevel;

            //Progress Data
            public int money;
            public int score;
            public int luckyWheelProgress;
            public bool lastFreeSpinState;
            public int dailyRewardClaimedCount;

            //Purchase & First rate Data
            public bool purchasedNoAds;
            public bool rated;

            // TODO: Shop data
            
            //Other Data
            public int sessionPlayed;
            private int _keyCount;
            public DateTime lastDailyRewardClaimTime = DateTime.MinValue;
            public DateTime lastFreeSpinTime = DateTime.MinValue;
            public DateTime lastTimeLogOut = DateTime.Now;

            public int KeyCount
            {
                get => _keyCount;
                set => _keyCount = Math.Clamp(value, 0, 3);
            }
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
        }
    }

    public static class Database
    {
        private const string DATA_KEY = "GameData";

        public static void SaveData()
        {
            string dataString = JsonConvert.SerializeObject(DataManager.Ins.GameData);
            PlayerPrefs.SetString(DATA_KEY, dataString);
            PlayerPrefs.Save();
        }

        public static GameData LoadData()
        {
            return PlayerPrefs.HasKey(DATA_KEY) ? JsonConvert.DeserializeObject<GameData>(PlayerPrefs.GetString(DATA_KEY)) : null;
        }
    }
}
