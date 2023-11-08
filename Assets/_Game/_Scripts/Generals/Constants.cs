using System;
using System.Collections.Generic;
using GameGridEnum;
using UnityEngine;

public static class Constants
{
    public const int CELL_SIZE = 2;

    // String Constants
    
    /// <summary>
    ///     Player Prefab Path
    /// </summary>
    public const string LEVEL_INDEX = "levelIndex";
    
    /// <summary>
    ///     Path to the folder in Resources
    /// </summary>
    public const string POOL_PATH = "Pool";
    public const string UI_PATH = "UI/";

    /// <summary>
    ///     Control input axis
    /// </summary>
    public const string HORIZONTAL = "Horizontal";
    public const string VERTICAL = "Vertical";
    public const float INPUT_THRESHOLD_P2 = 0.16f; // Threshold Power 2 from 0 to 1
    
    /// <summary>
    ///     Player Animation
    /// </summary>
    public const string INIT_ANIM = " ";

    public const string IDLE_ANIM = "idle";
    public const string WALK_ANIM = "walk";

    /// <summary>
    ///     UI Animation Trigger
    /// </summary>
    public const string OPEN = "Open";
    public const string CLOSE = "Close";

    
    // Value Constants
    public const int UPPER_HEIGHT = 1;
    public const int BELOW_HEIGHT = 1;

    public const float DELAY_INTERACT_TIME = 0.25f;
    public const float MOVING_TIME = 0.25f;
    public const float FALLING_TIME = 0.2f;
    
    public static readonly HeightLevel maxHeight = (HeightLevel)(Enum.GetValues(typeof(HeightLevel)).Length - 1);
    public const HeightLevel MIN_HEIGHT = HeightLevel.Zero;
    public static readonly Vector3 horizontalSkinRotation = new(0, 0, 90);
    public static readonly Vector3 verticalSkinRotation = new(0, -90, 90);
    
    // OLD 
    public const float TREE_HEIGHT = 1f;
    public const float MOVING_TIME2 = 0.25f;
    public const float MOVING_LOG_TIME = 0.01f;
    //
    
    
    // Dictionary constants
    
    /// <summary>
    ///  Dictionary
    /// </summary>
    public static readonly Dictionary<GridSurfaceType, HeightLevel> dirFirstHeightOfSurface = new()
    {
        { GridSurfaceType.Ground, HeightLevel.One },
        { GridSurfaceType.Water, HeightLevel.ZeroPointFive }
    };

    public static readonly Dictionary<Direction, Vector3> dirVector3F = new()
    {
        { Direction.None, Vector3.zero },
        { Direction.Left, Vector3.left },
        { Direction.Right, Vector3.right },
        { Direction.Forward, Vector3.forward },
        { Direction.Back, Vector3.back }
    };
    
    public static readonly Dictionary<Direction, Vector3Int> dirVector3 = new()
    {
        { Direction.None, Vector3Int.zero },
        { Direction.Left, Vector3Int.left },
        { Direction.Right, Vector3Int.right },
        { Direction.Forward, Vector3Int.forward },
        { Direction.Back, Vector3Int.back }
    };

    public static readonly Dictionary<Direction, Vector2Int> dirVector = new()
    {
        { Direction.None, Vector2Int.zero },
        { Direction.Left, Vector2Int.left },
        { Direction.Right, Vector2Int.right },
        { Direction.Forward, Vector2Int.up },
        { Direction.Back, Vector2Int.down }
    };
}


namespace _Game
{
    public interface IInit
    {
        public void OnInit();
    }
}
