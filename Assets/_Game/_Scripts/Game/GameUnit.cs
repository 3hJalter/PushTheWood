using MapEnum;
using UnityEngine;

namespace _Game
{
    public class GameUnit : MonoBehaviour
    {
        [SerializeField] private CellType type;

        [SerializeField] private CellState state;

        public CellType Type => type;
        public CellState State => state;
    }
}
