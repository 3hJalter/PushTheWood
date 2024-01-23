using GameGridEnum;
using UnityEngine;

namespace _Game.GameGrid.Unit.StaticUnit
{
    public class ButtonUnit : GridUnitStatic
    {
        [SerializeField] private Animator animator;
        private string _currentAnimName = " ";

        // public override void OnInit(GameGridCell mainCellIn, HeightLevel startHeightIn = HeightLevel.One, bool isUseInitData = true,
        //     Direction skinDirection = Direction.None, bool hasSetPosAndRot = false)
        // {
        //     // Reduce the height of the button by 1 -> Default height on Ground is 1, so we need to reduce it to 0.5
        //     base.OnInit(mainCellIn, startHeightIn - 1, isUseInitData, skinDirection, hasSetPosAndRot);
        // }

        protected override void OnEnterTriggerUpper(GridUnit triggerUnit)
        {
            ChangeAnim(Constants.ENTER_ANIM);
            // TODO: Button Logic
        }
        
        protected override void OnOutTriggerUpper(GridUnit triggerUnit)
        {
            ChangeAnim(Constants.EXIT_ANIM);
            // TODO: Button Logic
        }
        
        private void ChangeAnim(string animName)
        {
            if (_currentAnimName == animName) return;
            _currentAnimName = animName;
            animator.SetTrigger(_currentAnimName);
        }
    }
}
