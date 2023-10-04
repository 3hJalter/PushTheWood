using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Game
{
    using System;
    using Utilities;
    public class LevelManager : Singleton<LevelManager>
    {
        private static int ISLAND_ID = 0;
        [SerializeField]
        Map map;
        [SerializeField]
        public Player player;
        [SerializeField]
        GameObject environment;
        [SerializeField]
        GameObject world;
        [SerializeField]
        Cinemachine.CinemachineVirtualCameraBase _cameraMain;
        [SerializeField]
        Cinemachine.CinemachineVirtualCameraBase _cameraPlay;
        public bool isWin = false;
        public int Land = 1;
        public bool WinBySkip;
        public List<Vector3> initPos;
        GameObject worlddd;
        public int Steps;


        public Map Map => map;
        List<GameCell>[] islands = new List<GameCell>[50]; 
        List<GameCell>[] saveIslands = new List<GameCell>[50];

        private void Start()
        {
            worlddd = Instantiate(world, transform);
            //worlddd = world;
            map = worlddd.GetComponent<Map>();
            environment = worlddd.transform.GetChild(0).gameObject;
            player = worlddd.transform.GetChild(1).GetComponent<Player>();
            player._uiManager = FindObjectOfType<Daivq.UIManager>();



            //player._uiManager.SetUpCamera(this.transform);
            /*player.transform.position = cell.WorldPos;
            cell.Player = player;*/
            if(_cameraMain != null)
            {
                InitCamera();
            }
            InitGridData();
            InitGameObject();
        }

        

        public void Restart()
        {
            Destroy(worlddd);
            worlddd = Instantiate(world, transform);

            map = worlddd.GetComponent<Map>();
            environment = worlddd.transform.GetChild(0).gameObject;
            player = worlddd.transform.GetChild(1).GetComponent<Player>();
            player._uiManager = FindObjectOfType<Daivq.UIManager>();

            if(_cameraMain != null)
            {
                InitCamera();
            }
            /*player.transform.position = cell.WorldPos;
            cell.Player = player;*/
            InitGridData();
            InitGameObject();
        }

        void InitCamera()
        {
            _cameraMain.Follow = player.transform;
            _cameraMain.LookAt = player.transform;
            _cameraPlay.Follow = player.transform;
            _cameraPlay.LookAt = player.transform;
        }
        private void InitGridData()
        {
            GameUnit[] units = FindObjectsOfType<GameUnit>();
            GameCell cell;
            for (int i = 0; i < units.Length; i++)
            {
                cell = map.GridMap.GetGridCell(units[i].transform.position);

                if (units[i].Type != CELL_TYPE.NONE)
                {
                    cell.Value.type = units[i].Type;
                }
                if (units[i].State != CELL_STATE.NONE)
                {
                    cell.Value.state = units[i].State;
                }
            }
/*            for(int x = 0; x < map.GridMap.Width; x++)
            {
                for(int y = 0; y < map.GridMap.Height; y++)
                {
                    cell = map.GridMap.GetGridCell(x, y);
                    if(cell.Value.type == CELL_TYPE.WATER)
                    {
                        Instantiate(waterPrefab, cell.WorldPos, Quaternion.identity, environment.transform);
                    }
                }
            }*/
        }
        private void InitGameObject()
        {
            IInit[] objs = FindObjectsOfType<Chump>();
            foreach(IInit tree in objs)
            {
                tree.OnInit();
            }
            player.OnInit();
        }


        List<GameCell> openCells = new List<GameCell>();
        List<GameCell> closeCells = new List<GameCell>();
        List<GameCell> island;
        public int FindIsland(GameCell cell)
        {
            if (cell.IslandID >= 0) return cell.IslandID;

            openCells.Clear();
            closeCells.Clear();

            cell.IslandID = ISLAND_ID;
            openCells.Add(cell);
            island = new List<GameCell>();

            islands[ISLAND_ID] = island;
            island.Add(cell);

            while(openCells.Count > 0)
            {
                FindGroundNearby(openCells[0]);
            }
            ISLAND_ID += 1;
            return cell.IslandID;
        }

        private void FindGroundNearby(GameCell cell)
        {
            int x;
            int y;

            closeCells.Add(cell);
            openCells.Remove(cell);
            for(int i = -1; i <= 1; i++)
            {
                for(int j = -1; j <= 1; j++)
                {
                    if (i == j) return;

                    x = cell.X + i;
                    y = cell.Y + j;

                    if(x < 0 || y < 0 || x >= map.GridMap.Width || y >= map.GridMap.Height)
                    {
                        return;
                    }
                    GameCell neighbor = map.GridMap.GetGridCell(x, y);
                    if (!closeCells.Contains(neighbor))
                    {
                        if (neighbor.Value.type == CELL_TYPE.GROUND)
                        {
                            neighbor.IslandID = cell.IslandID;
                            openCells.Add(neighbor);
                            island.Add(neighbor);
                        }
                    }
                    
                }
            }
        }
        public void CreateGridSaveIsland(int id)
        {
            List<GameCell> save = new List<GameCell>();

            for (int i = 0; i < islands[id].Count; i++)
            {
                save.Add(new GameCell(islands[id][i]));
            }
            saveIslands[id] = save;
        }

        public void RecovereGridSaveIsland(int id)
        {
            for(int i = 0; i < saveIslands[id].Count; i++)
            {
                GameCell cell = saveIslands[id][i];
                map.GridMap.SetGridCell(cell.X, cell.Y, cell);
            }
        }
    }
}