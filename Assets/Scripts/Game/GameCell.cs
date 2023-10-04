using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    using Utilities.AI;
    public class GameCell : GridCell<GameCellData>
    {
        private int islandID = -1;
        public Chump Tree1;
        public Chump Tree2;
        public Player Player = null;
        public bool IsBlockingRollingTree
        {
            get
            {
                return value.state == CELL_STATE.LOW_ROCK_OBSTANCE 
                    || value.state == CELL_STATE.HIGH_ROCK_OBSTANCE;
            }
        }
        public bool IsRaft
        {
            get => Tree1 != null && Tree2 != null;
        }
        public bool IsBlockingPlayer
        {
            get => ((value.type == CELL_TYPE.WATER) && !IsRaft)
                || value.state == CELL_STATE.HIGH_ROCK_OBSTANCE;
        }       
        public bool IsCanPushRaft
        {
            get => (value.state == CELL_STATE.HIGH_ROCK_OBSTANCE)
                || (Tree1 != null && value.type == CELL_TYPE.GROUND);
        }

        public int IslandID
        {
            get => islandID;
            set
            {
                if (value < 0 || islandID >= 0) return;
                if(this.value.type == CELL_TYPE.GROUND) 
                { 
                    islandID = value;
                }
            }
        }
        public GameCell()
        {
            value = new GameCellData();
        }
        public GameCell(GameCellData data)
        {
            value = data;
        }
        public GameCell(GameCell copy) : base(copy)
        {
            value = new GameCellData();
            value.type = copy.Value.type;
            value.state = copy.Value.state;

            islandID = copy.islandID;
            Tree1 = copy.Tree1;
            Tree2 = copy.Tree2;
            Player = copy.Player;

        }
    }
}