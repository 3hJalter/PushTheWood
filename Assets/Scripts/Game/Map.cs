using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities.AI;

namespace Game
{
    public class Map : MonoBehaviour
    {
        [SerializeField]
        CONSTANTS.PLANE planeType;
        [SerializeField]
        Vector2Int mapSize;
        [SerializeField]
        float cellSize;

        Grid<GameCell, GameCellData> grid;
        Grid<GameCell, GameCellData>.DebugGrid debug;

        private Mesh mesh;
        private Obstance[] obstances;
        public Grid<GameCell, GameCellData> GridMap => grid;
        void Awake()
        {
            grid = new Grid<GameCell, GameCellData>(mapSize.x, mapSize.y, cellSize, transform.position, () => new GameCell(), planeType);
            debug = new Grid<GameCell, GameCellData>.DebugGrid();
            obstances = FindObjectsOfType<Obstance>();

            mesh = new Mesh();
            GetComponent<MeshFilter>().mesh = mesh;
            debug.DrawGrid(grid);
        }
        // Update is called once per frame
        public void UpdateVisual()
        {
            debug.UpdateVisualMap(grid, mesh);
        }
    }
}