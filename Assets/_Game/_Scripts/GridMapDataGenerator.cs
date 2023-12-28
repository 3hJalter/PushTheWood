using System;
using System.Collections.Generic;
using System.IO;
using _Game._Scripts.InGame;
using _Game.Data;
using _Game.GameGrid.GridSurface;
using _Game.GameGrid.Unit;
using _Game.Managers;
using UnityEngine;
using VinhLB;
using Random = UnityEngine.Random;

public class GridMapDataGenerator : MonoBehaviour
{
    [Header("Map Data")]
    [SerializeField] private string mapLevelName = "Lvl_0";
    [SerializeField] private int offsetSurfaceWithFirstCell = 3;
    [SerializeField] private Vector2 gridMapPosition = Vector2.zero;

    [Header("Map Container")]
    [SerializeField] private Transform surfaceContainer;
    [SerializeField] private Transform unitContainer;

    
    [Header("Load Level")]
    [SerializeField] private int loadLevelIndexStart;
    [SerializeField] private int loadLevelIndexEnd = 1;
    
    private List<Level> _loadedLevel;

    // Use this when some unit or surface Unloaded is null, then remove all other by hand
    [ContextMenu("Clear Loaded Level list to null")] 
    private void ClearLoadedLevel()
    {
        _loadedLevel = null;
    }

    [ContextMenu("Load Level")]
    private void LoadLevels()
    {
        if (_loadedLevel != null) UnLoadLevels();
        _loadedLevel = new List<Level>();
        for (int i = loadLevelIndexStart; i < loadLevelIndexEnd; i++)
        {
            Level level = new(i);
            _loadedLevel.Add(level);
        }
    }

    [ContextMenu("Unload Level")]
    private void UnLoadLevels()
    {
        for (int i = _loadedLevel.Count - 1; i >= 0; i--)
        {
            _loadedLevel[i].OnDeSpawnLevel(false);
            _loadedLevel.RemoveAt(i);
        }

        _loadedLevel = null;
    }

    [ContextMenu("Set All GroundSurface to Ground Parent and GroundUnit to Unit Parent")]
    private void SetSurfaceAndUnitToParent()
    {
        GridSurface[] gridSurfaces = FindObjectsOfType<GridSurface>();
        foreach (GridSurface gridSurface in gridSurfaces) gridSurface.Tf.parent = surfaceContainer;
        GridUnit[] gridUnits = FindObjectsOfType<GridUnit>();
        foreach (GridUnit gridUnit in gridUnits) gridUnit.Tf.parent = unitContainer;
    }

