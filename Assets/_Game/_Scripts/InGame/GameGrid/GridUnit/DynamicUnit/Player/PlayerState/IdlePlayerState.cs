using _Game._Scripts.InGame;
using _Game.DesignPattern.StateMachine;
using _Game.GameGrid.Unit.StaticUnit;
using _Game.Managers;
using _Game.Utilities.Timer;
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
                                tree.OnInteract();
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
                    SetUpCamera(island, t);
                }
                else
                {
                    Island island = LevelManager.Ins.CurrentLevel.Islands[t.islandID];
                    SetUpCamera(island, t);
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
        
        
        private const int OUT_OF_ISLAND_CELL_BEFORE_TARGET_CAM_TO_PLAYER = 0;
        private const float OFFSET = 3f;
        private const float CAMERA_DOWN_OFFSET = Constants.CELL_SIZE * Constants.DOWN_CAMERA_CELL_OFFSET;
        private const float MOVE_TIME = 0.25f;
        private static readonly Vector3 CameraDownOffsetVector = new(0, 0, CAMERA_DOWN_OFFSET);

        private static void SetUpCamera(Island island, Player t)
        {
            // TRICK: Take offset = 3 to make camera not really see the edge of the island
            
            float x = t.MovingData.enterMainCell.WorldPos.x;
            // NOTE: If the island is small (Size.x < 7), the camera will not change target pos
            if (island.isSmallIsland) return;
            
            // If the player is out of the island 2 cell, the camera will change target to Player
            if (island.minXIslandPos.x - x > Constants.CELL_SIZE * OUT_OF_ISLAND_CELL_BEFORE_TARGET_CAM_TO_PLAYER ||
                x - island.maxXIslandPos.x > Constants.CELL_SIZE * OUT_OF_ISLAND_CELL_BEFORE_TARGET_CAM_TO_PLAYER)
            {
                CameraManager.Ins.ChangeCameraTargetPosition(t.MovingData.enterMainCell.WorldPos + CameraDownOffsetVector, MOVE_TIME);  
                return;
            }
            if (Mathf.Abs(x - island.minXIslandPos.x) < 0.1f) // if minX
            {
                CameraManager.Ins.ChangeCameraTargetPosition(new Vector3(island.minXIslandPos.x + OFFSET, 0, island.centerIslandPos.z + CAMERA_DOWN_OFFSET), MOVE_TIME);
            }
            else if (Mathf.Abs(x - island.maxXIslandPos.x) < 0.1f) // if maxX
            {
                CameraManager.Ins.ChangeCameraTargetPosition(new Vector3(island.maxXIslandPos.x - OFFSET, 0, island.centerIslandPos.z + CAMERA_DOWN_OFFSET), MOVE_TIME);
            }
            else 
            {
                // Change the Camera to center if the player is come from bank and the distance is 1 Cell
                if (Mathf.Abs(x - island.centerIslandPos.x) < Constants.CELL_SIZE)
                {
                    CameraManager.Ins.ChangeCameraTargetPosition(island.centerIslandPos + CameraDownOffsetVector, MOVE_TIME);  
                }
            }
        }
    }
}
