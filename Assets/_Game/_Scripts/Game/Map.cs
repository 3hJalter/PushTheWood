using DesignPattern.Grid;
using UnityEngine;
using Plane = MapEnum.Plane;

namespace _Game
{
    public class Map : MonoBehaviour
    {
        [SerializeField] private Plane planeType;

        [SerializeField] private Vector2Int mapSize;

        [SerializeField] private float cellSize;

        [SerializeField] private Player player;

        public Player Player => player;

        private Grid<GameCell, GameCellData>.DebugGrid debug;

        private Mesh mesh;
        public Grid<GameCell, GameCellData> GridMap { get; private set; }

        public void OnInit()
        {
            GridMap = new Grid<GameCell, GameCellData>(mapSize.x, mapSize.y, cellSize, transform.position,
                () => new GameCell(), planeType);
            debug = new Grid<GameCell, GameCellData>.DebugGrid();

            mesh = new Mesh();
            GetComponent<MeshFilter>().mesh = mesh;
            debug.DrawGrid(GridMap);
        }
        
        // Update is called once per frame
        public void UpdateVisual()
        {
            debug.UpdateVisualMap(GridMap, mesh);
        }
    }
}
