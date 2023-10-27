using System.Collections.Generic;
using _Game.Camera;
using _Game.DesignPattern;
using _Game.InGame.Player;
using _Game.UIs.Screen;
using DG.Tweening;
using MapEnum;
using UnityEngine;

namespace _Game.Managers
{
    public class OldLevelManager : Singleton<OldLevelManager>
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
        private List<GameCell> island;


        public Map Map => map;

        private void Start()
        {
            // TEMP
            map = Instantiate(DataManager.Ins.WorldData.GetMap(0), Tf);
            map.OnInit();
            player = map.Player;
            SetCameraToPlayer();
            InitGridData();
            InitOnMapObject();
        }

        private void SetCameraToPlayer()
        {
            // CameraManager.Ins.ChangeCameraTarget(ECameraType.MainMenuCamera, player.Tf);
            // CameraManager.Ins.ChangeCameraTarget(ECameraType.InGameCamera, player.Tf);
        }


        public void Restart()
        {
            Destroy(map.gameObject);
            Instantiate(map, transform).OnInit();
            // TEMP
            player = map.Player;
            InitGridData();
            InitOnMapObject();
        }

        private void InitGridData()
        {
            GridGameUnit[] units = FindObjectsOfType<GridGameUnit>();   
            for (int i = 0; i < units.Length; i++)
            {
                GameCell cell = map.GridMap.GetGridCell(units[i].Tf.position);
                if (units[i].Type != CellType.None) cell.Data.type = units[i].Type;
                if (units[i].State != CellState.None) cell.Data.state = units[i].State;
            }
        }

        private void InitOnMapObject()
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

            closeCells.Add(cell);
            openCells.Remove(cell);
            for (int i = -1; i <= 1; i++)
            for (int j = -1; j <= 1; j++)
            {
                if (i == j) return;

                int x = cell.X + i;
                int y = cell.Y + j;

                if (x < 0 || y < 0 || x >= map.GridMap.Width || y >= map.GridMap.Height) return;
                GameCell neighbor = map.GridMap.GetGridCell(x, y);
                if (!closeCells.Contains(neighbor))
                    if (neighbor.Data.type == CellType.Ground)
                    {
                        neighbor.IslandID = cell.IslandID;
                        openCells.Add(neighbor);
                        island.Add(neighbor);
                    }

            }
        }

        public static void OnWin()
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
            // CameraManager.Ins.ChangeCamera(ECameraType.WorldMapCamera);
            CameraFollow.Ins.ChangeCamera(ECameraType.WorldMapCamera, player.Tf);
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
