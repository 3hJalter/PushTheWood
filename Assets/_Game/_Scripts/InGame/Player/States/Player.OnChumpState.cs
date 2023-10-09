namespace _Game._Scripts.InGame.Player
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
