using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace VinhLB
{
    [CreateAssetMenu(fileName = "TutorialUIData", menuName = "ScriptableObjects/TutorialUIData", order = 4)]
    public class TutorialUIData : SerializedScriptableObject
    {
        public Dictionary<TutorialType, TutorialUI[]> TutorialUIDict = new();
    }

    [System.Serializable]
    public class TutorialUI
    {
        public Sprite TutorialSprite;
        public string Description;
    }

    public enum TutorialType
    {
        None = -1,
        Test = 0,
    }
}