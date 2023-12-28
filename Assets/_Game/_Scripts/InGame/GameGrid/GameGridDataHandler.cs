using System.Linq;
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
                splitData[(int)GridDataType.GridPositionData],
                splitData[(int)GridDataType.SurfaceData],
                splitData[(int)GridDataType.SurfaceRotationDirectionData],
                splitData[(int)GridDataType.SurfaceMaterialData],
                splitData[(int)GridDataType.UnitData],
                splitData[(int)GridDataType.RotationDirectionUnitData]);
        }

        public static TextGridData CreateGridData2(TextAsset textAsset)
        {
            string[] splitData = textAsset.text.Split('@');
            return new TextGridData(
                splitData[(int)GridDataType.GridPositionData],
                splitData[(int)GridDataType.SurfaceData],
                splitData[(int)GridDataType.SurfaceRotationDirectionData],
                splitData[(int)GridDataType.SurfaceMaterialData],
                splitData[(int)GridDataType.UnitData],
                splitData[(int)GridDataType.RotationDirectionUnitData]);
        }

        private enum GridDataType
        {
            GridPositionData = 0,
            SurfaceData = 1,
            SurfaceRotationDirectionData = 2,
            SurfaceMaterialData = 3,
            UnitData = 4,
            RotationDirectionUnitData = 5
        }
    }

    public class TextGridData
    {
        public TextGridData(string gridPositionData, string surfaceData, string surfaceRotationDirectionData,
            string surfaceMaterialData, string unitData, string unitRotationDirectionData)
        {
            GridPositionData = gridPositionData;
            SurfaceData = surfaceData;
            SurfaceRotationDirectionData = surfaceRotationDirectionData;
            SurfaceMaterialData = surfaceMaterialData;
            UnitData = unitData;
            UnitRotationDirectionData = unitRotationDirectionData;
        }

        public string GridPositionData { get; }
        public string SurfaceData { get; }
        public string SurfaceRotationDirectionData { get; }
        public string SurfaceMaterialData { get; }
        public string UnitData { get; }
        public string UnitRotationDirectionData { get; }

        public Vector2Int GetSize(bool skipFirstLine = true)
        {
            string[] surfaceData = SurfaceData.Split('\n');
            if (skipFirstLine) surfaceData = surfaceData.Skip(1).ToArray();
            int x = surfaceData.Length;
            int y = surfaceData[0].Split(' ').Length;
            return new Vector2Int(x, y);
        }
    }
}
