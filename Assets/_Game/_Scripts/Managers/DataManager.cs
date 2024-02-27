using _Game.Data;
using _Game.DesignPattern;
using _Game.GameGrid.GridSurface;
using _Game.GameGrid.Unit;
using _Game.Resource;
using UnityEngine;
using VinhLB;

namespace _Game.Managers
{
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
        private ResourceDatabase _resourceDatabase;
        
        public ConfigData ConfigData => configData;
        public AudioData AudioData => audioData;
        public VFXData VFXData => vfxData;
        
        #endregion
        
        #region In-Game Function
        
        public int CountNormalLevel => gridData.CountNormalLevel;
        public int CountSecretLevel => gridData.CountSecretLevel;
        public int CountSurfaceMaterial => materialData.CountSurfaceMaterial;
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }
        public int GetLevelTime(LevelType type)
        {
            return type switch
            {
                LevelType.Normal => configData.timePerNormalLevel,
                LevelType.Secret => configData.timePerSecretLevel,
                LevelType.DailyChallenge => configData.timePerDailyChallengeLevel,
                _ => configData.timePerNormalLevel
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

        public ResourceData GetBoosterResourceData(BoosterType type)
        {
            return _resourceDatabase.BoosterResourceDataDict[type];
        }

        public ResourceData GetCurrencyResourceData(CurrencyType type)
        {
            return _resourceDatabase.CurrencyResourceDataDict[type];
        }
        
        #endregion
        
    }
}
