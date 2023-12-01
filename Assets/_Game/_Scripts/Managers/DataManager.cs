using _Game._Scripts.UIs.Tutorial;
using _Game.Data;
using _Game.DesignPattern;
using _Game.GameGrid.GridSurface;
using _Game.GameGrid.Unit;
using GameGridEnum;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Game.Managers
{
    public class DataManager : Singleton<DataManager>
    {
        [SerializeField] private AudioData audioData;

        [SerializeField] private GridData gridData;

        [SerializeField] private TutorialData tutorialData;
        
        public TutorialContext GetTutorial(int index)
        {
            return tutorialData.GetTutorial(index);
        }
        
        public int CountTutorial => tutorialData.CountTutorial;
        
        public AudioData AudioData => audioData;

        public int CountLevel => gridData.CountLevel;
        
        public TextAsset GetGridTextData(int index)
        {
            return gridData.GetGridTextData(index);
        }

        public GridSurface GetGridSurface(PoolType poolType)
        {
            return gridData.GetGridSurface(poolType);
        }

        public GridUnit GetGridUnit(PoolType poolType)
        {
            return gridData.GetGridUnit(poolType);
        }
    }
}
