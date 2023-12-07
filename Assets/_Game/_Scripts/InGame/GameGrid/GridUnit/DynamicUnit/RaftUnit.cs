using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using GameGridEnum;
using UnityEngine;

namespace _Game.GameGrid.Unit.DynamicUnit
{
    public class RaftUnit : GridUnitDynamic, IVehicle
    {
        private HashSet<GridUnitDynamic> _carryUnits = new();
        
        public void OnInit(GameGridCell mainCellIn, UnitType type, HeightLevel startHeightIn = HeightLevel.ZeroPointFive)
        {
            RotateSkin(type);
            unitState = UnitState.Down;
            base.OnInit(mainCellIn, startHeightIn, false);
        }

        public override void OnDespawn()
        {
            if (_carryUnits.Count > 0) ReleaseAllCarryUnit();
            base.OnDespawn();
        }

        // Handle Moving Logic
        public void OnMove(Direction direction)
        {
            if (isInAction) return;
            SetAllCarryUnitAsChild();
            if (direction == Direction.None) return;
            if (HasObstacleIfMove(direction, out GameGridCell nextMainCell,
                    out HashSet<GameGridCell> nextCells, out HashSet<GridUnit> nextUnits))
            {
                ReleaseAllCarryUnit();
                OnNotMove(direction, nextUnits, this);
                return;
            }
            if (nextCells.Any(cell => cell.SurfaceType is not GridSurfaceType.Water))
            {
                ReleaseAllCarryUnit();
                return;
            }
            isInAction = true;
            Vector3 newPosition = GetUnitWorldPos(nextMainCell);
            Tf.DOMove(newPosition, Constants.MOVING_TIME).SetEase(Ease.Linear).SetUpdate(UpdateType.Fixed).OnComplete(() =>
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
                isInAction = false;
                // ReleaseAllCarryUnit();
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
                unit.isLockedAction = true;
            }
        }
        
        private void ReleaseAllCarryUnit()
        {
            foreach (GridUnitDynamic unit in _carryUnits)
            {
                unit.transform.SetParent(null);
                unit.isInAction = false;
                unit.isLockedAction = false;
            }
            _carryUnits.Clear();
        }

        private UnitType _lastSpawnType = UnitType.None;
        private bool _isFirstSpawnDone;
        private void RotateSkin(UnitType type)
        {
            skin.localRotation =
                Quaternion.Euler(type is UnitType.Horizontal ? Constants.HorizontalSkinRotation : Constants.VerticalSkinRotation);
            switch (_isFirstSpawnDone)
            {
                case false when type is UnitType.Vertical:
                    size = new Vector3Int(size.z, size.y, size.x);
                    _isFirstSpawnDone = true;
                    break;
                case true when _lastSpawnType != type:
                    size = new Vector3Int(size.z, size.y, size.x);
                    break;
            }
            _lastSpawnType = type;
        }
    }
}
