using _Game.GameGrid;
using _Game.GameGrid.GridUnit;
using GameGridEnum;
using UnityEngine;

namespace _Game.GameRule.RuleEngine
{
    public class RuleRollingData : RuleMovingData
    {
        public Vector3Int nextSize;
        public HeightLevel nextEndHeight;
        
        public RuleRollingData(GridUnit runRuleUnit) : base(runRuleUnit)
        {
        }
        
        public override void SetData(Direction direction)
        {
            runDirection = direction;
            nextCells.Clear();
            blockUnits.Clear();
            nextSize = runRuleUnit.Size;
            nextEndHeight = runRuleUnit.EndHeight;
            SetNextMainCell();
            if (nextMainCell is null) return;
            int xAxisLoop = direction is Direction.Left or Direction.Right
                ? runRuleUnit.Size.y
                : runRuleUnit.Size.x;
            int zAxisLoop = direction is Direction.Left or Direction.Right
                ? runRuleUnit.Size.z
                : runRuleUnit.Size.y;
            Vector2Int nexMainCellPos = nextMainCell.GetCellPosition();
            nextSize = GridUnitFunc.RotateSize(direction, runRuleUnit.Size);
            nextEndHeight = runRuleUnit.CalculateEndHeight(runRuleUnit.StartHeight, nextSize);
            HeightLevel endHeightForChecking = nextEndHeight > runRuleUnit.EndHeight ? nextEndHeight : runRuleUnit.EndHeight;
            for (int i = 0; i < xAxisLoop; i++)
            for (int j = 0; j < zAxisLoop; j++)
            {
                Vector2Int cellPos = nexMainCellPos + new Vector2Int(i, j);
                GameGridCell cell = LevelManager.Ins.GetCell(cellPos);
                if (cell is null)
                {
                    continue;
                }
                for (HeightLevel k = runRuleUnit.StartHeight; k <= endHeightForChecking; k++)
                {
                    GridUnit unit = cell.GetGridUnitAtHeight(k);
                    if (unit is null || unit == runRuleUnit) continue;
                    blockUnits.Add(unit);
                }
                nextCells.Add(cell);
            }
            return;
            void SetNextMainCell()
            {
                Vector2Int nextMainCellPos = runRuleUnit.MainCell.GetCellPosition() + GetOffset();
                nextMainCell = LevelManager.Ins.GetCell(nextMainCellPos);
                return;
                Vector2Int GetOffset()
                {
                    switch (direction)
                    {
                        case Direction.Left:
                            return new Vector2Int(-runRuleUnit.Size.y, 0);
                        case Direction.Right:
                            return new Vector2Int(runRuleUnit.Size.x, 0);
                        case Direction.Forward:
                            return new Vector2Int(0, runRuleUnit.Size.z);
                        case Direction.Back:
                            return new Vector2Int(0, -runRuleUnit.Size.y);
                        case Direction.None:
                        default:
                            return Vector2Int.zero;
                    }
                }
            }
        }
        
    }
}
