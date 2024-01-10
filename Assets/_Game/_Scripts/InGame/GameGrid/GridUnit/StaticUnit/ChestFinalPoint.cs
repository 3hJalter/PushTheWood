using System;
using _Game.GameGrid.Unit.DynamicUnit.Player;
using DG.Tweening;
using UnityEngine;

namespace _Game.GameGrid.Unit.StaticUnit
{
    public class ChestFinalPoint : GridUnitStatic
    {
        [SerializeField] private Animator chestAnimator;
        [SerializeField] private GameObject chestModel;

        public override void OnInteract()
        {
            ShowAnim(true);
            DOVirtual.DelayedCall(1f, () => LevelManager.Ins.OnWin());
        }

        public override void OnBePushed(Direction direction = Direction.None, GridUnit pushUnit = null)
        {
            base.OnBePushed(direction, pushUnit);
            // if direction is not invert with skinRotateDirection, return
            // Forward: Forward, Back: Back, Left: Right, Right: Left (Forward and Back different because of GetRotationAngle in BuildDingUnitDatabaseSO.cs)
            switch (direction)
            {
                case Direction.Forward:
                    if (skinRotationDirection != Direction.Forward && skinRotationDirection != Direction.None) return;
                    break;
                case Direction.Back:
                    if (skinRotationDirection != Direction.Back) return;
                    break;
                case Direction.Left:
                    if (skinRotationDirection != Direction.Right) return;
                    break;
                case Direction.Right:
                    if (skinRotationDirection != Direction.Left) return;
                    break;
                case Direction.None:
                default:
                    return;
            }
            
            if (pushUnit is not Player) return;
            ShowAnim(true);
            DOVirtual.DelayedCall(1f, () => LevelManager.Ins.OnWin());
        }

        public override void OnDespawn()
        {
            ShowAnim(false);
            base.OnDespawn();
        }

        private void ShowAnim(bool isShow)
        {
            chestAnimator.gameObject.SetActive(isShow);
            chestModel.SetActive(!isShow);
            if (isShow)
            {
                chestAnimator.SetTrigger(Constants.OPEN_ANIM);
                btnCanvas.gameObject.SetActive(false);
            }
        }
    }
}
