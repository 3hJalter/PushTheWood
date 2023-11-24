
using _Game.GameRule.RuleEngine;

namespace _Game.DesignPattern
{
    public interface IRule
    {
        bool IsApplicable(RuleEngineData data);
        void Apply(RuleEngineData data);
        
        void Reverse(RuleEngineData data);
        
    }
}
