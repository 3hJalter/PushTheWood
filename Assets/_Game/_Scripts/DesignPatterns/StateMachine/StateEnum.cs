namespace _Game.DesignPattern.StateMachine
{
    public enum StateEnum
    {
        Idle = 0, // Idle for all
        JumpDown = 1,
        JumpUp = 2,
        Interact = 3,
        Push = 4,
        Fall = 5,
        CutTree = 6,
        Die = 7,
        Happy = 8,

        // Chump
        Move = 9, // Move also for Player
        Roll = 10,
        TurnOver = 11,
        FormRaft = 12,
        RollBlock = 13, 
    }
}
