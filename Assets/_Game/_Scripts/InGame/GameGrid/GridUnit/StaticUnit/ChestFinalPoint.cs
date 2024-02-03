using System;
using _Game.GameGrid.Unit.DynamicUnit.Player;
using _Game.GameGrid.Unit.StaticUnit.Interface;
using DG.Tweening;
using UnityEngine;

namespace _Game.GameGrid.Unit.StaticUnit
{
    public class ChestFinalPoint : GridUnitStatic, IChest
    {
        [SerializeField] private Animator chestAnimator;
        [SerializeField] private GameObject chestModel;
        private bool isInteracted = false;

        public override void OnInteract()
        {
            ShowAnim(true);
            DOVirtual.DelayedCall(Constants.CHEST_OPEN_TIME, OnOpenChestComplete);
        }

        public override void OnBePushed(Direction direction = Direction.None, GridUnit pushUnit = null)
        {
            if (isInteracted) return;
            base.OnBePushed(direction, pushUnit);
            if (pushUnit is not Player) return;
            isInteracted = true;
            ShowAnim(true);
            DOVirtual.DelayedCall(1f, OnOpenChestComplete);
        }

        public override void OnDespawn()
        {
            ShowAnim(false);
            isInteracted = false;
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

        public void OnOpenChestComplete()
        {
            LevelManager.Ins.OnWin();
        }
    }
}
