using _Game._Scripts.DesignPattern;
using GameGridEnum;
using UnityEngine;

namespace _Game.GameGrid.GridSurface
{
    public abstract class GridSurfaceBase : GameUnit, IInit
    {
        [SerializeField] protected GridSurfaceType surfaceType;
        [SerializeField] private int _islandID = -1;

        public int IslandID
        {
            get => _islandID;
            set
            {
                if (value < 0 || _islandID >= 0) return;
                if (surfaceType != GridSurfaceType.Water) _islandID = value;
            }
        }

        public GridSurfaceType SurfaceType => surfaceType;

        public virtual void OnInit()
        {
        }
    }
}
