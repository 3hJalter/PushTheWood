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
