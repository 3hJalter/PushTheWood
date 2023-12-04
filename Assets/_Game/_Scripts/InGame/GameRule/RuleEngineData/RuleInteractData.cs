using _Game.GameGrid.Unit;

namespace _Game.GameRule.RuleEngine
{
    public class RuleInteractData : RuleEngineData
    {
        public Direction runDirection;
        public GridUnit interactedUnit;
        public bool isInteractAccept;
        public RuleInteractData(GridUnit runRuleUnit)
        {
            this.runRuleUnit = runRuleUnit;
        }
        public void SetData(Direction direction, GridUnit interactedUnitIn)
        {
            isInteractAccept = false;
            runDirection = direction;
            interactedUnit = interactedUnitIn;
        }
    }
}
