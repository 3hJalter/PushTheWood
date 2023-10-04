public static class Constants
{
    public enum Plane
    {
        XY = 0,
        XZ = 1,
        YZ = 2
    }

    public const float TREE_HEIGHT = 1f;
    public const float MOVING_TIME = 0.25f;
    public const float MOVING_LOG_TIME = 0.01f;
}

namespace Game
{
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

    public class GameCellData
    {
        public CellState state = CellState.None;
        public CellType type = CellType.Water;
    }

    public interface IInit
    {
        public void OnInit();
    }
}