    [ContextMenu("Save Data as txt file")]
    private void Setup()
    {

        #region verify

        switch (mapLevelName)
        {
            case null:
            case " ":
            case "":
                return;
        }

        GridSurface[] gridSurfaces = FindObjectsOfType<GridSurface>();
        if (gridSurfaces.Length == 0)
        {
            Debug.LogError("Grid must have at least 1 surface, and all unit must have on a surface");
            return;
        }

        #endregion

        #region Set up GridMap

        GridUnit[] gridUnits = FindObjectsOfType<GridUnit>();
        int minX = int.MaxValue;
        int minZ = int.MaxValue;
        int maxX = int.MinValue;
        int maxZ = int.MinValue;
        foreach (GridSurface gridSurface in gridSurfaces)
        {
            Vector3 position = gridSurface.Tf.position;
            if (position.x < minX) minX = (int)Math.Round(position.x);
            if (position.z < minZ) minZ = (int)Math.Round(position.z);
            if (position.x > maxX) maxX = (int)Math.Round(position.x);
            if (position.z > maxZ) maxZ = (int)Math.Round(position.z);
        }

        int offsetS = offsetSurfaceWithFirstCell * 2 + 1;
        // if minX or minZ < offsetS, get the offset and make all position added with this offset so the minX and minY can be offsetS (index 5,5)`
        if (minX < offsetS)
        {
            int offsetX = offsetS - minX;
            foreach (GridSurface gridSurface in gridSurfaces)
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
        else if (minX > offsetS)
        {
            int offsetX = minX - offsetS;
            foreach (GridSurface gridSurface in gridSurfaces)
            {
                Vector3 position = gridSurface.Tf.position;
                position.x -= offsetX;
                gridSurface.Tf.position = position;
            }

            foreach (GridUnit gridUnit in gridUnits)
            {
                Vector3 position = gridUnit.Tf.position;
                position.x -= offsetX;
                gridUnit.Tf.position = position;
            }

            minX -= offsetX;
            maxX -= offsetX;
        }

        maxX += offsetS - 1; // add five more cell to maxX and maxY
        if (minZ < offsetS)
        {
            int offsetZ = offsetS - minZ;
            foreach (GridSurface gridSurface in gridSurfaces)
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
        else if (minZ > offsetS)
        {
            int offsetZ = minZ - offsetS;
            foreach (GridSurface gridSurface in gridSurfaces)
            {
                Vector3 position = gridSurface.Tf.position;
                position.z -= offsetZ;
                gridSurface.Tf.position = position;
            }

            foreach (GridUnit gridUnit in gridUnits)
            {
                Vector3 position = gridUnit.Tf.position;
                position.z -= offsetZ;
                gridUnit.Tf.position = position;
            }

            minZ -= offsetZ;
            maxZ -= offsetZ;
        }

        maxZ += offsetS - 1;
        const int cellOffset = 1;
        maxX = (maxX + cellOffset) / 2;
        maxZ = (maxZ + cellOffset) / 2;

        #endregion

        #region Create Txt File

        string path = "Assets/_Game/Resources/" + mapLevelName + ".txt";
        File.WriteAllText(path, string.Empty);
        using StreamWriter file = new(path, true);

        #endregion

        #region Save map position

        file.WriteLine(gridMapPosition.x + " " + gridMapPosition.y);

        #endregion

        #region Init Grid Surface

        // create 2 dimension int array with default value is 0
        int[,] gridData = new int[maxX, maxZ];
        // fill the array with 0
        for (int i = 0; i < maxX; i++)
        for (int j = 0; j < maxZ; j++)
            gridData[i, j] = 0;
        // Get position of each gridSurface and set the value of gridSurfaceData to 1
        foreach (GridSurface gridSurface in gridSurfaces)
        {
            Vector3 position = gridSurface.Tf.position;
            int x = (int)(position.x + 1) / 2;
            int z = (int)(position.z + 1) / 2;
            gridData[x - 1, z - 1] = (int)gridSurface.PoolType;
        }

        // Save the array as txt file in Resources folder
        // Write a @ to separate
        file.WriteLine("@");
        for (int i = 0; i < maxX; i++)
        {
            string line = "";
            for (int j = 0; j < maxZ; j++) line += gridData[i, j] + " ";
            line = line.Remove(line.Length - 1);
            file.WriteLine(line);
        }

        Debug.Log("Save surface: Complete");

        #endregion

        #region Rotate Grid Surface

        // Reset the array to all -1
        for (int i = 0; i < maxX; i++)
        for (int j = 0; j < maxZ; j++)
            gridData[i, j] = -1;
        // Handle gridSurfaceRotationDirection
        foreach (GridSurface gridSurface in gridSurfaces)
        {
            Vector3 position = gridSurface.Tf.position;
            int x = (int)(position.x + 1) / 2;
            int z = (int)(position.z + 1) / 2;
            gridData[x - 1, z - 1] = (int)BuildingUnitData.GetDirection(gridSurface.Tf.eulerAngles.y);
        }

        // Save the array as txt file in Resources folder
        // Write a @ to separate
        file.WriteLine("@");
        for (int i = 0; i < maxX; i++)
        {
            string line = "";
            for (int j = 0; j < maxZ; j++) line += gridData[i, j] + " ";
            line = line.Remove(line.Length - 1);
            file.WriteLine(line);
        }

        #endregion

        #region Set Ground Surface Color

        // Save Surface GroundMaterial 
        // Reset the array to all -1
        for (int i = 0; i < maxX; i++)
        for (int j = 0; j < maxZ; j++)
            gridData[i, j] = -1;
        // Handle GridSurface GroundMaterial
        foreach (GridSurface gridSurface in gridSurfaces)
        {
            if (gridSurface is not GroundSurface groundSurface) continue;
            Vector3 position = groundSurface.Tf.position;
            int x = (int)(position.x + 1) / 2;
            int z = (int)(position.z + 1) / 2;
            int colorIndex = (int)groundSurface.groundMaterialEnum;
            // if color index is -1, set it to random color
            if (colorIndex == -1)
            {
                colorIndex = Random.Range(0, DataManager.Ins.CountSurfaceMaterial - 1);
                groundSurface.groundMaterialEnum = (MaterialEnum)colorIndex;
                groundSurface.SetMaterialToGround();
            }

            gridData[x - 1, z - 1] = (int)groundSurface.groundMaterialEnum;
        }

        // Save the array as txt file in Resources folder
        // Write a @ to separate
        file.WriteLine("@");
        for (int i = 0; i < maxX; i++)
        {
            string line = "";
            for (int j = 0; j < maxZ; j++) line += gridData[i, j] + " ";
            line = line.Remove(line.Length - 1);
            file.WriteLine(line);
        }

        #endregion

        #region Init Grid Unit

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

            gridData[x - 1, z - 1] = (int)gridUnit.PoolType;
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

        Debug.Log("Save unit: Complete");

        #endregion

        #region Set Grid Unit Rotation

        // Reset the array to all -1
        for (int i = 0; i < maxX; i++)
        for (int j = 0; j < maxZ; j++)
            gridData[i, j] = -1;
        // Handle gridUnitRotationDirection
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

            gridData[x - 1, z - 1] = (int)gridUnit.SkinRotationDirection;
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

        Debug.Log("Save unit rotation: Complete");

        #endregion

        file.Close();

    }
}
