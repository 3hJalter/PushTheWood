using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class GameUnit : MonoBehaviour
    {
        [SerializeField]
        CELL_TYPE type;
        [SerializeField]
        CELL_STATE state;
        public CELL_TYPE Type => type;
        public CELL_STATE State => state;
    }
}