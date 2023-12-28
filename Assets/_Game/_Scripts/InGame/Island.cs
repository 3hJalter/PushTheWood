using System.Collections.Generic;
using System.Linq;
using _Game.DesignPattern;
using _Game.GameGrid;
using _Game.GameGrid.Unit;
using _Game.Managers;
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
        
        public Vector3 GetCenterIslandPos()
        {
            Vector3 centerPos = Vector3.zero;
            for (int i = 0; i < GridCells.Count; i++) centerPos += GridCells[i].WorldPos;
            centerPos /= GridCells.Count;
            return centerPos;
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
