using System;
using _Game.Managers;
using UnityEngine;

namespace _Game.GameGrid
{
    public static class GameGridDataHandler
    {
        public static TextGridData CreateGridData(int mapIndex)
        {
            TextAsset textAsset = DataManager.Ins.GetGridTextData(mapIndex);
            string[] splitData = textAsset.text.Split('@');
            return new TextGridData(
                splitData[(int)GridDataType.SurfaceData],
                splitData[(int)GridDataType.UnitData],
                splitData[(int)GridDataType.RotationDirectionUnitData]);
        }

        public static TextGridData CreateGridData2(TextAsset textAsset)
        {
            string[] splitData = textAsset.text.Split('@');
            return new TextGridData(
                splitData[(int)GridDataType.SurfaceData],
                splitData[(int)GridDataType.UnitData],
                splitData[(int)GridDataType.RotationDirectionUnitData]);
        }

        private enum GridDataType
        {
            SurfaceData = 0,
            UnitData = 1,
            RotationDirectionUnitData = 2
        }
    }

    public class TextGridData
    {
        public TextGridData(string surfaceData, string unitData, string unitRotationDirectionData)
        {
            SurfaceData = surfaceData;
            UnitData = unitData;
            UnitRotationDirectionData = unitRotationDirectionData;
        }

        public string SurfaceData { get; }
        public string UnitData { get; }
        public string UnitRotationDirectionData { get; }

        public Vector2Int GetSize()
        {
            string[] surfaceData = SurfaceData.Split('\n');
            int x = surfaceData.Length;
            int y = surfaceData[0].Split(' ').Length;
            return new Vector2Int(x, y);
        }
    }
}