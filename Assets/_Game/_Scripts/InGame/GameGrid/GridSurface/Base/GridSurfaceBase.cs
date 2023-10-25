using _Game.DesignPattern;
using GameGridEnum;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Game.GameGrid.GridSurface
{
    public abstract class GridSurfaceBase : GameUnit
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

        public void OnDespawn()
        {
            islandID = -1;
            SimplePool.Despawn(this);
        }
    }
}
