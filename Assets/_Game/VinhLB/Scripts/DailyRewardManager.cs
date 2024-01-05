using System;
using _Game.DesignPattern;
using _Game.Utilities;
using UnityEngine;

namespace VinhLB
{
    public class DailyRewardManager : Singleton<DailyRewardManager>
    {
        #region Debug
        
#if UNITY_EDITOR
        // [UnityEditor.MenuItem("Debug/Daily Reward/Print Parameters")]
        public static void PrintParameters()
        {
            if (Application.isPlaying)
            {
                DevLog.Log(DevId.Vinh, $"StartDateTime: {Ins._startDateTime}");
                DevLog.Log(DevId.Vinh, $"DailyRewardCount: {Ins.DailyRewardCount}");
                DevLog.Log(DevId.Vinh, $"LastDailyRewardDate: {Ins._lastDailyRewardDate}");
                DevLog.Log(DevId.Vinh, $"IsInFirstCycle: {Ins.IsInFirstCycle}");
                DevLog.Log(DevId.Vinh, $"TotalDays: {Ins.TotalDays}");
                DevLog.Log(DevId.Vinh, $"CycleDay: {Ins.CycleDay}");
                DevLog.Log(DevId.Vinh, $"IsTodayRewardObtained: {Ins.IsTodayRewardObtained}");
            }
            else
            {
                DevLog.Log(DevId.Vinh,
                    $"PREFS_DAILY_REWARD_START_DATE: {Ins.GetPlayerPrefsDateTime(PREFS_DAILY_REWARD_START_DATE)}");
                DevLog.Log(DevId.Vinh,
                    $"PREFS_DAILY_REWARD_COUNT: {Ins.GetPlayerPrefsDateTime(PREFS_DAILY_REWARD_COUNT)}");
                DevLog.Log(DevId.Vinh,
                    $"PREFS_LAST_DAILY_REWARD_DATE: {Ins.GetPlayerPrefsDateTime(PREFS_LAST_DAILY_REWARD_DATE, DateTime.Now.AddHours(-24))}");
            }
        }

        // [UnityEditor.MenuItem("Debug/Daily Reward/Set Can Collect Today")]
        public static void SetCanCollectToday()
        {
            PlayerPrefs.DeleteKey(PREFS_LAST_DAILY_REWARD_DATE);
            if (Application.isPlaying)
            {
                Ins._lastDailyRewardDate = DateTime.Now.AddHours(-24);
                
                Ins.OnDailyRewardParamsChanged?.Invoke();
            }
        }

        // [UnityEditor.MenuItem("Debug/Daily Reward/Increase 1 Daily Day")]
        public static void Increase1DailyDay()
        {
            if (Ins.DailyRewardSettingsSO.MissRewardIfNotLogin)
            {
                Ins.SetPlayerPrefsDateTime(PREFS_DAILY_REWARD_START_DATE,
                    Ins.GetPlayerPrefsDateTime(PREFS_DAILY_REWARD_START_DATE).AddDays(-1));
                Ins._startDateTime = Ins.GetPlayerPrefsDateTime(PREFS_DAILY_REWARD_START_DATE);
                DevLog.Log(DevId.Vinh, $"StartDateTime: {Ins._startDateTime}");
            }
            else
            {
                Ins.DailyRewardCount += 1;
                DevLog.Log(DevId.Vinh, $"DailyRewardCount: {Ins.DailyRewardCount}");
            }

            if (Application.isPlaying)
            {
                Ins.OnDailyRewardParamsChanged?.Invoke();
            }
        }

        // [UnityEditor.MenuItem("Debug/Daily Reward/Decrease 1 Daily Day")]
        public static void Decrease1DailyDay()
        {
            if (Ins.DailyRewardSettingsSO.MissRewardIfNotLogin)
            {
                Ins._startDateTime = Ins.GetPlayerPrefsDateTime(PREFS_DAILY_REWARD_START_DATE);
                if (Ins._startDateTime.AddDays(1) <= DateTime.Now.Date)
                {
                    // Make sure start date is not after today
                    Ins.SetPlayerPrefsDateTime(PREFS_DAILY_REWARD_START_DATE,
                        Ins.GetPlayerPrefsDateTime(PREFS_DAILY_REWARD_START_DATE).AddDays(1));
                    Ins._startDateTime = Ins.GetPlayerPrefsDateTime(PREFS_DAILY_REWARD_START_DATE);
                }

                DevLog.Log(DevId.Vinh, $"StartDateTime: {Ins._startDateTime}");
            }
            else
            {
                Ins.DailyRewardCount = Mathf.Max(0, Ins.DailyRewardCount - 1);
                DevLog.Log(DevId.Vinh, $"DailyRewardCount: {Ins.DailyRewardCount}");
            }

            if (Application.isPlaying)
            {
                Ins.OnDailyRewardParamsChanged?.Invoke();
            }
        }
        
