using System.Collections;
using System.Collections.Generic;
using _Game._Scripts.InGame;
using _Game._Scripts.Managers;
using UnityEngine;
namespace _Game.GameGrid.Unit.DynamicUnit.Box.BoxState
{
    using _Game._Scripts.InGame.GameCondition.Data;
    using _Game.DesignPattern.StateMachine;
    using _Game.Managers;
    using _Game.Utilities.Timer;
    using DG.Tweening;

    public class MoveBlockBoxState : IState<Box>
    {
        List<GridUnit> blockObjects = new();
        Direction blockDirection;
        Tween moveTween;

        public StateEnum Id => StateEnum.Block;

        public void OnEnter(Box t)
        {
            blockObjects.Clear();
            GameplayManager.Ins.IsCanUndo = false;
            OnExecute(t);
        }

        public void OnExecute(Box t)
        {
            GetBlockObjects(t.MovingData);
            blockDirection = t.MovingData.inputDirection;
            
            Vector3 originPos = t.Tf.position;
            moveTween = t.Tf.DOShakePosition(Constants.MOVING_TIME * 0.8f, 0.1f, 60, 0, false, true, ShakeRandomnessMode.Harmonic).SetEase(Ease.InQuad)
                .OnComplete(ChangeToIdle);

            if (blockObjects.Count > 0)
                TimerManager.Ins.WaitForTime(Constants.MOVING_TIME * 0.5f, ObjectBlocking);

            void GetBlockObjects(MovingData data)
            {
                blockObjects.AddRange(data.blockStaticUnits);
                blockObjects.AddRange(data.blockDynamicUnits);
            }
            void ObjectBlocking()
            {
                for (int i = 0; i < blockObjects.Count; i++)
                {
                    blockObjects[i].OnBePushed(blockDirection, t);
                }
                //NOTE: Checking if push dynamic object does not create any change in grid -> discard the newest save.
                if (!LevelManager.Ins.CurrentLevel.GridMap.IsChange)
                {
                    LevelManager.Ins.DiscardSaveState();
                }
                else
                {
                    #region Push Hint Step Handler

                    if (t.BeInteractedData.pushUnit is Player.Player p)
                    {
                        //NOTE: Saving when push dynamic object that make grid change
                        if (LevelManager.Ins.IsSavePlayerPushStep)
                        {
                            GameplayManager.Ins.SavePushHint.SaveStep(p.MainCell.X, p.MainCell.Y, (int) p.LastPushedDirection, p.islandID);        
                        }
                        EventGlobalManager.Ins.OnPlayerPushStep?.Dispatch(new PlayerStep{x = p.MainCell.X, y = p.MainCell.Y, d = (int) p.LastPushedDirection, i = p.islandID});
                    }

                    #endregion
                }
            }

            void ChangeToIdle()
            {
                t.StateMachine.ChangeState(StateEnum.Idle);
            }
        }

        public void OnExit(Box t)
        {
            GameplayManager.Ins.IsCanUndo = true;
            moveTween?.Kill();
        }

    }
}