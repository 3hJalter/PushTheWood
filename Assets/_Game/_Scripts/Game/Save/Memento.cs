using UnityEngine;

namespace _Game
{
    public abstract class Memento
    {
        protected Vector2Int gridPosition;
        public abstract void Save();
        public abstract void Revert();
    }
}
