using _Game._Scripts.Managers;
using UnityEngine;

namespace _Game.GameGrid
{
    public static class GameGridDataHandler
    {
        private enum GridDataType
        {
            SurfaceData = 0,
            UnitData = 1
        }

        public static TextGridData CreateGridData(int mapIndex)
        {
            TextAsset textAsset = DataManager.Ins.GetGridTextData(mapIndex);
            string[] splitData = textAsset.text.Split('@');
            return new TextGridData(splitData[(int) GridDataType.SurfaceData], 
                splitData[(int) GridDataType.UnitData]);
        }
    }

    public class TextGridData
    {
        public TextGridData(string surfaceData, string unitData)
        {
            SurfaceData = surfaceData;
            UnitData = unitData;
        }

        public string SurfaceData { get; }

        public string UnitData { get; }
    }
}
