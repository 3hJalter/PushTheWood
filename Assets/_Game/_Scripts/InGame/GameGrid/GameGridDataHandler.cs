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
                splitData[(int)GridDataType.SurfaceRotationDirectionData],
                splitData[(int)GridDataType.SurfaceMaterialData],
                splitData[(int)GridDataType.UnitData],
                splitData[(int)GridDataType.RotationDirectionUnitData]);
        }

        public static TextGridData CreateGridData2(TextAsset textAsset)
        {
            string[] splitData = textAsset.text.Split('@');
            return new TextGridData(
                splitData[(int)GridDataType.SurfaceData],
                splitData[(int)GridDataType.SurfaceRotationDirectionData],
                splitData[(int)GridDataType.SurfaceMaterialData],
                splitData[(int)GridDataType.UnitData],
                splitData[(int)GridDataType.RotationDirectionUnitData]);
        }

        private enum GridDataType
        {
            SurfaceData = 0,
            SurfaceRotationDirectionData = 1,
            SurfaceMaterialData = 2,
            UnitData = 3,
            RotationDirectionUnitData = 4,
        }
    }

    public class TextGridData
    {
        public TextGridData(string surfaceData, string surfaceRotationDirectionData, string surfaceMaterialData, string unitData, string unitRotationDirectionData)
        {
            SurfaceData = surfaceData;
            SurfaceRotationDirectionData = surfaceRotationDirectionData;
            SurfaceMaterialData = surfaceMaterialData;
            UnitData = unitData;
            UnitRotationDirectionData = unitRotationDirectionData;
        }

        public string SurfaceData { get; }
        public string SurfaceRotationDirectionData { get; }
        public string SurfaceMaterialData { get; }
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
