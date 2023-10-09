using System.Collections.Generic;
using UnityEngine;

namespace _Game._Scripts.Data
{
    [CreateAssetMenu(fileName = "WorldData", menuName = "ScriptableObjects/WorldData", order = 1)]
    public class WorldData : ScriptableObject
    {
        [SerializeField] private List<Map> maps = new();
        
        public Map GetMap(int index)
        {
            return maps[index];
        }
    }
}
