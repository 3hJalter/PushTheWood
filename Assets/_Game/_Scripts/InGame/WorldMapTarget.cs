using UnityEngine;

namespace _Game._Scripts.InGame
{
    public class WorldMapTarget : HMonoBehaviour
    {
        [SerializeField] private Transform header;
        [SerializeField] private Transform footer;

        private int _furthestLevelIndex;
        private int _nearestLevelIndex;
        
        public float GetHeaderZPos()
        {
            return header.position.z;  
        }
        
        public float GetFooterZPos()
        {
            return footer.position.z;
        }
    }
}
