using _Game._Scripts.DesignPattern;

namespace MapEnum
{
    public enum GridPlane
    {
        XY = 0,
        XZ = 1,
        YZ = 2
    }
    
    public enum CellType
    {
        None = 0,
        Ground = 1,
        Water = 2
    }

    public enum CellState
    {
        None = 0,
        Player = 1,
        TreeObstacle = 3,
        LowRockObstacle = 4,
        HighRockObstacle = 5
    }

    public enum TreeType
    {
        Horizontal = 0,
        Vertical = 1
    }

    public enum TreeState
    {
        Up = 0,
        Down = 1
    }
}
public enum Direction
{
    None = -1,
    Left = 0,
    Right = 1,
    Up = 2,
    Down = 3,
}

namespace TweenTypeEnum
{
    public enum EasingType
    {
        Linear = 1,
        InSine,
        OutSine,
        InOutSine,
        InQuad,
        OutQuad,
        InOutQuad,
        InCubic,
        OutCubic,
        InOutCubic,
        InQuart,
        OutQuart,
        InOutQuart,
        InQuint,
        OutQuint,
        InOutQuint,
        InExpo,
        OutExpo,
        InOutExpo,
        InCirc,
        OutCirc,
        InOutCirc,
        InElastic,
        OutElastic,
        InOutElastic,
        InBack,
        OutBack,
        InOutBack,
        InBounce,
        OutBounce,
        InOutBounce,
        Flash,
        InFlash,
        OutFlash,
        InOutFlash,
        Custom
    }
}

namespace GameGridEnum
{
    public enum HeightLevel
    {
        None = -1,
        Zero = 0,
        One = 1,
        Two = 2,
    }
    public enum GridSurfaceType
    {
        // Default value of all GameGridCell is Water, so no need to create a WaterGridSurface
        // Other GridSurfaceType must be created when needed (e.g. GroundGridSurface) to overwrite the default value
        Water = PoolType.SurfaceWater, 
        Ground = PoolType.SurfaceGround,
    }
    public enum GridUnitStaticType
    {
        None = -1,
        RockShort = 0,
        RockHigh = 1,
        TreeRoot = 2,
        TreeShort = 3,
        TreeHigh = 4,
        BridgeShort = 5,
        BridgeHigh = 6,
    }
    public enum GridUnitDynamicType
    {
        None = -1,
        Player = 0,
        ChumpShort = 1,
        ChumpHigh = 2,
        Raft = 3,
    }
    
}

