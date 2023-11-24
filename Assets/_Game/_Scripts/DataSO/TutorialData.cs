using System.Collections.Generic;
using _Game._Scripts.UIs.Tutorial;
using UnityEngine;

namespace _Game.Data
{
    [CreateAssetMenu(fileName = "TutorialData", menuName = "ScriptableObjects/TutorialData", order = 1)]
    public class TutorialData : ScriptableObject
    {
        [SerializeField] private List<TutorialContext> tutorialContext = new();
        
        public TutorialContext GetTutorial(int index)
        {
            return tutorialContext[index];
        }
        
        public int CountTutorial => tutorialContext.Count;
    }
}
