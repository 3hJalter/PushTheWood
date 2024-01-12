using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using _Game._Scripts.InGame;
using _Game.Data;
using _Game.DesignPattern;
using _Game.GameGrid;
using _Game.GameGrid.GridSurface;
using _Game.GameGrid.Unit;
using _Game.GameGrid.Unit.DynamicUnit.Player;
using _Game.Managers;
using _Game.Utilities;
using GameGridEnum;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
using VinhLB;
using Random = UnityEngine.Random;

public class GridMapDataGenerator : MonoBehaviour
{
    [Title("Level Data")]
    [Tooltip("The distance between the corner cells and the surfaceUnit closest to it")] 
    [SerializeField] private int offsetSurfaceWithFirstCell = 3; // Should be 3
    [InlineButton("SaveLevelAsJson","Save Level")]
    [SerializeField] private string mapLevelName = "Lvl_0";
    
    [Title("Load Level")]
    [InlineButton("LoadLevels", "Load")]
    [InlineButton("UnLoadLevels", "UnLoad")]
    [SerializeField] private int index;
    private Level _loadedLevel;

    [Title("Level Map Container")] 
    [InlineButton("TestHintTrail", "Run")]
    [SerializeField]
    private TestHintLineInEditMode hintLineTrail;
    [SerializeField]
    private string hintLineString = "";
    [InlineButton("AddNewHintLineObjectList", "New"), PropertyTooltip("Use the position data from hintLineObjList to create hint lint string" +
                                                                      "\nNew: Add a new hint line string" +
                                                                      "\nAdd: Add a new string to the current hint line string" +
                                                                      "\nClear: Destroy and Clear all hint Line Object")]
    [InlineButton("AddHintLineObjectList", "Add")]
    [InlineButton("DestroyAllHintLine", "Clear")]
    [PropertySpace(SpaceBefore = 5, SpaceAfter = 10)]
    [SerializeField] private List<Transform> hintLineObj;

    [FoldoutGroup("Container")]
    [InfoBox("Container store where units in map are drawn.")]
    [SerializeField] private Transform surfaceContainer;
    [FoldoutGroup("Container")]
    [SerializeField] private Transform unitContainer;
    [FoldoutGroup("Container")]
    [SerializeField] private Transform shadowContainer;
    [FoldoutGroup("Container")]
    [SerializeField] private Transform hintLineContainer;
   
    [FoldoutGroup("Grid Utilities")]
    [HorizontalGroup("Grid Utilities/Horizontal")]
    [Button]
    [BoxGroup("Grid Utilities/Horizontal/1")]
    private void DestroyAll()
    {
        DestroyAllUnit();
        DestroyAllSurface();
        DestroyAllShadow();
        _loadedLevel = null;
    }
    [Button]
    [BoxGroup("Grid Utilities/Horizontal/1")]
    private void SetObjectToContainer()
    {
        // Find object with GridSurface and not in surfaceContainer, then set parent to surfaceContainer
        GridSurface[] gridSurfaces = FindObjectsOfType<GridSurface>();
        foreach (GridSurface gridSurface in gridSurfaces)
        {
            if (gridSurface.Tf.parent != surfaceContainer) gridSurface.Tf.parent = surfaceContainer;
        }
        // Find object with GridUnit and not in unitContainer or shadowContainer, then set parent to unitContainer
        GridUnit[] gridUnits = FindObjectsOfType<GridUnit>();
        foreach (GridUnit gridUnit in gridUnits)
        {
            if (gridUnit.Tf.parent != unitContainer && gridUnit.Tf.parent != shadowContainer) gridUnit.Tf.parent = unitContainer;
        }
    }
    
    [BoxGroup("Grid Utilities/Horizontal/1")]
    [Button]
    private void DestroyAllHintLine()
    {
        foreach (Transform child in hintLineContainer)
        {
            DestroyImmediate(child.gameObject);
        }
        hintLineObj.Clear();
    }
    
    [BoxGroup("Grid Utilities/Horizontal/2")]
    [Button]
    private void DestroyAllShadow()
    {
        GridUnit[] gridUnits = shadowContainer.GetComponentsInChildren<GridUnit>();
        foreach (GridUnit gridUnit in gridUnits) DestroyImmediate(gridUnit.gameObject);
    }
    
    [BoxGroup("Grid Utilities/Horizontal/2")]
    [Button]
    private void DestroyAllUnit()
    {
        GridUnit[] gridUnits = unitContainer.GetComponentsInChildren<GridUnit>();
        foreach (GridUnit gridUnit in gridUnits) DestroyImmediate(gridUnit.gameObject);
    }
    
