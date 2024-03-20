using System.Linq;
using _Game.Data;
using _Game.DesignPattern;
using _Game.GameGrid.GridSurface;
using _Game.GameGrid.Unit;
using _Game.Resource;
using UnityEngine;
using UnityEngine.Serialization;
using VinhLB;
using Random = System.Random;

namespace _Game.Managers
{
    [DefaultExecutionOrder(-100)]
    public class DataManager : Singleton<DataManager>
    {

        #region Game Saving Data
        private GameData _gameData;
        
        public GameData GameData => _gameData ?? Load();

        private GameData Load()
        {
            _gameData = Database.LoadData();
            return _gameData;
        }      
        public void Save()
        {
            Database.SaveData(_gameData);
        }
        #endregion
        
        #region In-Game Data
        [SerializeField] 
        private ConfigData configData;
        [SerializeField]
        private AudioData audioData;
        [SerializeField]
        private GridData gridData;
        [SerializeField]
        private MaterialData materialData;
        [SerializeField]
        private VFXData vfxData;
        [SerializeField]
        private UIResourceDatabase _uiResourceDatabase;
        
        public ConfigData ConfigData => configData;
        public AudioData AudioData => audioData;
        public VFXData VFXData => vfxData;
        public UIResourceDatabase UIResourceDatabase => _uiResourceDatabase;
        
        #endregion
        
        #region In-Game Function
        
        public int CountNormalLevel => gridData.CountNormalLevel;
        public int CountSecretLevel => gridData.CountSecretLevel;
        public int CountSurfaceMaterial => materialData.CountSurfaceMaterial;
        public int CurrentPlayerSkinIndex => GameData.user.currentPlayerSkinIndex;
        public int HintAdsCount => GameData.user.hintAdsCount;
        
        public int GoldCount => GameData.user.gold;
        
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        public int GetDailyChallengeDay(int index)
        {
            if (index >= Constants.DAILY_CHALLENGER_COUNT) return 0;
            return _gameData.user.dailyLevelIndex[index];
        }
        
        public bool IsClearAllDailyChallenge()
        {
            return _gameData.user.dailyLevelIndexComplete.Count >= _gameData.user.currentDailyChallengerDay;
        }

        public bool IsClearAllSecretLevel()
        {
            return _gameData.user.secretLevelIndexComplete.Count >= _gameData.user.secretLevelUnlock;
        }

        public bool IsAdsHintEnough()
        {
            return GameData.user.hintAdsCount >= ConfigData.GetBoosterConfig(BoosterType.PushHint).TicketPerBuyRatio.ticketNeed;
        }
        public bool IsCharacterSkinUnlock(int index)
        {
            return GameData.user.playerSkinState[index] != 0;
        }
        public void SetUnlockCharacterSkin(int index, bool value)
        {
            GameData.user.playerSkinState[index] = value ? 1 : 0;
        }

        public void SetCharacterSkinIndex(int index)
        {
            GameData.user.currentPlayerSkinIndex = index;
        }
        public int GetLevelTime(LevelType type, LevelNormalType normalType = LevelNormalType.None)
        {
            return type switch
            {
                LevelType.Normal => normalType is LevelNormalType.None
                    ? configData.timePerNormalLevel[(int) LevelNormalType.Medium].time
                    : configData.timePerNormalLevel[(int) normalType].time,
                LevelType.Secret => configData.timePerSecretLevel,
                LevelType.DailyChallenge => configData.timePerDailyChallengeLevel,
                _ => configData.timePerNormalLevel[(int)LevelNormalType.Medium].time
            };
        }
        
        public Material GetTransparentMaterial()
        {
            return materialData.GetTransparentMaterial();
        }
        
        public Material GetSurfaceMaterial(MaterialEnum materialEnum)
        {
            return materialData.GetSurfaceMaterial(materialEnum);
        }
        
        public Material GetGrassMaterial(MaterialEnum materialEnum)
        {
            return materialData.GetGrassMaterial(materialEnum);
        }
        
        public TextAsset GetLevelData(LevelType type, int index)
        {
            return gridData.GetLevelData(type, index);
        }
        
        public void AddGridTextData(LevelType type, TextAsset textAsset)
        {
            gridData.AddGridTextData(type, textAsset);
        }
        
        public bool HasGridTextData(LevelType type, TextAsset textAsset)
        {
            return gridData.HasGridTextData(type, textAsset);
        }

        public GridSurface GetGridSurface(PoolType poolType)
        {
            return gridData.GetGridSurface(poolType);
        }

        public GridUnit GetGridUnit(PoolType poolType)
        {
            return gridData.GetGridUnit(poolType);
        }

        public int GetGridTextDataIndex(LevelType type, TextAsset load)
        {
            return gridData.GetGridTextDataIndex(type, load);
        }

        public EnvironmentUnit GetRandomEnvironmentObject(PoolType poolType)
        {
            return gridData.GetRandomEnvironmentObject(poolType);
        }

        public UIUnit GetUIUnit(PoolType poolType)
        {
            return gridData.GetUIUnit(poolType);
        }
        
        public UIUnit GetWorldUIUnit(PoolType poolType)
        {
            return gridData.GetWorldUIUnit(poolType);
        }

        public UIResourceConfig GetBoosterUIResourceConfig(BoosterType type)
        {
            return _uiResourceDatabase.BoosterResourceConfigDict[type];
        }

        public UIResourceConfig GetCurrencyUIResourceConfig(CurrencyType type)
        {
            if (type is CurrencyType.RandomBooster)
            {
                return _uiResourceDatabase.BoosterResourceConfigDict.ElementAt(new Random().Next(0, _uiResourceDatabase.BoosterResourceConfigDict.Count)).Value;
            }
            return _uiResourceDatabase.CurrencyResourceConfigDict[type];
        }
        
        public UIResourceConfig GetCharacterUIResourceConfig(CharacterType type)
        {
            return _uiResourceDatabase.CharacterResourceConfigDict[type];
        }
        
        #endregion
    }
}
