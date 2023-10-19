using System;
using System.Collections.Generic;
using GameGridEnum;
using UnityEngine;

public static class Constants
{
    public const int CELL_SIZE = 2;

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

    public const float TREE_HEIGHT = 1f;

    public const float MOVING_TIME = 0.25f;
    public const float MOVING_LOG_TIME = 0.01f;

    public static readonly Dictionary<GridSurfaceType, HeightLevel> dirFirstHeightOfSurface = new()
    {
        { GridSurfaceType.Ground, HeightLevel.One },
        { GridSurfaceType.Water, HeightLevel.Zero }
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

    public static readonly Dictionary<Direction, Vector3Int> dirRotate = new()
    {
        { Direction.None, Vector3Int.zero },
        { Direction.Left, new Vector3Int(0, 0, 90) },
        { Direction.Right, new Vector3Int(0, 0, -90) },
        { Direction.Forward, new Vector3Int(90, 0, 0) },
        { Direction.Back, new Vector3Int(-90, 0, 0) }
    };

    // BUG: Seem like the changing is not correct
    public static readonly Dictionary<Direction, Action<Transform>> dirRotateAction = new()
    {
        { Direction.Left, skin => { skin.Rotate(0, 0, 90); } },
        { Direction.Right, skin => { skin.Rotate(0, 0, -90); } },
        { Direction.Forward, skin => { skin.Rotate(90, 0, 0); } },
        { Direction.Back, skin => { skin.Rotate(-90, 0, 0); } }
    };
}


namespace _Game
{
    public interface IInit
    {
        public void OnInit();
    }
}
