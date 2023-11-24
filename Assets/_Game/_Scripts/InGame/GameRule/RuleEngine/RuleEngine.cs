using System.Collections.Generic;
using _Game.GameRule.Rule;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Game.GameRule.RuleEngine
{
    [CreateAssetMenu(fileName = "RuleEngine", menuName = "RuleSO/RuleEngine", order = 1)]
    public class RuleEngine : SerializedScriptableObject
    {
        // ReSharper disable once CollectionNeverUpdated.Local
        // ReSharper disable once Unity.RedundantSerializeFieldAttribute
        [SerializeField] private readonly List<IRule> rules = new();
        [SerializeField] private bool acceptReverseRule = true;
        public void ApplyRules(RuleEngineData data)
        {
            for (int index = 0; index < rules.Count; index++)
            {
                IRule rule = rules[index];
                if (rule.IsApplicable(data)) rule.Apply(data);
                else if (acceptReverseRule && index > 0)
                {
                    for (int j = index - 1; j >= 0; j--)
                    {
                        IRule reverseRule = rules[j];reverseRule.Reverse(data);
                    }
                    return;
                }
            }
        }
    }
}
