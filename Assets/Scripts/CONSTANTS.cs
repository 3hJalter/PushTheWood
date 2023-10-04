using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CONSTANTS 
{
    public enum PATH_FINDING
    {
        A_STAR = 0,
        BFS = 1,
        DFS = 2,
    }
    public enum PLANE
    {
        XY = 0,
        XZ = 1,
        YZ = 2,
    }

    public const float TREE_HEIGHT = 1f;
    public const float MOVING_TIME = 0.25f;
    public const float MOVING_LOG_TIME = 0.01f;
}

namespace Game
{
    public enum CELL_TYPE
    {
        NONE = 0,
        GROUND = 1,
        WATER = 2,
    }
    public enum CELL_STATE
    {
        NONE = 0,
        PLAYER = 1,
        TREE_OBSTANCE = 3,
        LOW_ROCK_OBSTANCE = 4,
        HIGH_ROCK_OBSTANCE = 5,
    }

    public enum TREE_TYPE
    {
        HORIZONTAL = 0,
        VERTICAL = 1,
    }

    public enum TREE_STATE
    {
        UP = 0,
        DOWN = 1,
    }

    public class GameCellData
    {
        public CELL_TYPE type = CELL_TYPE.WATER;
        public CELL_STATE state = CELL_STATE.NONE;
    }

    public interface IInit
    {
        public void OnInit();
    }
}
