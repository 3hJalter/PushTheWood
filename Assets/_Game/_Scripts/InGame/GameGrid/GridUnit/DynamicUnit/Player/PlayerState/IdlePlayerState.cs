using _Game._Scripts.InGame;
using _Game.DesignPattern.StateMachine;
using _Game.GameGrid.Unit.StaticUnit;
using _Game.Managers;
using _Game.Utilities.Timer;
using System.Collections.Generic;
using UnityEngine;

namespace _Game.GameGrid.Unit.DynamicUnit.Player.PlayerState
{
    public class IdlePlayerState : IState<Player>
    {
        private bool _isChangeAnim;
        private bool isFirstStop;
        private int cutTreeFrameCount;
        bool hasTreeRoot = false;
        STimer sleepTimer;

        public StateEnum Id => StateEnum.Idle;

        public void OnEnter(Player t)
        {
            isFirstStop = true;
            cutTreeFrameCount = Constants.WAIT_CUT_TREE_FRAMES;
            if (LevelManager.Ins.IsFirstLevel) return;
            sleepTimer ??= TimerManager.Inst.PopSTimer();
            sleepTimer.Start(Constants.SLEEP_TIME, ChangeSleepState);
            return;

            void ChangeSleepState()
            {
                t.StateMachine.ChangeState(StateEnum.Sleep);
            }
        }

        public void OnExecute(Player t)
        {
            //NOTE:Checking for IdleState            
            if (t.IsStun)
            {
                t.StateMachine.ChangeState(StateEnum.Stun);
                return;
            }
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
            //NOTE: Checking for riding raft or something like that
            if (t.HasVehicle() && !t.isRideVehicle)
                if (t.CanRideVehicle(t.Direction))
                {
                    t.OnRideVehicle(t.Direction);
                    return;
                }
            t.MovingData.SetData(t.Direction);
            if (!t.conditionMergeOnMoving.IsApplicable(t.MovingData))
            {
                //NOTE: Checking to push an dynamic object
                if (t.MovingData.blockDynamicUnits.Count > 0)
                    t.StateMachine.ChangeState(StateEnum.Push);               
                else if (!_isChangeAnim) t.ChangeAnim(Constants.IDLE_ANIM);

                //NOTE: Checking to push an static object
                if (t.MovingData.blockStaticUnits.Count > 0)
                {
                    switch (t.MovingData.blockStaticUnits[0])
                    {
                        case StaticUnit.Tree tree:
                            if (isFirstStop)
                            {
                                //NOTE: Checking button down of player input
                                switch (t.InputDetection.InputAction)
                                {
                                    case InputAction.ButtonDown:
                                        isFirstStop = false;
                                        break;
                                    case InputAction.ButtonHold:
                                        if(cutTreeFrameCount <= 0)
                                        {
                                            isFirstStop = false;
                                        }
                                        else
                                            cutTreeFrameCount--;
                                        break;
                                }
                                if (!_isChangeAnim) t.ChangeAnim(Constants.IDLE_ANIM);
                            }
                            else
                            {
                                tree.OnCutTree(t);
                            }
                            break;
                        // case ChestFinalPoint chestFinalPoint:
                        // case Rock rock:
                        default:
                            switch (t.InputDetection.InputAction)
                            {
                                case InputAction.ButtonDown:
                                    t.StateMachine.ChangeState(StateEnum.Push);
                                    break;
                            }
                            break;
                    }
                }

                switch (t.MovingData.Condition)
                {
                    case CONDITION.SIT_DOWN:
                        if(t.InputDetection.InputAction == InputAction.ButtonDown && !t.IsInputCache)
                            t.StateMachine.ChangeState(StateEnum.SitDown);
                        break;
                    case CONDITION.RUN_ABOVE_CHUMP:
                        t.StateMachine.ChangeState(StateEnum.RunAboveChump);
                        break;
                    default:
                        break;
                }
                return;
            }

            hasTreeRoot = false;
            if (t.MovingData.blockStaticUnits.Count == 1) 
            {
                switch (t.MovingData.blockStaticUnits[0])
                {
                    case TreeRoot treeRoot:
                        hasTreeRoot = true;
                        treeRoot.UpHeight(t, t.MovingData.inputDirection);
                        break;
                }               
            }
            
            t.SetEnterCellData(t.MovingData.inputDirection, t.MovingData.enterMainCell, t.UnitTypeY);
            t.SetIslandId(t.MovingData.enterMainCell, out bool isChangeIsland);
            
            // NOTE: Checking if player reach to the bank of the island (MinX or MaxX)
            // If reach, Change Camera target pos to the Pos (MinX or MaxX, 0, CenterPos.z) of the island
            // If not, Change Camera target pos to the center of the island
            if (!isChangeIsland)
            {
                if (t.MovingData.enterMainCell.IslandID != -1)
                {
                    Island island = LevelManager.Ins.CurrentLevel.Islands[t.MovingData.enterMainCell.IslandID];
                    t.SetUpCamera(island, t.MovingData.enterMainCell);
                }
                else
                {
                    Island island = LevelManager.Ins.CurrentLevel.Islands[t.islandID];
                    t.SetUpCamera(island, t.MovingData.enterMainCell);
                }
            }
            
            t.OnOutCells();
            t.OnEnterCells(t.MovingData.enterMainCell, t.MovingData.enterCells);
            if (hasTreeRoot) 
                t.StateMachine.ChangeState(StateEnum.JumpUp);
            else
                t.StateMachine.ChangeState(t.EnterPosData.isFalling ? StateEnum.JumpDown : StateEnum.Move);
        }

        public void OnExit(Player t)
        {
            _isChangeAnim = false;
            sleepTimer?.Stop();
        }
        
    }
}
