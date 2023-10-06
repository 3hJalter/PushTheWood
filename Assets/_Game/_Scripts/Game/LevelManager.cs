using System.Collections.Generic;
using _Game._Scripts.Managers;
using _Game._Scripts.UIs.Screen;
using DesignPattern;
using DG.Tweening;
using MapEnum;
using UnityEngine;
using CameraType = _Game._Scripts.Managers.CameraType;

namespace _Game
{
    public class LevelManager : Singleton<LevelManager>
    {
        private const float TRANSITION_LEVEL_TIME = 2f;
        private static int _islandID;

        [SerializeField] private Map map;

        [SerializeField] public Player player;

        public int landIndex = 1;
        public bool winBySkip;
        public List<Vector3> initPos;
        public int steps;
        private readonly List<GameCell> closeCells = new();
        private readonly List<GameCell>[] islands = new List<GameCell>[50];


        private readonly List<GameCell> openCells = new();
        private readonly List<GameCell>[] saveIslands = new List<GameCell>[50];
        private List<GameCell> island;


        public Map Map => map;

        private void Start()
        {
            // TEMP
            map = Instantiate(DataManager.Ins.WorldData.GetMap(0), transform);
            map.OnInit();
            player = map.Player;
            SetCameraToPlayer();
            InitGridData();
            InitGameObject();
        }

        private void SetCameraToPlayer()
        {
            CameraManager.Ins.ChangeCameraTarget(CameraType.MainMenuCamera, player.Tf);
            CameraManager.Ins.ChangeCameraTarget(CameraType.InGameCamera, player.Tf);
        }
        
        
        public void Restart()
        {
            Destroy(map.gameObject);
            Instantiate(map, transform).OnInit();
            // TEMP
            player = map.Player;
            // player._uiManager = FindObjectOfType<UIManager>();

            /*player.transform.position = cell.WorldPos;
            cell.Player = player;*/
            InitGridData();
            InitGameObject();
        }

        private void InitGridData()
        {
            GameUnit[] units = FindObjectsOfType<GameUnit>();
            for (int i = 0; i < units.Length; i++)
            {
                GameCell cell = map.GridMap.GetGridCell(units[i].transform.position);

                if (units[i].Type != CellType.None) cell.Value.type = units[i].Type;
                if (units[i].State != CellState.None) cell.Value.state = units[i].State;
            }
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

            cell.IslandID = _islandID;
            openCells.Add(cell);
            island = new List<GameCell>();

            islands[_islandID] = island;
            island.Add(cell);

            while (openCells.Count > 0) FindGroundNearby(openCells[0]);
            _islandID += 1;
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

        public void GoNextLevel()
        {
            steps = 0;
            if (winBySkip) return;
            player.SetPosition(initPos[landIndex]);
            FxManager.Ins.PlayTweenFog(false, TRANSITION_LEVEL_TIME);
            CameraManager.Ins.ChangeCamera(CameraType.WorldMapCamera);
            DOVirtual.DelayedCall(TRANSITION_LEVEL_TIME, () => { UIManager.Ins.OpenUI<InGameScreen>(); });
        }

        public void GoLevel(int index)
        {
            steps = 0;
            player.SetPosition(initPos[index]);
            UIManager.Ins.OpenUI<InGameScreen>();
        }
    }
}
