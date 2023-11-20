using _Game.DesignPattern;
using GameGridEnum;
using UnityEngine;

namespace _Game.GameGrid.GridSurface
{
    public abstract class GridSurface : GameUnit
    {
        [SerializeField] protected GridSurfaceType surfaceType;
        public PoolType PoolType => ConvertToPoolType(surfaceType);
        [SerializeField] private int islandID = -1;

        public int IslandID
        {
            get => islandID;
            set
            {
                if (value < 0 || islandID >= 0) return;
                if (surfaceType != GridSurfaceType.Water) islandID = value;
            }
        }
        public GridSurfaceType SurfaceType => surfaceType;

        public virtual void OnDespawn()
        {
            islandID = -1;
            SimplePool.Despawn(this);
        }

        public virtual void OnUnitEnter(GridUnit.GridUnit gridUnit)
        { }
    }
}
