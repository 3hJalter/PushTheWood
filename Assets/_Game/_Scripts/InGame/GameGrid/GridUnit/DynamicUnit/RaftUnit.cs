﻿using System.Collections.Generic;
using DG.Tweening;
using GameGridEnum;
using UnityEngine;

namespace _Game.GameGrid.GridUnit.DynamicUnit
{
    public class RaftUnit : GridUnitDynamic, IVehicle
    {
        private HashSet<GridUnitDynamic> _carryUnits = new();
        
        public void OnInit(GameGridCell mainCellIn, ChumpType type, HeightLevel startHeightIn = HeightLevel.ZeroPointFive)
        {
            base.OnInit(mainCellIn, startHeightIn);
            RotateSkin(type);
        }

        // Handle Moving Logic
        public void OnMove(Direction direction)
        {
            if (direction == Direction.None) return;
            if (!HasNoObstacleIfMove(direction, out GameGridCell nextMainCell,
                    out HashSet<GameGridCell> nextCells, out HashSet<GridUnit> nextUnits))
            {
                OnNotMove(direction, nextUnits, this);
                return;
            }
            SetAllCarryUnitAsChild();
            Vector3 newPosition = GetUnitNextWorldPos(nextMainCell);
            Tf.DOMove(newPosition, Constants.MOVING_TIME).SetEase(Ease.Linear).OnComplete(() =>
            {
                OnOutCurrentCells();
                OnEnterNextCellWithoutFall(nextMainCell, nextCells);
                // Get next mainCell and cellInUnits of all carry units, do the same
                foreach (GridUnitDynamic unit in _carryUnits)
                {
                    GameGridCell nextMainCellIn = GridUnitFunc.GetNextMainCell(unit, direction);
                    HashSet<GameGridCell> nextCellsIn = GridUnitFunc.GetNextCells(unit, direction);
                    unit.OnOutCurrentCells();
                    unit.OnEnterNextCellWithoutFall(nextMainCellIn, nextCellsIn);
                }
                ReleaseAllCarryUnit();
                OnMove(direction);
            });
        }

        private void SetAllCarryUnitAsChild()
        {
            _carryUnits = GetAllAboveUnit();
            foreach (GridUnitDynamic unit in _carryUnits)
            {
                unit.transform.SetParent(transform);
                unit.isInAction = true;
            }
        }
        
        private void ReleaseAllCarryUnit()
        {
            foreach (GridUnitDynamic unit in _carryUnits)
            {
                unit.transform.SetParent(null);
                unit.isInAction = false;
            }
            _carryUnits.Clear();
        }
        
        private void RotateSkin(ChumpType type)
        {
            skin.localRotation =
                Quaternion.Euler(type is ChumpType.Horizontal ? Constants.horizontalSkinRotation : Constants.verticalSkinRotation);
            if (type is ChumpType.Vertical) size = new Vector3Int(size.z, size.y, size.x);
        }
    }
}
