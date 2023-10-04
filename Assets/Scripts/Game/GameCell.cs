using Utilities.Grid;

namespace Game
{
    public class GameCell : GridCell<GameCellData>
    {
        private int islandID = -1;
        public Player Player;
        public Chump Tree1;
        public Chump Tree2;

        public GameCell()
        {
            value = new GameCellData();
        }

        public GameCell(GameCellData data)
        {
            value = data;
        }

        public GameCell(GameCell copy) : base(copy)
        {
            value = new GameCellData();
            value.type = copy.Value.type;
            value.state = copy.Value.state;

            islandID = copy.islandID;
            Tree1 = copy.Tree1;
            Tree2 = copy.Tree2;
            Player = copy.Player;

        }

        public bool IsBlockingRollingTree =>
            value.state == CellState.LowRockObstacle
            || value.state == CellState.HighRockObstacle;

        public bool IsRaft => Tree1 != null && Tree2 != null;

        public bool IsBlockingPlayer =>
            (value.type == CellType.Water && !IsRaft)
            || value.state == CellState.HighRockObstacle;

        public bool IsCanPushRaft =>
            value.state == CellState.HighRockObstacle
            || (Tree1 != null && value.type == CellType.Ground);

        public int IslandID
        {
            get => islandID;
            set
            {
                if (value < 0 || islandID >= 0) return;
                if (this.value.type == CellType.Ground) islandID = value;
            }
        }
    }
}
