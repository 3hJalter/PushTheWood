using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project
{
    using Utilities.AI;
    public class Map : MonoBehaviour
    {
        // Start is called before the first frame update
        Grid<NodeCell, int>.PathfindingAlgorithm pathfinding;
        Grid<NodeCell, int> grid;
        Grid<NodeCell, int>.DebugGrid debug;

        private Mesh mesh;
        private Obstance[] obstances;
        public Grid<NodeCell, int> MapGrid => grid;
        void Awake()
        {
            pathfinding = new AStar();
            grid = new Grid<NodeCell, int>(10, 10, 5, Vector2.one * -5 * 5, () => new NodeCell());
            debug = new Grid<NodeCell, int>.DebugGrid();
            obstances = FindObjectsOfType<Obstance>();
            UpdateObstance();

            mesh = new Mesh();
            GetComponent<MeshFilter>().mesh = mesh;
            debug.DrawGrid(grid);
            debug.UpdateVisualMap(grid, mesh);
        }
        // Update is called once per frame
        private void Update()
        {
            if (Input.GetMouseButtonDown(1))
            {
                Vector3 mousePos = GridUtilities.GetMouseWorldPosition();
                (int x, int y) = grid.GetGridPosition(mousePos);

                NodeCell cell = grid.GetGridCell(x, y);
                cell.IsWalkable = !cell.IsWalkable;
                debug.UpdateVisualMap(grid, mesh);
            }
        }

        public void DrawPath(List<NodeCell> path)
        {
            debug.DrawPath(path);
        }

        private void UpdateObstance()
        {
            foreach(Obstance obstance in obstances)
            {
                foreach(Vector2 point in obstance.PointExists)
                {
                    grid.GetGridCell(point).IsWalkable = false;
                }
            }
        }
    }
}