﻿using System.Collections.Generic;
using System.Linq;
using _Game.DesignPattern;
using _Game.GameGrid;
using _Game.GameGrid.Unit;
using _Game.Managers;
using _Game.Utilities;
using DG.Tweening;
using UnityEngine;

namespace _Game._Scripts.InGame
{
    public class Island
    {
        private readonly HashSet<GridUnit> _gridUnits = new();

        private readonly Dictionary<GameGridCell, PoolType> _initGridUnitDic = new();
        private readonly int _islandID;
        public HashSet<GridUnit> GridUnits => _gridUnits;

        public Island(int islandID)
        {
            _islandID = islandID;
        }

        public List<GameGridCell> GridCells { get; } = new();

        public GameGridCell FirstPlayerStepCell { get; private set; }

        public Vector3 centerIslandPos;
        public Vector3 minXIslandPos;
        public Vector3 maxXIslandPos;
        public Vector3 minZIslandPos;
        public Vector3 maxZIslandPos;

        public Vector2Int islandSize;
        public bool isSmallIsland;

        public void SetIslandPos()
        {
            centerIslandPos = Vector3.zero;
            minXIslandPos = maxXIslandPos = minZIslandPos = maxZIslandPos = GridCells[0].WorldPos;
            for (int i = 0; i < GridCells.Count; i++)
            {
                centerIslandPos += GridCells[i].WorldPos;
                if (GridCells[i].WorldPos.x < minXIslandPos.x)
                    minXIslandPos = GridCells[i].WorldPos;
                if (GridCells[i].WorldPos.x > maxXIslandPos.x)
                    maxXIslandPos = GridCells[i].WorldPos;
                if (GridCells[i].WorldPos.z < minZIslandPos.z)
                    minZIslandPos = GridCells[i].WorldPos;
                if (GridCells[i].WorldPos.z > maxZIslandPos.z)
                    maxZIslandPos = GridCells[i].WorldPos;
            }
            centerIslandPos /= GridCells.Count;
            // Island size x  = (maxX - minX) / Constants.CELL_SIZE + 1
            // Island size z  = (maxZ - minZ) / Constants.CELL_SIZE + 1
            islandSize = new Vector2Int(
                (int) ((maxXIslandPos.x - minXIslandPos.x) / Constants.CELL_SIZE + 1),
                (int) ((maxZIslandPos.z - minZIslandPos.z) / Constants.CELL_SIZE + 1));
            isSmallIsland = islandSize.x < Constants.MAX_SMALL_ISLAND_SIZE;
            // Debug
            DevLog.Log(DevId.Hoang, "Island " + _islandID + " center pos: " + centerIslandPos);
            DevLog.Log(DevId.Hoang, "Island " + _islandID + " minX pos: " + minXIslandPos);
            DevLog.Log(DevId.Hoang, "Island " + _islandID + " maxX pos: " + maxXIslandPos);
            DevLog.Log(DevId.Hoang, "Island " + _islandID + " minZ pos: " + minZIslandPos);
            DevLog.Log(DevId.Hoang, "Island " + _islandID + " maxZ pos: " + maxZIslandPos);
            DevLog.Log(DevId.Hoang, "Island " + _islandID + " size: " + islandSize);
            //
        }
        
        
            
        public void SetFirstPlayerStepCell(GameGridCell cell)
        {
            FirstPlayerStepCell ??= cell;
        }

        public void AddGridCell(GameGridCell cell)
        {
            GridCells.Add(cell);
        }

        public void AddInitUnitToIsland(GridUnit unit, PoolType type, GameGridCell cell)
        {
            _gridUnits.Add(unit);
            _initGridUnitDic.Add(cell, type);
        }

        public void AddNewUnitToIsland(GridUnit unit)
        {
            _gridUnits.Add(unit);
        }

        public void ClearIsland()
        {
            for (int i = 0; i < GridCells.Count; i++)
            {
                GameGridCell cell = GridCells[i];
                cell.ClearGridUnit();
            }

            foreach (GridUnit unit in _gridUnits.Where(unit => unit.gameObject.activeSelf))
            {
                if (unit.islandID != _islandID) continue;
                unit.OnDespawn();
            }

            _gridUnits.Clear();
        }

        public void ResetIsland()
        {
            DOTween.KillAll();
            ClearIsland();
            foreach (KeyValuePair<GameGridCell, PoolType> pair in _initGridUnitDic)
            {
                GridUnit unit =
                    SimplePool.Spawn<GridUnit>(DataManager.Ins.GetGridUnit(pair.Value));
                unit.OnInit(pair.Key);
                AddNewUnitToIsland(unit);
            }
        }
    }
}