    [BoxGroup("Grid Utilities/Horizontal/2")]
    [Button]
    private void DestroyAllSurface()
    {
        GridSurface[] gridSurfaces = surfaceContainer.GetComponentsInChildren<GridSurface>();
        foreach (GridSurface gridSurface in gridSurfaces) DestroyImmediate(gridSurface.gameObject);
    }

    private void AddHintLineObjectList()
    {
        // if hintLineString is not empty, add a ";"
        if (hintLineString != "") hintLineString += " ; ";
        int countChild = hintLineContainer.childCount;
        for (int i = 0; i < countChild; i++)
        {
            hintLineObj.Add(hintLineContainer.GetChild(i));
            // store the x and z pos of this obj
            hintLineString += hintLineObj[i].position.x + " " + hintLineObj[i].position.z;
            // if not last, add a ";"
            if (i != countChild - 1) hintLineString += " ; ";
        }
    }
    
    private void AddNewHintLineObjectList()
    {
        // add all object in hintLintContainer to hintLineObj
        hintLineObj.Clear();
        hintLineString = "";
        int countChild = hintLineContainer.childCount;
        for (int i = 0; i < countChild; i++)
        {
            hintLineObj.Add(hintLineContainer.GetChild(i));
            // store the x and z pos of this obj
            hintLineString += hintLineObj[i].position.x + " " + hintLineObj[i].position.z;
            // if not last, add a ";"
            if (i != countChild - 1) hintLineString += " ; ";
        }
    }
    
    private void LoadLevels()
    {
        if (_loadedLevel != null) UnLoadLevels();
        // Create a new empty object
        GameObject levelObject = new("Level " + (index));
        _loadedLevel = new Level(index, levelObject.transform);
        // Spawn the Player to the level
        // Player player = SimplePool.Spawn<Player>(DataManager.Ins.GetGridUnit(PoolType.Player));
        // player.OnSetPositionAndRotation(PredictUnitPos(player, _loadedLevel.firstPlayerInitCell), _loadedLevel.firstPlayerDirection);
        // Set all GridSurface to surfaceContainer
        GridSurface[] gridSurfaces = FindObjectsOfType<GridSurface>();
        foreach (GridSurface gridSurface in gridSurfaces)
        {
            if (gridSurface.Tf.parent != surfaceContainer) gridSurface.Tf.parent = surfaceContainer;
        }
        // Set all GridUnit from _loadedLevel.LevelUnitData.unit to unitContainer
        foreach (GridUnit unit in _loadedLevel.UnitDataList.Select(levelUnitData => levelUnitData.unit).Where(unit => unit.Tf.parent != unitContainer))
        {
            unit.Tf.parent = unitContainer;
        }
        // Set all GridUnit from _loadedLevel.ShadowUnitList to shadowContainer
        if (_loadedLevel.ShadowUnitList.Count <= 0)
        {
            // Destroy the empty object
            DestroyImmediate(levelObject);
            return;
        }
        _loadedLevel.ShadowUnitList[0].SetAlphaTransparency(0.5f);
        foreach (GridUnit shadowUnit in _loadedLevel.ShadowUnitList.Where(shadowUnit => shadowUnit.Tf.parent != shadowContainer))
        {
            shadowUnit.Tf.parent = shadowContainer;
            shadowUnit.gameObject.SetActive(true);
        }
        // Destroy the empty object
        DestroyImmediate(levelObject);
    }
    
    private void UnLoadLevels()
    {
        SetObjectToContainer();
        DestroyAll();
        _loadedLevel = null;
    }
    
