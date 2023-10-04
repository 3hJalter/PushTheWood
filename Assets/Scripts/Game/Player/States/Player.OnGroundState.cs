using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public partial class Player
    {
        public class OnGroundState : State
        {
            public OnGroundState(Player Data) : base(Data)
            {
            }
        }
    }
}