        // [UnityEditor.MenuItem("Debug/Daily Reward/Reset All")]
        public static void ResetAll()
        {
            if (!Application.isPlaying)
            {
                PlayerPrefs.DeleteKey(PREFS_DAILY_REWARD_START_DATE);
                PlayerPrefs.DeleteKey(PREFS_DAILY_REWARD_COUNT);
                PlayerPrefs.DeleteKey(PREFS_LAST_DAILY_REWARD_DATE);
            }
        }
#endif
        
        #endregion

        private const string PREFS_DAILY_REWARD_START_DATE = "DailyRewardStartDate";
        private const string PREFS_DAILY_REWARD_COUNT = "DailyRewardCount";
        private const string PREFS_LAST_DAILY_REWARD_DATE = "LastDailyRewardDate";

        public event Action OnDailyRewardParamsChanged;

        [SerializeField]
        private DailyRewardSettingsSO _dailyRewardSettingsSO;

        private DateTime _startDateTime;
        private DateTime _lastDailyRewardDate;
        private bool _isEnabled;

        #region Properties

        public DailyRewardSettingsSO DailyRewardSettingsSO => _dailyRewardSettingsSO;
        public bool IsTodayRewardObtained => DateTime.Now.Date <= _lastDailyRewardDate;
        public int DailyRewardCount
        {
            get { return PlayerPrefs.GetInt(PREFS_DAILY_REWARD_COUNT); }
            set { PlayerPrefs.SetInt(PREFS_DAILY_REWARD_COUNT, value); }
        }
        public int TotalDays => _dailyRewardSettingsSO.MissRewardIfNotLogin
            ? (DateTime.Now.Date - _startDateTime.Date).Days
            : (DailyRewardCount - (IsTodayRewardObtained ? 1 : 0));
        public int CycleDay => TotalDays % _dailyRewardSettingsSO.CycleDays;
        public bool IsInFirstCycle => TotalDays / _dailyRewardSettingsSO.CycleDays == 0;
        public Reward[] RewardArray => IsInFirstCycle && _dailyRewardSettingsSO.DifferentFirstCycle
            ? _dailyRewardSettingsSO.FirstCycleRewardArray
            : _dailyRewardSettingsSO.RewardArray;
        public Reward CurrentReward => RewardArray[CycleDay];

        #endregion

        private void Awake()
        {
            if (!PlayerPrefs.HasKey(PREFS_DAILY_REWARD_START_DATE))
            {
                SetPlayerPrefsDateTime(PREFS_DAILY_REWARD_START_DATE, DateTime.Now.Date);
            }

            _startDateTime = GetPlayerPrefsDateTime(PREFS_DAILY_REWARD_START_DATE);
            _lastDailyRewardDate = GetPlayerPrefsDateTime(PREFS_LAST_DAILY_REWARD_DATE, DateTime.Now.AddHours(-24));
        }

        public void ObtainTodayReward()
        {
            if (IsTodayRewardObtained)
            {
                DevLog.Log(DevId.Vinh, $"Day {CycleDay + 1} reward is obtained");
                return;
            }

            DevLog.Log(DevId.Vinh, $"Obtain day {CycleDay + 1} reward successfully");
            CurrentReward.Obtain();

            DailyRewardCount += 1;
            _lastDailyRewardDate = DateTime.Now.Date;
            SetPlayerPrefsDateTime(PREFS_LAST_DAILY_REWARD_DATE, _lastDailyRewardDate);

            Ins.OnDailyRewardParamsChanged?.Invoke();
        }

        private void SetPlayerPrefsDateTime(string key, DateTime dateTime)
        {
            string dateTimeString = dateTime.ToShortDateString();
            PlayerPrefs.SetString(key, dateTimeString);
        }

        private DateTime GetPlayerPrefsDateTime(string key, DateTime defaultDateTime = default)
        {
            string dateTimeString = PlayerPrefs.GetString(key);
            if (!DateTime.TryParse(dateTimeString, out DateTime dateTime))
            {
                dateTime = defaultDateTime;
            }

            return dateTime;
        }
    }
}