     private void Setup()
    {
        SetObjectToContainer();

        #region verify

        switch (mapLevelName)
        {
            case null:
            case " ":
            case "":
                return;
        }

        GridSurface[] gridSurfaces = surfaceContainer.GetComponentsInChildren<GridSurface>();
        if (gridSurfaces.Length == 0)
        {
            Debug.LogError("Grid must have at least 1 surface, and all unit must have on a surface");
            return;
        }
        GridUnit[] gridUnits = unitContainer.GetComponentsInChildren<GridUnit>();
        
        // // check name convention
        // if (!mapLevelName.Contains("Lvl_"))
        // {
        //     Debug.LogError("Map name must be Lvl_0, Lvl_1, Lvl_2, ...");
        //     return;
        // }
        // // Get the number of map
        // string[] split = mapLevelName.Split('_');
        // if (split.Length != 2)
        // {
        //     Debug.LogError("Map name must be Lvl_0, Lvl_1, Lvl_2, ...");
        //     return;
        // }
        // if (!int.TryParse(split[1], out int mapNumber))
        // {
        //     Debug.LogError("Map name must be Lvl_0, Lvl_1, Lvl_2, ...");
        //     return;
        // }
        // // Check if the map number is < 0 or > DataManager.Ins.CountLevel
        // if (mapNumber <= 0 || mapNumber > DataManager.Ins.CountLevel + 1)
        // {
        //     Debug.LogError("Map number must be > 0 and <= " + (DataManager.Ins.CountLevel + 1));
        //     return;
        // }
        #endregion

        // #region Get Previous Level Data
        //
        // int previousLevelXSpawnPos = 0;
        // int previousLevelYSpawnPos = 0;
        // Vector2Int previousLevelSize = Vector2Int.zero;
        // // Get previous level data
        // if (mapNumber > 1)
        // {
        //     TextGridData previousLevelData = GameGridDataHandler.CreateGridData(mapNumber - 2);
        //     // Get GridPositionData
        //     string[] splitGridPositionData = previousLevelData.GridPositionData.Split(' ');
        //     // Get the X (the first value)
        //     if (!int.TryParse(splitGridPositionData[0], out previousLevelXSpawnPos))
        //     {
        //         Debug.LogError("Error when get previous level data");
        //         return;
        //     }
        //     // Get the Y (the second value)
        //     if (!int.TryParse(splitGridPositionData[1], out previousLevelYSpawnPos))
        //     {
        //         Debug.LogError("Error when get previous level data");
        //         return;
        //     }
        //     // Get the size of previous level
        //     previousLevelSize = previousLevelData.GetSize();
        // }
        //
        // #endregion
        
        #region Set up GridMap
        
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

        // float xSpawnPos = 0f;
        // float ySpawnPos = 0f;
        // // if (mapNumber > 1)
        // // {
        // //     ySpawnPos = previousLevelYSpawnPos + previousLevelSize.y * Constants.CELL_SIZE + offsetS * Constants.CELL_SIZE;
        // //     if (mapNumber % 2 == 0)
        // //         // xSpawnPos = previousLevelXSpawnPos + previousLevelSize.x * Constants.CELL_SIZE - offsetS;
        // //         xSpawnPos = 10;
        // //     else
        // //         // xSpawnPos = previousLevelXSpawnPos - maxX * Constants.CELL_SIZE + offsetS;
        // //         xSpawnPos = 0;
        // // }
        // Vector2 gridMapPosition = new(xSpawnPos, ySpawnPos);
        // file.WriteLine(gridMapPosition.x + " " + gridMapPosition.y);
        // // Write a @ to separate
        // file.WriteLine("@");
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

        #region Set shadow Unit (for hint or smt)
        
        // Reset the array to all -1
        // Get all units in shadowContainer
        GridUnit[] shadowUnits = shadowContainer.GetComponentsInChildren<GridUnit>();
        // Take data to shadowUnitData
        RawLevelData.ShadowUnitData[] shadowUnitDataList = new RawLevelData.ShadowUnitData[shadowUnits.Length];
        // Write a @ to separate
        file.WriteLine("@");
        for (int i = 0; i < shadowUnits.Length; i++)
        {
           // Check if unit is down
            shadowUnitDataList[i].rA = shadowUnits[i].Tf.eulerAngles;
            // if rotation round int of x and z divided by 180, then it is up
            bool isUp = Mathf.RoundToInt(shadowUnitDataList[i].rA.x) % 180 == 0 
                        && Mathf.RoundToInt(shadowUnitDataList[i].rA.z) % 180 == 0;
            
            shadowUnitDataList[i].p = shadowUnits[i].Tf.position;
            if (!isUp) 
            {
                shadowUnitDataList[i].p = shadowUnits[i].Tf.position;
                
                // if y position is floating number (not near integer with 0.05f), then it is done already, and we dont do anything
                // but if the y position is near integer with 0.05f, then we need to add the offset
                if (Mathf.Abs(shadowUnitDataList[i].p.y - Mathf.RoundToInt(shadowUnitDataList[i].p.y)) < 0.05f)
                {
                    shadowUnitDataList[i].p.y -= shadowUnits[i].yOffsetOnDown;
                }
            }
            
            shadowUnitDataList[i].t = (int)shadowUnits[i].PoolType;
            // Save as format x, y, z, rotationAngleX, rotationAngleY, rotationAngleZ, type
            // \nx, y, z, rotationAngleX, rotationAngleY, rotationAngleZ, type, ...
            string saveLine = shadowUnitDataList[i].p.x + " " + shadowUnitDataList[i].p.y + " " +
                        shadowUnitDataList[i].p.z + " "
                        + shadowUnitDataList[i].rA.x + " " + shadowUnitDataList[i].rA.y + " " +
                        shadowUnitDataList[i].rA.z + " "
                        + shadowUnitDataList[i].t;
            file.WriteLine(saveLine);     
        }
        #endregion
        
        #region Set hint line
       
        // Write a @ to separate
        file.WriteLine("@");
        // Save hint line
        // Check if hintLineString has other than number, space and ;
        if (hintLineString.Any(c => !char.IsDigit(c) && c != ' ' && c != ';'))
        {
            Debug.LogError("Hint line string must be number, space and ;\nSave file still success but no hint line is generated");
            file.WriteLine("");
        }
        else file.WriteLine(hintLineString);
        
        #endregion
        file.Close();
        
        // check if gridData contain this textAsset with this name, if not, add it 
        if (DataManager.Ins.HasGridTextData(Resources.Load<TextAsset>(mapLevelName))) return;
        // Add new txt file to gridData from DataManager from path 
        DataManager.Ins.AddGridTextData(Resources.Load<TextAsset>(mapLevelName));
    }

