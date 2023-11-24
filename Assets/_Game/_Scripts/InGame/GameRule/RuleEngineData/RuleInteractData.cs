using _Game.GameGrid.GridUnit;

namespace _Game.GameRule.RuleEngine
{
    public class RuleInteractData : RuleEngineData
    {
        public Direction runDirection;
        public GridUnit interactedUnit;
        public RuleInteractData(GridUnit runRuleUnit)
        {
            this.runRuleUnit = runRuleUnit;
        }
        public void SetData(Direction direction, GridUnit interactedUnitIn)
        {
            runDirection = direction;
            interactedUnit = interactedUnitIn;
        }
    }
}
