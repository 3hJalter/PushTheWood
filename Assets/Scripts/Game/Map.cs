using UnityEngine;
using Utilities.Grid;

namespace Game
{
    public class Map : MonoBehaviour
    {
        [SerializeField] private Constants.Plane planeType;

        [SerializeField] private Vector2Int mapSize;

        [SerializeField] private float cellSize;

        private Grid<GameCell, GameCellData>.DebugGrid debug;

        private Mesh mesh;
        public Grid<GameCell, GameCellData> GridMap { get; private set; }

        private void Awake()
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
