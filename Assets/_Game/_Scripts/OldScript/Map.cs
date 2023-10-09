using _Game._Scripts.InGame.Player;
using _Game._Scripts.Utilities.Grid;
using MapEnum;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Game
{
    public class Map : HMonoBehaviour
    {
        [SerializeField] private GridPlane gridPlaneType;

        [SerializeField] private Vector2Int mapSize;

        [SerializeField] private float cellSize;

        [SerializeField] private Player player;

        public Player Player => player;

        private Grid<GameCell, GameCellData>.DebugGrid debug;
        
        public Grid<GameCell, GameCellData> GridMap { get; private set; }

        public void OnInit()
        {
            GridMap = new Grid<GameCell, GameCellData>(mapSize.x, mapSize.y, cellSize, Tf.position,
                () => new GameCell(), gridPlaneType);
            debug = new Grid<GameCell, GameCellData>.DebugGrid();
            debug.DrawGrid(GridMap);
        }
        
        // [ContextMenu("")]
    }
}
