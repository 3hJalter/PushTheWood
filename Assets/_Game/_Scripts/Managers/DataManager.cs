using _Game.Data;
using _Game.DesignPattern;
using _Game.GameGrid.GridSurface;
using _Game.GameGrid.Unit;
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
        
        public ConfigData ConfigData => configData;
        public AudioData AudioData => audioData;
        public VFXData VFXData => vfxData;
        
        #endregion
        
        #region In-Game Function

        public int CountNormalLevel => gridData.CountNormalLevel;
        public int CountSurfaceMaterial => materialData.CountSurfaceMaterial;
        
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
        
        public TextAsset GetNormalLevelData(LevelType type, int index)
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

        public EnvironmentObject GetRandomEnvironmentObject(PoolType poolType)
        {
            return gridData.GetRandomEnvironmentObject(poolType);
        }

        public WorldUI GetWorldUI(PoolType poolType)
        {
            return gridData.GetWorldUI(poolType);
        }

        #endregion
        
    }
}
