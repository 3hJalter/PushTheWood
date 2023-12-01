using _Game.DesignPattern;
using _Game.GameGrid.Unit.DynamicUnit;
using UnityEngine;

namespace _Game.GameGrid.GridSurface
{
    public class TutorialGroundSurface : GridSurface
    {
        private bool _isTutorialDone;
        public override void OnUnitEnter(Unit.GridUnit gridUnit)
        {
            if (_isTutorialDone) return;
            if (gridUnit is not PlayerUnit) return;
            _isTutorialDone = true;
            LevelManager.Ins.OnShowTutorial();
        }

        public override void OnDespawn()
        {
            SimplePool.Release(this);
            Destroy(gameObject);
        }
    }
}
