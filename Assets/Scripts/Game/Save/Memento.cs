using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public abstract class Memento
    {
        protected Vector2Int gridPosition;
        public abstract void Save();
        public abstract void Revert();
    }
}