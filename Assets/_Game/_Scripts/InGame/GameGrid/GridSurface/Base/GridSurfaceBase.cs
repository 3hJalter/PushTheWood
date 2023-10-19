using _Game.DesignPattern;
using GameGridEnum;
using UnityEngine;

namespace _Game.GameGrid.GridSurface
{
    public abstract class GridSurfaceBase : GameUnit
    {
        [SerializeField] protected GridSurfaceType surfaceType;
        private int _islandID = -1;

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
    }
}
