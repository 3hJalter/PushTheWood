using System;
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
            public const int PLAYER_SKIN_COUNT = 6;
            // Level Progress Data
            public int normalLevelIndex;
            public int secretLevelIndex;
            public int secretLevelUnlock;
            
            // daily challenge
            public bool isCompleteDailyChallengerTutorial;
            public int currentDailyChallengerDay = 1;
            public List<int> dailyLevelIndexComplete = new();
            public List<int> dailyChallengeRewardCollected = new(); 
            
            // daily reward            
            public int dailyRewardClaimedCount;
            public DateTime startDailyRewardClaimTime = DateTime.UtcNow.Date;
            public DateTime lastDailyRewardClaimTime = DateTime.UtcNow.AddHours(-24);
            
            // Income Progress Data
            public int gold;
            public int adTickets;
            public int rewardChestKeys;
            public int levelChestProgress;
            public int currentRewardChestIndex = 0;
            public int currentLevelChestIndex = 0;
            public int rewardChestUnlock = 0;
            public int levelChestUnlock = 0;
            public int secretMapPieces;

            // Booster Data
            public int undoCount;
            public int resetIslandCount;
            public int growTreeCount;
            public int pushHintCount;
            
            // Purchase & First rate Data
            public bool purchasedNoAds;
            public bool rated;

            // TODO: Shop data
            public int currentPlayerSkinIndex = 0;
            public int[] playerSkinState = new int[PLAYER_SKIN_COUNT];
            
            //Other Data
            public int sessionPlayed;
            
            
            public DateTime lastTimeLogOut = DateTime.UtcNow;
            public bool isFirstDayOfWeekCheck;

            //Ads Data
            public int hintAdsCount;
            
            
        }

        [Serializable]
        public class SettingData
        {
            public bool enablePn;
            public bool requestedPn;
            public bool haptic = true;
            
            public bool isBgmMute;
            public bool isSfxMute;
            public bool isEnvSoundMute;
            
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
