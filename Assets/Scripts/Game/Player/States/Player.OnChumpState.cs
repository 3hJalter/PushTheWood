using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public partial class Player
    {
        public class OnChumpState : State
        {
            public OnChumpState(Player Data) : base(Data)
            {
            }
        }

        public class OnWaterChumpState : OnChumpState
        {
            public OnWaterChumpState(Player Data) : base(Data)
            {
            }
        }

        public class OnGroundChumpState : OnChumpState
        {
            public OnGroundChumpState(Player Data) : base(Data)
            {
            }
        }
    }
}