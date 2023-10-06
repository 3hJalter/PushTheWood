namespace MapEnum
{
    public enum Plane
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
namespace AudioEnum
{
    public enum SfxType
    {
        None = -1,
    }
    
    public enum BgmType
    {
        None = -1,
        MainMenu = 0,
    }
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


