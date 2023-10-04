using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utilities.AI
{
    public class AStar : Grid<NodeCell, int>.PathfindingAlgorithm
    {
        private const int MOVE_STRAIGHT_COST = 10;
        private const int MOVE_DIAGONAL_COST = 14;

        private List<NodeCell> openList;
        private List<NodeCell> closeList;

        public AStar()
        {

            openList = new List<NodeCell>();
            closeList = new List<NodeCell>();
        }

        public override List<NodeCell> FindPath(int startX, int startY, int endX, int endY, Grid<NodeCell, int> grid)
        {
            this.grid = grid;
            openList.Clear();
            closeList.Clear();
            NodeCell startNode = grid.GetGridCell(startX, startY);
            NodeCell endNode = grid.GetGridCell(endX, endY);
            openList.Add(startNode);

            for (int x = 0; x < grid.Width; x++)
            {
                for (int y = 0; y < grid.Height; y++)
                {
                    NodeCell cell = grid.GetGridCell(x, y);
                    cell.GCost = int.MaxValue;
                    cell.CalculateFCost();
                    cell.Parent = null;
                }
            }

            startNode.GCost = 0;
            startNode.HCost = CalculateDistanceCost(startNode, endNode);
            startNode.CalculateFCost();

            while (openList.Count > 0)
            {
                NodeCell currentNode = GetLowestFCostCell(openList);
                if (currentNode == endNode)
                {
                    //NOTE: Reach
                    return CalculatePath(endNode);
                }

                openList.Remove(currentNode);
                closeList.Add(currentNode);

                foreach (NodeCell neighbourNode in GetNeighbourNode(currentNode))
                {
                    if (closeList.Contains(neighbourNode))
                        continue;
                    if (!neighbourNode.IsWalkable)
                    {
                        closeList.Add(neighbourNode);
                        continue;
                    }

                    int tentativeGCost = currentNode.GCost + CalculateDistanceCost(currentNode, neighbourNode);
                    if (tentativeGCost < neighbourNode.GCost)
                    {
                        neighbourNode.Parent = currentNode;
                        neighbourNode.GCost = tentativeGCost;
                        neighbourNode.HCost = CalculateDistanceCost(neighbourNode, endNode);
                        neighbourNode.CalculateFCost();
                    }

                    if (!openList.Contains(neighbourNode))
                    {
                        openList.Add(neighbourNode);
                    }
                }
            }

            return null;
        }
        public List<NodeCell> GetNeighbourNode(NodeCell currentNode)
        {
            List<NodeCell> neighbourNodes = new List<NodeCell>();

            if (currentNode.X - 1 >= 0)
            {
                neighbourNodes.Add(grid.GetGridCell(currentNode.X - 1, currentNode.Y));

                if (currentNode.Y - 1 >= 0) neighbourNodes.Add(grid.GetGridCell(currentNode.X - 1, currentNode.Y - 1));
                if (currentNode.Y + 1 < grid.Height) neighbourNodes.Add(grid.GetGridCell(currentNode.X - 1, currentNode.Y + 1));
            }

            if (currentNode.X + 1 < grid.Width)
            {
                neighbourNodes.Add(grid.GetGridCell(currentNode.X + 1, currentNode.Y));

                if (currentNode.Y - 1 >= 0) neighbourNodes.Add(grid.GetGridCell(currentNode.X + 1, currentNode.Y - 1));
                if (currentNode.Y + 1 < grid.Height) neighbourNodes.Add(grid.GetGridCell(currentNode.X + 1, currentNode.Y + 1));
            }

            if (currentNode.Y - 1 >= 0) neighbourNodes.Add(grid.GetGridCell(currentNode.X, currentNode.Y - 1));
            if (currentNode.Y + 1 < grid.Height) neighbourNodes.Add(grid.GetGridCell(currentNode.X, currentNode.Y + 1));

            return neighbourNodes;
        }
        private List<NodeCell> CalculatePath(NodeCell endNode)
        {
            List<NodeCell> path = new List<NodeCell>();
            path.Add(endNode);
            NodeCell currentNode = endNode;
            while (currentNode.Parent != null)
            {
                path.Add(currentNode.Parent);
                currentNode = currentNode.Parent;
            }
            path.Reverse();
            return path;
        }
        private int CalculateDistanceCost(NodeCell a, NodeCell b)
        {
            int xDistance = Mathf.Abs(a.X - b.X);
            int yDistance = Mathf.Abs(a.Y - b.Y);
            int remaining = Mathf.Abs(xDistance - yDistance);
            return MOVE_DIAGONAL_COST * Mathf.Min(xDistance, yDistance) + MOVE_STRAIGHT_COST * remaining;
        }
        private NodeCell GetLowestFCostCell(List<NodeCell> nodeCellList)
        {
            NodeCell lowestFCostNode = nodeCellList[0];
            for (int i = 1; i < nodeCellList.Count; i++)
            {
                if (nodeCellList[i].FCost < lowestFCostNode.FCost)
                {
                    lowestFCostNode = nodeCellList[i];
                }
            }
            return lowestFCostNode;
        }
    }
}