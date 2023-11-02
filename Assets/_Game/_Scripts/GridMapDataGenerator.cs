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
    [SerializeField] private string mapLevelName = "level0";
    private int _gridSizeX;
    private int _gridSizeY;
    private Grid<GameGridCell, GameGridCellData>.DebugGrid _debugGrid;
    private Grid<GameGridCell, GameGridCellData> _gridMap;

    private GridSurfaceBase[,] _gridSurfaceMap;

    // Test Init GridUnit
    private PlayerUnit _pUnit;
    private TextGridData _textGridData;
    private void Start()
    {
        TextAsset gridData = Resources.Load<TextAsset>(mapLevelName);
        TextGridData textGridData = GameGridDataHandler.CreateGridData2(gridData);
        GenerateMap();
    }

    private void GenerateMap()
    {
        // Get gridData.txt from Assets/_Game/Resources/gridData.txt
        TextAsset gridData = Resources.Load<TextAsset>(mapLevelName);
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
        Array.Resize(ref unitData, unitData.Length - 2);
        // Get the maximum x and z position of gridSurface
        _gridSizeX = surfaceData.Length;
        _gridSizeY = surfaceData[0].Split(' ').Length;
        _gridSurfaceMap = new GridSurfaceBase[_gridSizeX, _gridSizeY];
        // Create GridMap
        _gridMap = new Grid<GameGridCell, GameGridCellData>(_gridSizeX, _gridSizeY, Constants.CELL_SIZE, transform.position,
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
        for (int x = 0; x < _gridSizeY; x++)
        {
            string[] unitDataSplit = unitData[x].Split(' ');
            for (int y = 0; y < _gridSizeY; y++)
            {
                if (!int.TryParse(unitDataSplit[y], out int cell)) continue;
                if (Enum.IsDefined(typeof(GridUnitDynamicType), cell))
                {
                    GridUnitDynamic gridUnitDynamic = DataManager.Ins.GetGridUnitDynamic((GridUnitDynamicType)cell);
                    if (gridUnitDynamic is null) continue;
                    GameGridCell gridCell = _gridMap.GetGridCell(x, y);
                    SimplePool.Spawn<GridUnitDynamic>(gridUnitDynamic).OnInit(gridCell);
                } else if (Enum.IsDefined(typeof(GridUnitStaticType), cell))
                {
                    GridUnitStatic gridUnitStatic = DataManager.Ins.GetGridUnitStatic((GridUnitStaticType)cell);
                    if (gridUnitStatic is null) continue;
                    GameGridCell gridCell = _gridMap.GetGridCell(x, y);
                   SimplePool.Spawn<GridUnitStatic>(gridUnitStatic).OnInit(gridCell);
                }
            }
        }
    }

    private void SpawnGridSurfaceToGrid(string[] surfaceData)
    {
        
        for (int x = 0; x < _gridSizeX; x++)
        {
            string[] surfaceDataSplit = surfaceData[x].Split(' ');
            for (int y = 0; y < _gridSizeY; y++)
            {
                if (!int.TryParse(surfaceDataSplit[y], out int cell)) continue;
                if (!Enum.IsDefined(typeof(GridSurfaceType), cell)) continue;
                GridSurfaceBase gridSurface = DataManager.Ins.GetGridSurface((GridSurfaceType)cell);
                if (gridSurface is null) continue;
                GameGridCell gridCell = _gridMap.GetGridCell(x, y);
                gridCell.SetSurface(
                    SimplePool.Spawn<GridSurfaceBase>(gridSurface,
                        new Vector3(gridCell.WorldX, 0, gridCell.WorldY), Quaternion.identity));
                _gridSurfaceMap[x, y] = gridCell.Data.gridSurface;
            }
        }
    }

    [ContextMenu("Save Data as txt file")]
    private void Setup()
    {
        switch (mapLevelName)
        {
            case null:
            case " ":
            case "":
                return;
        }
        GroundSurface[] gridSurfaces = FindObjectsOfType<GroundSurface>();
        if (gridSurfaces.Length == 0)
        {
            Debug.LogError("Grid must have at least 1 surface, and all unit must have on a surface");
            return;
        }
        GridUnit[] gridUnits = FindObjectsOfType<GridUnit>();
        int minX = int.MaxValue;
        int minZ = int.MaxValue;
        int maxX = int.MinValue;
        int maxZ = int.MinValue;
        foreach (GroundSurface gridSurface in gridSurfaces)
        {
            Vector3 position = gridSurface.Tf.position;
            if (position.x < minX) minX = (int) Math.Round(position.x);
            if (position.z < minZ) minZ = (int) Math.Round(position.z);
            if (position.x > maxX) maxX = (int) Math.Round(position.x);
            if (position.z > maxZ) maxZ = (int) Math.Round(position.z);
        }
        // if minX or minY < 11, get the offset and make all position added with this offset so the minX and minY can be 11 (index 1,1)
        if (minX < 11)
        {
            int offsetX = 11 - minX;
            foreach (GroundSurface gridSurface in gridSurfaces)
            {
                Vector3 position = gridSurface.Tf.position;
                position.x += offsetX;
                gridSurface.Tf.position = position;
            }
            foreach (GridUnit gridUnit in gridUnits)
            {
                Vector3 position = gridUnit.Tf.position;
                position.x += offsetX;
                gridUnit.Tf.position = position;
            }
            minX += offsetX;
            maxX += offsetX;
        }
        maxX += 10; // add one more cell to maxX and maxY
        if (minZ < 11)
        {
            int offsetZ = 11 - minZ;
            foreach (GroundSurface gridSurface in gridSurfaces)
            {
                Vector3 position = gridSurface.Tf.position;
                position.z += offsetZ;
                gridSurface.Tf.position = position;
            }
            foreach (GridUnit gridUnit in gridUnits)
            {
                Vector3 position = gridUnit.Tf.position;
                position.z += offsetZ;
                gridUnit.Tf.position = position;
            }
            minZ += offsetZ;
            maxZ += offsetZ;
        }
        maxZ += 10;
        const int cellOffset = 1;
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
        string path = "Assets/_Game/Resources/" + mapLevelName + ".txt";
        File.WriteAllText(path, string.Empty);
        using StreamWriter file = new(path, true);
        for (int i = 0; i < maxX; i++)
        {
            string line = "";
            for (int j = 0; j < maxZ; j++) line += gridData[i, j] + " ";
            line = line.Remove(line.Length - 1);
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
            // if x or z larger than size of array, return 
            if (x > maxX || z > maxZ)
            {
                Debug.LogError("Grid Unit must be on Grid Surface");
                // Close the file then delete it
                file.Close();
                File.Delete(path);
                // Remove the file
                return;
            }
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
            line = line.Remove(line.Length - 1);
            file.WriteLine(line);
        }
        file.Close();
        Debug.Log("Save unit: Complete");
    }
}
