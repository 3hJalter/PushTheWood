using _Game.DesignPattern.StateMachine;
using _Game.GameGrid.Unit.StaticUnit;
using DG.Tweening;
using GameGridEnum;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

namespace _Game.GameGrid.Unit.DynamicUnit.Chump.ChumpState
{
    public class TurnOverChumpState : IState<Chump>
    {
        private bool _isTurnOver;

        public void OnEnter(Chump t)
        {
            t.TurnOverData.SetData(t.ChumpBeInteractedData.inputDirection);
            _isTurnOver = t.ConditionMergeOnBePushed.IsApplicable(t.TurnOverData);
            OnExecute(t);
        }

        public void OnExecute(Chump t)
        {
            if (!_isTurnOver)
            {
                if (t.TurnOverData.blockDynamicUnits.Count > 0) t.OnPush(t.TurnOverData.inputDirection, t.TurnOverData);
                t.ChangeState(StateEnum.RollBlock);
                //Play Anim Block
            }
            else
            {
                bool hasTreeRoot = false;
                if (t.TurnOverData.blockStaticUnits.Count == 1 &&
                    t.TurnOverData.blockStaticUnits[0] is TreeRoot tr)
                {
                    // Temporary for only tree root
                    tr.UpHeight(t, t.TurnOverData.inputDirection);
                    hasTreeRoot = true;
                }
                #region Handle Cell Data

                t.anchor.ChangeAnchorPos(t, t.TurnOverData.inputDirection);
                t.Size = t.TurnOverData.nextSize;
                Vector3 skinOffset = t.TurnOverData.enterMainCell.WorldPos - t.MainCell.WorldPos;
                UnitTypeY switchType = t.UnitTypeY == UnitTypeY.Up ? UnitTypeY.Down : UnitTypeY.Up;
                t.SetEnterCellData(t.TurnOverData.inputDirection, t.TurnOverData.enterMainCell, switchType, false,
                    t.TurnOverData.enterCells);
                t.OnOutCells();
                bool isFallToWater = false;
                if (t.UnitTypeY is UnitTypeY.Up ||
                    (t.TurnOverData.enterMainCell.SurfaceType is not GridSurfaceType.Water &&
                     t.UnitTypeY is UnitTypeY.Down))
                    t.SwitchType(t.TurnOverData.inputDirection);
                else if (t.TurnOverData.enterMainCell.SurfaceType is GridSurfaceType.Water 
                         &&  t.TurnOverData.enterMainCell.GetGridUnitAtHeight(Constants.DirFirstHeightOfSurface[GridSurfaceType.Water]) is not null)
                    t.SwitchType(t.TurnOverData.inputDirection);
                else 
                    isFallToWater = true;
                t.OnEnterCells(t.TurnOverData.enterMainCell, t.TurnOverData.enterCells);

                #endregion

                #region Animation and complete

                Vector3 axis = Vector3.Cross(Vector3.up, Constants.DirVector3[t.TurnOverData.inputDirection]);
                // Roll 90 degree around anchor in 0.18 second using Tween
                float lastAngle = 0;
                if (hasTreeRoot)
                {
                    Vector3 position = t.Tf.position;
                    t.Tf.DOMove(new Vector3(position.x, t.EnterPosData.finalPos.y, position.z), Constants.MOVING_TIME).SetEase(Ease.Linear)
                        .SetUpdate(UpdateType.Fixed);
                }
                DOVirtual.Float(0, 90, Constants.MOVING_TIME, i =>
                {
                    t.skin.RotateAround(t.anchor.Tf.position, axis, i - lastAngle);
                    lastAngle = i;
                }).SetUpdate(UpdateType.Fixed).SetEase(Ease.Linear).OnComplete(() =>
                {
                    t.skin.localPosition -= skinOffset;
                    t.Tf.position = t.EnterPosData.initialPos;
                    if (!t.EnterPosData.isFalling)
                    {
                        t.OnEnterTrigger(t);
                        if (isFallToWater)
                        {
                            // Can be change to animation later
                            t.skin.localRotation =
                                Quaternion.Euler(t.UnitTypeXZ is UnitTypeXZ.Horizontal
                                    ? Constants.HorizontalSkinRotation
                                    : Constants.VerticalSkinRotation);
                            // minus position offsetY
                            t.Tf.position -= Vector3.up * t.yOffsetOnDown;
                        }

                        t.ChangeState(StateEnum.Idle);
                    }
                    else
                    {
                        // Tween to final position
                        t.Tf.DOMove(t.EnterPosData.finalPos, Constants.MOVING_TIME).SetEase(Ease.Linear)
                            .SetUpdate(UpdateType.Fixed).OnComplete(() =>
                            {
                                t.OnEnterTrigger(t);
                                if (isFallToWater)
                                {
                                    // Can be change to animation later
                                    t.skin.localRotation =
                                        Quaternion.Euler(t.UnitTypeXZ is UnitTypeXZ.Horizontal
                                            ? Constants.HorizontalSkinRotation
                                            : Constants.VerticalSkinRotation);
                                    // minus position offsetY
                                    t.Tf.position -= Vector3.up * t.yOffsetOnDown;
                                }

                                t.ChangeState(StateEnum.Idle);
                            });
                    }
                });

                #endregion

            }
        }

        public void OnExit(Chump t)
        {

        }
    }
}
