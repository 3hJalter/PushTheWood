using UnityEngine;

namespace _Game._Scripts.DesignPattern
{
    public abstract class GameUnit : HMonoBehaviour
    {
        [HideInInspector]
        public PoolType poolType;
        
        public PoolType ConvertToPoolType<T>(T type)
        {
            return (PoolType) (object) type;
        }
    }
}
