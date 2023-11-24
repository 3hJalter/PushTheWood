using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace _Game.DesignPattern
{
    public static class SimplePool
    {
        private const string PATH = "Pool";
        private const int DEFAULT_POOL_SIZE = 3;

        private static readonly Dictionary<int, Pool> Pools = new();

        private static readonly Dictionary<PoolType, GameUnit> PoolTypes = new();

        private static GameUnit[] _gameUnitResources;

        private static Transform _root;

        // All of our pools
        private static readonly Dictionary<int, Pool> PoolInstanceID = new();

        private static Transform Root
        {
            get
            {
                if (_root == null)
                {
                    _root = Object.FindObjectOfType<PoolController>().transform;

                    if (_root == null) _root = new GameObject(PATH).transform;
                }

                return _root;
            }
        }

        private static void Init(GameUnit prefab = null, int qty = DEFAULT_POOL_SIZE, Transform parent = null,
            bool collect = false, bool clamp = false)
        {
            if (prefab != null && !IsHasPool(prefab.GetInstanceID()))
                PoolInstanceID.Add(prefab.GetInstanceID(), new Pool(prefab, qty, parent, collect, clamp));
        }

        private static bool IsHasPool(int instanceID)
        {
            return PoolInstanceID.ContainsKey(instanceID);
        }

        public static void Preload(GameUnit prefab, int qty = 1, Transform parent = null, bool collect = false,
            bool clamp = false)
        {
            PoolTypes.TryAdd(prefab.poolType, prefab);

            if (prefab == null)
            {
                if (parent != null) Debug.LogError(parent.name + " : IS EMPTY!!!");
                return;
            }

            Init(prefab, qty, parent, collect, clamp);

            // Make an array to grab the objects we're about to pre-spawn.
            GameUnit[] obs = new GameUnit[qty];
            for (int i = 0; i < qty; i++) obs[i] = Spawn(prefab);

            // Now despawn them all.
            for (int i = 0; i < qty; i++) Despawn(obs[i]);
        }

        public static T Spawn<T>(PoolType poolType, Vector3 pos, Quaternion rot) where T : GameUnit
        {
            return Spawn(GetGameUnitByType(poolType), pos, rot) as T;
        }

        public static T Spawn<T>(PoolType poolType) where T : GameUnit
        {
            return Spawn<T>(GetGameUnitByType(poolType));
        }

        public static T Spawn<T>(GameUnit obj, Vector3 pos, Quaternion rot) where T : GameUnit
        {
            return Spawn(obj, pos, rot) as T;
        }

        public static T Spawn<T>(GameUnit obj) where T : GameUnit
        {
            return Spawn(obj) as T;
        }

        public static GameUnit Spawn(GameUnit obj, Vector3 pos, Quaternion rot)
        {
            if (!PoolInstanceID.ContainsKey(obj.GetInstanceID()))
            {
                Transform newRoot = new GameObject(obj.name).transform;
                newRoot.SetParent(Root);
                Preload(obj, 1, newRoot, true);
            }

            return PoolInstanceID[obj.GetInstanceID()].Spawn(pos, rot);
        }

        public static GameUnit Spawn(GameUnit obj)
        {
            if (!PoolInstanceID.ContainsKey(obj.GetInstanceID()))
            {
                Transform newRoot = new GameObject(obj.name).transform;
                newRoot.SetParent(Root);
                Preload(obj, 1, newRoot, true);
            }

            return PoolInstanceID[obj.GetInstanceID()].Spawn();
        }

        public static void Despawn(GameUnit obj)
        {
            if (obj.gameObject.activeSelf)
            {
                if (Pools.ContainsKey(obj.GetInstanceID()))
                    Pools[obj.GetInstanceID()].DeSpawn(obj);
                else
                    Object.Destroy(obj.gameObject);
            }
        }

        public static void Release(GameUnit obj)
        {
            if (Pools.ContainsKey(obj.GetInstanceID()))
            {
                Pools[obj.GetInstanceID()].Release();
                Pools.Remove(obj.GetInstanceID());
            }
            else
            {
               Object.Destroy(obj.gameObject);
            }
        }
        
        public static void ReleaseImmediate(GameUnit obj)
        {
            if (Pools.ContainsKey(obj.GetInstanceID()))
            {
                Pools[obj.GetInstanceID()].Release();
                Pools.Remove(obj.GetInstanceID());
            }
            else
            {
                Object.DestroyImmediate(obj);
            }
        }

        public static void Collect(GameUnit obj)
        {
            if (PoolInstanceID.ContainsKey(obj.GetInstanceID()))
                PoolInstanceID[obj.GetInstanceID()].Collect();
        }

        public static void CollectAll()
        {
            foreach (KeyValuePair<int, Pool> item in PoolInstanceID)
                if (item.Value.IsCollect)
                    item.Value.Collect();
        }

        private static GameUnit GetGameUnitByType(PoolType poolType)
        {
            if (_gameUnitResources == null || _gameUnitResources.Length == 0)
                _gameUnitResources = Resources.LoadAll<GameUnit>(PATH);

            if (!PoolTypes.ContainsKey(poolType) || PoolTypes[poolType] == null)
            {
                GameUnit unit = null;

                for (int i = 0; i < _gameUnitResources.Length; i++)
                    if (_gameUnitResources[i].poolType == poolType)
                    {
                        unit = _gameUnitResources[i];
                        break;
                    }

                PoolTypes.Add(poolType, unit);
            }

            return PoolTypes[poolType];
        }

        private class Pool
        {
            //collect obj active inGame
            private readonly List<GameUnit> _active;

            private readonly Queue<GameUnit> _inactive;
            private readonly int _mAmount;

            private readonly bool _mClamp;

            private readonly Transform _mSRoot;

            // The prefab that we are pooling
            private readonly GameUnit _prefab;

            // Constructor
            public Pool(GameUnit prefab, int initialQty, Transform parent, bool collect, bool clamp)
            {
                _inactive = new Queue<GameUnit>(initialQty);
                _mSRoot = parent;
                _prefab = prefab;
                IsCollect = collect;
                _mClamp = clamp;
                if (IsCollect) _active = new List<GameUnit>();
                if (_mClamp) _mAmount = initialQty;
            }

            public bool IsCollect { get; }

            public int Count => _inactive.Count;

            // Spawn an object from our pool
            public GameUnit Spawn(Vector3 pos, Quaternion rot)
            {
                GameUnit obj = Spawn();

                obj.Tf.SetPositionAndRotation(pos, rot);

                return obj;
            }

            public GameUnit Spawn()
            {
                GameUnit obj;
                if (_inactive.Count == 0)
                {
                    obj = Object.Instantiate(_prefab, _mSRoot);

                    if (!Pools.ContainsKey(obj.GetInstanceID()))
                        Pools.Add(obj.GetInstanceID(), this);
                }
                else
                {
                    // Grab the last object in the inactive array
                    obj = _inactive.Dequeue();

                    if (obj == null) return Spawn();
                }

                if (IsCollect) _active.Add(obj);
                if (_mClamp && _active.Count >= _mAmount) DeSpawn(_active[0]);

                obj.gameObject.SetActive(true);

                return obj;
            }

            // Return an object to the inactive pool.
            public void DeSpawn(GameUnit obj)
            {
                if (obj != null /*&& !inactive.Contains(obj)*/)
                {
                    obj.gameObject.SetActive(false);
                    _inactive.Enqueue(obj);
                }

                if (IsCollect) _active.Remove(obj);
            }

            public void Clamp(int amount)
            {
                while (_inactive.Count > amount)
                {
                    GameUnit go = _inactive.Dequeue();
                    Object.DestroyImmediate(go);
                }
            }

            public void Release()
            {
                while (_inactive.Count > 0)
                {
                    GameUnit go = _inactive.Dequeue();
                    Object.DestroyImmediate(go);
                }

                _inactive.Clear();
            }

            public void Collect()
            {
                while (_active.Count > 0) DeSpawn(_active[0]);
            }
        }
    }

    [Serializable]
    public class PoolAmount
    {
        [Header("-- Pool Amount --")] public Transform root;

        public GameUnit prefab;
        public int amount;
        public bool collect;
        public bool clamp;
    }

    public enum PoolType
    {
        None = -1,
        // Grid Surface
        SurfaceWater = 0,
        SurfaceGround = 1,
        SurfaceGroundTut = 2,
        // Grid Unit Dynamic
        Player = 3,
        ChumpShort = 4,
        ChumpHigh = 5,
        Raft = 6,
        RaftLong = 12,
        // Grid Unit Static
        RockShort = 7,
        RockHigh = 8,
        TreeRoot = 9,
        TreeShort = 10,
        TreeHigh = 11,
        FinalPoint = 13,


    }
}
