using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace VinhLB
{
    [CreateAssetMenu(fileName = "EnvironmentObjectData", menuName = "ScriptableObjects/EnvironmentObjectData", order = 4)]
    public class EnvironmentObjectData : SerializedScriptableObject
    {
        [Title("Environment Object Data")]
        [SerializeField]
        private readonly Dictionary<EnvironmentObjectType, GameObject[]> _environmentObjectDict;

        public GameObject GetRandomEnvironmentObject(EnvironmentObjectType type)
        {
            if (!_environmentObjectDict.ContainsKey(type) || _environmentObjectDict[type] == null)
            {
                return null;
            }
            
            int randomIndex = Random.Range(0, _environmentObjectDict[type].Length);

            return _environmentObjectDict[type][randomIndex];
        }
    }

    public enum EnvironmentObjectType
    {
        None = -1,
        Cloud = 0,
        Shark = 1
    }
}