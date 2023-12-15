using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Game.GameGrid.Unit.StaticUnit
{
    public class ChestFinalPoint : GridUnitStatic
    {
        [SerializeField] private Animator chestAnimator;
        [SerializeField] private GameObject chestModel;
        protected override void OnInteractBtnClick()
        {
            ShowAnim(true);
            LevelManager.Ins.OnWin();
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
            if (isShow) chestAnimator.SetTrigger(Constants.OPEN_ANIM);
        }
        
    }
}
