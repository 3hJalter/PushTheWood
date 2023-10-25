using UnityEngine;

namespace _Game.DesignPattern
{
    public abstract class GameUnit : HMonoBehaviour
    {
        [HideInInspector]
        public PoolType poolType;
        
        protected static PoolType ConvertToPoolType<T>(T type)
        {
            return (PoolType) (object) type;
        }
    }
}
