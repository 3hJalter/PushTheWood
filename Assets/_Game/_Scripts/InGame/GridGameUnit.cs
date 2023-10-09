using MapEnum;
using UnityEngine;

namespace _Game
{
    public class GridGameUnit : HMonoBehaviour
    {
        [SerializeField] private CellType type;

        [SerializeField] private CellState state;

        public CellType Type => type;
        public CellState State => state;
    }
}
