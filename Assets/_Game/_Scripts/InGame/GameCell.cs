using _Game._Scripts.InGame.Player;
using _Game._Scripts.Utilities.Grid;
using MapEnum;

namespace _Game
{
    public class GameCell : GridCell<GameCellData>
    {
        private int islandID = -1;
        public Player Player;
        public Chump Tree1;
        public Chump Tree2;

        public GameCell()
        {
            data = new GameCellData();
        }

        public bool IsBlockingRollingTree =>
            data.state == CellState.LowRockObstacle
            || data.state == CellState.HighRockObstacle;

        public bool IsRaft => Tree1 != null && Tree2 != null;

        public bool IsBlockingPlayer =>
            (data.type == CellType.Water && !IsRaft)
            || data.state == CellState.HighRockObstacle;

        public bool IsCanPushRaft =>
            data.state == CellState.HighRockObstacle
            || (Tree1 != null && data.type == CellType.Ground);

        public int IslandID
        {
            get => islandID;
            set
            {
                if (value < 0 || islandID >= 0) return;
                if (this.data.type == CellType.Ground) islandID = value;
            }
        }
    }
     
    public class GameCellData
    {
        public CellState state = CellState.None;
        public CellType type = CellType.Water;
    }
}
