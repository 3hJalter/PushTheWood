using System.Collections.Generic;
using _Game._Scripts.Managers;
using _Game._Scripts.UIs.Screen;
using Cinemachine;
using Daivq;
using DesignPattern;
using DG.Tweening;
using MapEnum;
using UnityEngine;
using CameraType = _Game._Scripts.Managers.CameraType;

namespace _Game
{
    public class LevelManager : Singleton<LevelManager>
    {
        private static int ISLAND_ID;

        [SerializeField] private Map map;

        [SerializeField] public Player player;

        [SerializeField] private GameObject environment;

        [SerializeField] private GameObject world;

        [SerializeField] private CinemachineVirtualCameraBase _cameraMain;

        [SerializeField] private CinemachineVirtualCameraBase _cameraPlay;

        public bool isWin;
        public int Land = 1;
        public bool WinBySkip;
        public List<Vector3> initPos;
        public int Steps;
        private readonly List<GameCell> closeCells = new();
        private readonly List<GameCell>[] islands = new List<GameCell>[50];


        private readonly List<GameCell> openCells = new();
        private readonly List<GameCell>[] saveIslands = new List<GameCell>[50];
        private List<GameCell> island;
        private GameObject worlddd;


        public Map Map => map;

        private void Start()
        {
            worlddd = Instantiate(world, transform);
            //worlddd = world;
            map = worlddd.GetComponent<Map>();
            environment = worlddd.transform.GetChild(0).gameObject;
            player = worlddd.transform.GetChild(1).GetComponent<Player>();
            // player._uiManager = FindObjectOfType<UIManager>();


            //player._uiManager.SetUpCamera(this.transform);
            /*player.transform.position = cell.WorldPos;
            cell.Player = player;*/
            if (_cameraMain != null) InitCamera();
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
            // player._uiManager = FindObjectOfType<UIManager>();

            if (_cameraMain != null) InitCamera();
            /*player.transform.position = cell.WorldPos;
            cell.Player = player;*/
            InitGridData();
            InitGameObject();
        }

        private void InitCamera()
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

                if (units[i].Type != CellType.None) cell.Value.type = units[i].Type;
                if (units[i].State != CellState.None) cell.Value.state = units[i].State;
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
            foreach (IInit tree in objs) tree.OnInit();
            player.OnInit();
        }

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

            while (openCells.Count > 0) FindGroundNearby(openCells[0]);
            ISLAND_ID += 1;
            return cell.IslandID;
        }

        private void FindGroundNearby(GameCell cell)
        {
            int x;
            int y;

            closeCells.Add(cell);
            openCells.Remove(cell);
            for (int i = -1; i <= 1; i++)
            for (int j = -1; j <= 1; j++)
            {
                if (i == j) return;

                x = cell.X + i;
                y = cell.Y + j;

                if (x < 0 || y < 0 || x >= map.GridMap.Width || y >= map.GridMap.Height) return;
                GameCell neighbor = map.GridMap.GetGridCell(x, y);
                if (!closeCells.Contains(neighbor))
                    if (neighbor.Value.type == CellType.Ground)
                    {
                        neighbor.IslandID = cell.IslandID;
                        openCells.Add(neighbor);
                        island.Add(neighbor);
                    }

            }
        }

        public void CreateGridSaveIsland(int id)
        {
            List<GameCell> save = new();

            for (int i = 0; i < islands[id].Count; i++) save.Add(new GameCell(islands[id][i]));
            saveIslands[id] = save;
        }

        public void RecovereGridSaveIsland(int id)
        {
            for (int i = 0; i < saveIslands[id].Count; i++)
            {
                GameCell cell = saveIslands[id][i];
                map.GridMap.SetGridCell(cell.X, cell.Y, cell);
            }
        }

        public void OnWin()
        {
            UIManager.Ins.CloseAll();
            UIManager.Ins.OpenUI<WinScreen>();
        }

        private const float TRANSITION_LEVEL_TIME = 2f;
        
        public void GoNextLevel()
        {
            Steps = 0;
            if (WinBySkip) return;
            player.SetPosition(initPos[Land]);
            FxManager.Ins.PlayTweenFog(false, TRANSITION_LEVEL_TIME);
            CameraManager.Ins.ChangeCamera(CameraType.WorldMapCamera);
            DOVirtual.DelayedCall(TRANSITION_LEVEL_TIME, () =>
            {
                UIManager.Ins.OpenUI<InGameScreen>();
            });
        }

        public void GoLevel(int index)
        {
            Steps = 0;
            player.SetPosition(initPos[index]);
            UIManager.Ins.OpenUI<InGameScreen>();
        }
    }
}
