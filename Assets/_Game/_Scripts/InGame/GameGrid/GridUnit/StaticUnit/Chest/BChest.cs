using _Game.GameGrid.Unit.DynamicUnit.Player;
using _Game.GameGrid.Unit.StaticUnit.Interface;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Game.GameGrid.Unit.StaticUnit.Chest
{
    public abstract class BChest : GridUnitStatic, IChest
    {
        [Title("Chest")]
        [SerializeField] protected Animator chestAnimator;
        [SerializeField] protected GameObject chestModel;
        
        protected bool isInteracted;

        public virtual void OnOpenChestComplete()
        { }
        
        protected void ShowAnim(bool isShow)
        {
            chestAnimator.gameObject.SetActive(isShow);
            chestModel.SetActive(!isShow);
            if (!isShow) return;
            chestAnimator.SetTrigger(Constants.OPEN_ANIM);
        }

        protected override void OnEnterTriggerNeighbor(GridUnit triggerUnit)
        {
            if (isInteracted) return;
            if (triggerUnit is not Player) return;
            isInteracted = true;
            ShowAnim(true);
            DOVirtual.DelayedCall(Constants.CHEST_OPEN_TIME, OnOpenChestComplete);        }
    }
}
