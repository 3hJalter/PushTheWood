using System;
using System.IO;
using _Game.DesignPattern;
using _Game.GameGrid;
using _Game.GameGrid.GridSurface;
using _Game.GameGrid.GridUnit;
using _Game.GameGrid.GridUnit.DynamicUnit;
using _Game.Managers;
using _Game.Utilities.Grid;
using GameGridEnum;
using MapEnum;
using UnityEngine;

public class GridMapDataGenerator : MonoBehaviour
{
    [SerializeField] private int gridSizeX;
    [SerializeField] private int gridSizeY;
    private Grid<GameGridCell, GameGridCellData>.DebugGrid _debugGrid;
    private Grid<GameGridCell, GameGridCellData> _gridMap;

    private GridSurfaceBase[,] _gridSurfaceMap;

    // Test Init GridUnit
    private PlayerUnit _pUnit;
    private TextGridData _textGridData;
    private void Start()
    {
        GenerateMap();
    }

    private void GenerateMap()
    {
        // Get gridData.txt from Assets/_Game/Resources/gridData.txt
        TextAsset gridData = Resources.Load<TextAsset>("gridData");
        // Split the text into two parts: surfaceData and unitData by @
        string[] gridDataSplit = gridData.text.Split('@');
        // Split surfaceData into lines
        string[] surfaceData = gridDataSplit[0].Split('\n');
        // remove the last line
        Array.Resize(ref surfaceData, surfaceData.Length - 1);
        // Split unitData into lines
        string[] unitData = gridDataSplit[1].Split('\n');
        // remove the first line
        Array.Copy(unitData, 1, unitData, 0, unitData.Length - 1);
        // remove the last line
        Array.Resize(ref unitData, unitData.Length - 1);
        // Get the maximum x and z position of gridSurface
        gridSizeX = surfaceData.Length;
        gridSizeY = surfaceData[0].Split(' ').Length;
        _gridSurfaceMap = new GridSurfaceBase[gridSizeX, gridSizeY];
        // Create GridMap
        _gridMap = new Grid<GameGridCell, GameGridCellData>(gridSizeX, gridSizeY, Constants.CELL_SIZE, transform.position,
            () => new GameGridCell(), GridPlane.XZ);
        _debugGrid = new Grid<GameGridCell, GameGridCellData>.DebugGrid();
        _debugGrid.DrawGrid(_gridMap);
        // Spawn GridSurface
        SpawnGridSurfaceToGrid(surfaceData);
        // Spawn GridUnit
        SpawnGridUnitToGrid(unitData);
    }

    private void SpawnGridUnitToGrid(string[] unitData)
    {
        
    }

    private void SpawnGridSurfaceToGrid(string[] surfaceData)
    {
        
        for (int x = 0; x < gridSizeX; x++)
        {
            string[] surfaceDataSplit = surfaceData[x].Split(' ');
            for (int y = 0; y < gridSizeY; y++)
            {
                if (!int.TryParse(surfaceDataSplit[y], out int cell)) continue;
                if (!Enum.IsDefined(typeof(GridSurfaceType), cell)) continue;
                GridSurfaceBase gridSurface = DataManager.Ins.GetGridSurface((GridSurfaceType)cell);
                if (gridSurface is null) continue;
                GameGridCell gridCell = _gridMap.GetGridCell(x, y);
                gridCell.SetSurface(
                    SimplePool.Spawn<GridSurfaceBase>(gridSurface,
                        new Vector3(gridCell.WorldX, 0, gridCell.WorldY), Quaternion.identity));
                _gridSurfaceMap[x, y] = gridCell.GetData().gridSurface;
            }
        }
    }

    [ContextMenu("Save Data as txt file")]
    private void Setup()
    {
        Debug.Log("Setup");
        GroundSurface[] gridSurfaces = FindObjectsOfType<GroundSurface>();
        GridUnit[] gridUnits = FindObjectsOfType<GridUnit>();
        // Handle gridSurface
        // Get the maximum x and z position of gridSurface
        int maxX = 0;
        int maxZ = 0;
        const int cellOffset = 1;
        foreach (GroundSurface gridSurface in gridSurfaces)
        {
            if (gridSurface.Tf.position.x > maxX) maxX = (int)gridSurface.Tf.position.x;
            if (gridSurface.Tf.position.z > maxZ) maxZ = (int)gridSurface.Tf.position.z;
        }

        maxX = (maxX + cellOffset) / 2;
        maxZ = (maxZ + cellOffset) / 2;
        // create 2 dimension int array with default value is 0
        int[,] gridData = new int[maxX, maxZ];
        // fill the array with 0
        for (int i = 0; i < maxX; i++)
        for (int j = 0; j < maxZ; j++)
            gridData[i, j] = 0;
        // Get position of each gridSurface and set the value of gridSurfaceData to 1
        foreach (GroundSurface gridSurface in gridSurfaces)
        {
            Vector3 position = gridSurface.Tf.position;
            int x = (int)(position.x + 1) / 2;
            int z = (int)(position.z + 1) / 2;
            gridData[x - 1, z - 1] = (int)gridSurface.PoolType;
        }

        // Save the array as txt file in Resources folder
        const string path = "Assets/_Game/Resources/gridData.txt";
        File.WriteAllText(path, string.Empty);
        using StreamWriter file = new(path, true);
        for (int i = 0; i < maxX; i++)
        {
            string line = "";
            for (int j = 0; j < maxZ; j++) line += gridData[i, j] + " ";
            file.WriteLine(line);
        }

        Debug.Log("Save surface: Complete");
        // Reset the array to all 0
        for (int i = 0; i < maxX; i++)
        for (int j = 0; j < maxZ; j++)
            gridData[i, j] = 0;
        // Handle gridUnit
        // Set position of gritUnit to gridData
        foreach (GridUnit gridUnit in gridUnits)
        {
            Vector3 position = gridUnit.Tf.position;
            int x = (int)(position.x + 1) / 2;
            int z = (int)(position.z + 1) / 2;
            gridData[x - 1, z - 1] = gridUnit switch
            {
                GridUnitDynamic { PoolType: not null } gridUnitDynamic => (int)gridUnitDynamic.PoolType,
                GridUnitStatic { PoolType: not null } gridUnitStatic => (int)gridUnitStatic.PoolType,
                _ => gridData[x - 1, z - 1]
            };
        }

        // Write a @ to separate
        file.WriteLine("@");
        // Save the array as txt file in Resources folder
        for (int i = 0; i < maxX; i++)
        {
            string line = "";
            for (int j = 0; j < maxZ; j++) line += gridData[i, j] + " ";
            file.WriteLine(line);
        }

        Debug.Log("Save unit: Complete");
    }
}
