using _Game.Data;
using _Game.DesignPattern;
using _Game.GameGrid.GridSurface;
using _Game.GameGrid.Unit;
using UnityEngine;
using UnityEngine.Serialization;
using VinhLB;

namespace _Game.Managers
{
    public class DataManager : Singleton<DataManager>
    {

        #region Game Saving Data
        private GameData _gameData;
        
        public GameData GameData => _gameData ?? LoadData();

        public GameData LoadData()
        {
            _gameData = Database.LoadData();
            if (_gameData != null) return _gameData;
            // If no game data can be loaded, create new one
            _gameData = new GameData();
            Database.SaveData(_gameData);
            return _gameData;
        }      
        #endregion

        #region In-Game Data
        [SerializeField]
        private AudioData audioData;
        [SerializeField]
        private GridData gridData;
        [SerializeField]
        private MaterialData materialData;
        [SerializeField]
        private VFXData _vfxData;
        #endregion


        public AudioData AudioData => audioData;
        public VFXData VFXData => _vfxData;

        public int CountNormalLevel => gridData.CountNormalLevel;
        public int CountSurfaceMaterial => materialData.CountSurfaceMaterial;

        public void Save()
        {
            Database.SaveData(_gameData);
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
    }
}
