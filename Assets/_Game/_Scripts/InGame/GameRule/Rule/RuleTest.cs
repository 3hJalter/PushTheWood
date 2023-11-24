using _Game.GameRule.RuleEngine;
using UnityEngine;

namespace _Game.GameRule.Rule
{
    [CreateAssetMenu(fileName = "RuleTest", menuName = "RuleSO/Moving/RuleTest", order = 1)]
    public class RuleTest : ScriptableObject, IRule
    {
        public bool IsApplicable(RuleEngineData data)
        {
            throw new System.NotImplementedException();
        }

        public void Apply(RuleEngineData data)
        {
            throw new System.NotImplementedException();
        }

        public void Reverse(RuleEngineData data)
        {
            throw new System.NotImplementedException();
        }
    }
}
