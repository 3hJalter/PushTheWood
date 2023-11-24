using System.Collections.Generic;
using _Game.DesignPattern;
using _Game.GameGrid;
using _Game.GameGrid.GridUnit;
using GameGridEnum;

namespace _Game.GameRule.RuleEngine
{
    public class RuleMovingData : RuleEngineData
    {
        public Direction runDirection;
        public GameGridCell nextMainCell;
        public readonly HashSet<GameGridCell> nextCells;
        public readonly HashSet<GridUnit> blockUnits;

        public RuleMovingData(GridUnit runRuleUnit)
        {
            this.runRuleUnit = runRuleUnit;
            nextCells = new HashSet<GameGridCell>();
            blockUnits = new HashSet<GridUnit>();
        }

        public virtual void SetData(Direction direction)
        {
            runDirection = direction;
            nextCells.Clear();
            blockUnits.Clear();
            nextMainCell = LevelManager.Ins.GetNeighbourCell(runRuleUnit.MainCell, direction);
            for (int i = 0; i < runRuleUnit.cellInUnits.Count; i++)
            {
                GameGridCell neighbour = LevelManager.Ins.GetNeighbourCell(runRuleUnit.cellInUnits[i], direction);
                if (neighbour is null) continue;
                for (HeightLevel j = runRuleUnit.StartHeight; j <= runRuleUnit.EndHeight; j++)
                {
                    GridUnit unit = neighbour.GetGridUnitAtHeight(j);
                    if (unit is null || unit == runRuleUnit) continue;
                    blockUnits.Add(unit);
                }
                nextCells.Add(neighbour);
            }
        }
    }
}
