using System.Collections.Generic;
using UnityEngine;

namespace _Game.Data
{
    [CreateAssetMenu(fileName = "TutorialData", menuName = "ScriptableObjects/TutorialData", order = 1)]
    public class TutorialData : ScriptableObject
    {
        [SerializeField] private List<TutorialContext> tutorialContext = new();

        public int CountTutorial => tutorialContext.Count;

        public TutorialContext GetTutorial(int index)
        {
            return tutorialContext[index];
        }
    }
}
