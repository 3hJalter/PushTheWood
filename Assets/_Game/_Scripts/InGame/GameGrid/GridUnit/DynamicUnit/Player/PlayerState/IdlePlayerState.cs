using _Game.DesignPattern.StateMachine;
using _Game.GameGrid.Unit.StaticUnit;
using UnityEngine;

namespace _Game.GameGrid.Unit.DynamicUnit.Player.PlayerState
{
    public class IdlePlayerState : IState<Player>
    {
        private bool _isChangeAnim;


        public void OnEnter(Player t)
        {

        }

        public void OnExecute(Player t)
        {

            if (t.Direction == Direction.None)
            {
                if (!_isChangeAnim && !t.isRideVehicle)
                {
                    _isChangeAnim = true;
                    t.ChangeAnim(Constants.IDLE_ANIM);
                }
                return;
            }

            t.LookDirection(t.Direction);
            if (t.HasVehicle() && !t.isRideVehicle)
                if (t.CanRideVehicle(t.Direction))
                {
                    t.OnRideVehicle(t.Direction);
                    return;
                }

            t.MovingData.SetData(t.Direction);
            if (!t.conditionMergeOnMoving.IsApplicable(t.MovingData))
            {
                if (t.MovingData.blockDynamicUnits.Count > 0)
                    t.ChangeState(StateEnum.Push);
                else if (!_isChangeAnim) t.ChangeAnim(Constants.IDLE_ANIM);
                return;
            }

            bool hasTreeRoot = false;
            if (t.MovingData.blockStaticUnits.Count == 1 &&
                t.MovingData.blockStaticUnits[0] is TreeRoot tr) // Temporary for only tree root
            {
                hasTreeRoot = true;
                tr.UpHeight(t, t.MovingData.inputDirection);
            }

            t.SetEnterCellData(t.MovingData.inputDirection, t.MovingData.enterMainCell, t.UnitTypeY);
            t.SetIslandId(t.MovingData.enterMainCell);
            t.OnOutCells();
            t.OnEnterCells(t.MovingData.enterMainCell, t.MovingData.enterCells);
            if (hasTreeRoot) // Temporary
                t.ChangeState(StateEnum.JumpUp);
            else
                t.ChangeState(t.EnterPosData.isFalling ? StateEnum.JumpDown : StateEnum.Move);
        }

        public void OnExit(Player t)
        {
            _isChangeAnim = false;
        }
    }
}