     [ContextMenu("Save JSON")]
    private void SaveLevelAsJson()
    {
        SetObjectToContainer();

        #region Verify

        // Verify name
        switch (mapLevelName)
        {
            case null:
            case " ":
            case "":
                Debug.LogError("Level name can not be empty");
                return;
        }
        // Verify Grid Surface
        GridSurface[] gridSurfaces = surfaceContainer.GetComponentsInChildren<GridSurface>();
        if (gridSurfaces.Length == 0)
        {
            Debug.LogError("Grid must have at least 1 surface");
            return;
        }
        // Verify Grid Unit (Player has in this level)
        GridUnit[] gridUnits = unitContainer.GetComponentsInChildren<GridUnit>();
        // Check if have Player
        if (gridUnits.All(gridUnit => gridUnit.PoolType != PoolType.Player))
        {
            Debug.LogError("Grid must have at least 1 Player");
            return;
        }

        GridUnit[] shadowUnits = shadowContainer.GetComponentsInChildren<GridUnit>();
        
        #endregion
        
        #region Setup GridMapSize
        
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

        #region Set MinX and MaxX

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

        #endregion

        #region Set MinZ and MaxZ

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
        
        // Set gridMapSize

        Vector2Int size = new(maxX, maxZ);
        
        #endregion
        
        #region Setup GridSurfaceData
        
        // Create a list of GridSurfaceData
        List<RawLevelData.GridSurfaceData> gridSurfaceDataList = new();
        
        // Loop all gridSurface 
        foreach (GridSurface gridSurface in gridSurfaces)
        {
            // Save the position of gridSurface
            Vector3 position = gridSurface.Tf.position;
            // Get the x and z index of gridSurface
            int x = (int)(position.x - 1) / 2;
            int z = (int)(position.z - 1) / 2;
            // Save the data to gridSurfaceData
            RawLevelData.GridSurfaceData gridSurfaceData = new()
            {
                p = new Vector2Int(x, z),
                t = (int)gridSurface.PoolType,
                d = (int)BuildingUnitData.GetDirection(gridSurface.Tf.eulerAngles.y),
                // save the ground material if it is ground surface
                m = gridSurface is GroundSurface groundSurface ? (int)groundSurface.groundMaterialEnum : -1
            };
            // Add gridSurfaceData to gridSurfaceDataList
            gridSurfaceDataList.Add(gridSurfaceData);
        }
        
        #endregion

        #region Setup GridUnitData

        // Create a list of GridUnitData
        List<RawLevelData.GridUnitData> gridUnitDataList = new();
        
        // Loop all gridUnit
        
        foreach (GridUnit gridUnit in gridUnits)
        {
            // Save the position of gridUnit
            Vector3 position = gridUnit.Tf.position;
            // Get the x and z index of gridUnit
            int x = (int)(position.x - 1) / 2;
            int z = (int)(position.z - 1) / 2;
            // Save the data to gridUnitData
            RawLevelData.GridUnitData gridUnitData = new()
            {
                c = new Vector2Int(x, z),
                t = (int)gridUnit.PoolType,
                // rotationDirection = (int)gridUnit.SkinRotationDirection
                // TEST
                d = (int)BuildingUnitData.GetDirection(gridUnit.Tf.eulerAngles.y),
            };
            // Add gridUnitData to gridUnitDataList
            gridUnitDataList.Add(gridUnitData);
        }

        #endregion

        #region Setup ShadowUnitData
        
        // Create a list of ShadowUnitData
        List<RawLevelData.ShadowUnitData> shadowUnitDataList = new();

        // Loop all shadowUnit
        foreach (GridUnit shadowUnit in shadowUnits)
        {
            // Check if unit is down
            Vector3 rotationAngle = shadowUnit.Tf.eulerAngles;
            // if rotation round int of x and z divided by 180, then it is up
            bool isUp = Mathf.RoundToInt(rotationAngle.x) % 180 == 0 
                        && Mathf.RoundToInt(rotationAngle.z) % 180 == 0;
            // if not up, add the offset
            if (!isUp)
            {
                // if y position is floating number (not near integer with 0.05f), then it is done already, and we dont do anything
                // but if the y position is near integer with 0.05f, then we need to add the offset
                Vector3 position = shadowUnit.Tf.position;
                if (Mathf.Abs(position.y - Mathf.RoundToInt(position.y)) < 0.05f)
                {
                    position.y -= shadowUnit.yOffsetOnDown;
                    // Round the y position to 2 decimal
                    position.y = (float)Math.Round(position.y, 2);
                }
            }
            
            // Save the data to shadowUnitData
            RawLevelData.ShadowUnitData shadowUnitData = new()
            {
                p = shadowUnit.Tf.position,
                rA = rotationAngle,
                t = (int)shadowUnit.PoolType
            };
            // Add shadowUnitData to shadowUnitDataList
            shadowUnitDataList.Add(shadowUnitData);
        }
        
        #endregion
        
        #region Setup HintTrailData
        
        // Create a list of HintTrailData
        List<RawLevelData.HintTrailData> hintTrailDataList = new();

        bool isNotVerify = hintLineString.Any(c => !char.IsDigit(c) && c != ' ' && c != ';');
        
        // Check the hintLineString
        if (!isNotVerify)
        {
            // separate the hintLineString to each Vector3(x,3f,z)
            string[] splitHintLineString = hintLineString.Split(" ; ");
            // if hintLineString is empty, return
            if (splitHintLineString.Length == 0) return;
            foreach (string s in splitHintLineString)
            {
                string[] split = s.Split(' ');
                if (split.Length != 2) continue;
                if (!float.TryParse(split[0], out float x)) continue;
                if (!float.TryParse(split[1], out float z)) continue;
                RawLevelData.HintTrailData hintTrailData = new()
                {
                    p = new Vector2(x, z)
                };
                hintTrailDataList.Add(hintTrailData);
            }
        }
        
        #endregion
        
        RawLevelData levelData = new()
        {
            s = size,
            sfD = gridSurfaceDataList.ToArray(),
            uD = gridUnitDataList.ToArray(),
            suD = shadowUnitDataList.ToArray(),
            htD = hintTrailDataList.ToArray()
        };
        // Convert levelData to json
        DevLog.Log(DevId.Hoang, "Save levelData: " + levelData.sfD.Length);
        string json = JsonUtility.ToJson(levelData);
        // Format json
        DevLog.Log(DevId.Hoang, "Save json: " + json);
        // Save json to file
        string path = "Assets/_Game/Resources/Level/" + mapLevelName + ".json";
        DevLog.Log(DevId.Hoang, "Save to path: " + path);
        File.WriteAllText(path, json);
        
        // check if gridData contain this textAsset with this name, if not, add it 
        TextAsset textAsset = Resources.Load<TextAsset>("Level/" + mapLevelName);
        if (DataManager.Ins.HasGridTextData(textAsset)) return;
        // Add new txt file to gridData from DataManager from path 
        DataManager.Ins.AddGridTextData(textAsset);
    }
     
    private void TestHintTrail()
    {
        // separate the hintLineString to each Vector3(x,3f,z)
        string[] splitHintLineString = hintLineString.Split(" ; ");
        // if hintLineString is empty, return
        if (splitHintLineString.Length == 0 || hintLineTrail == null) return;
        List<Vector3> hintLinePosList = new();
        foreach (string s in splitHintLineString)
        {
            string[] split = s.Split(' ');
            if (split.Length != 2) continue;
            if (!float.TryParse(split[0], out float x)) continue;
            if (!float.TryParse(split[1], out float z)) continue;
            hintLinePosList.Add(new Vector3(x, 3f, z));
        }
        // Make the hintLineTrail follow the hintLinePosList
        hintLineTrail.TestHintMoving(hintLinePosList);
    }
     
    private Vector3 PredictUnitPos(GridUnit unit, GameGridCell cell)
    {
        float offsetY = (float)HeightLevel.One / 2 * Constants.CELL_SIZE;
        if (unit.UnitTypeY == UnitTypeY.Down) offsetY -= unit.yOffsetOnDown;
        return cell.WorldPos + Vector3.up * offsetY;
    }
}
