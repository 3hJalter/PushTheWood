using _Game.Camera;
using _Game.GameGrid.Unit.DynamicUnit.Player;
using GameGridEnum;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Game.GameGrid.Unit
{
    public abstract class GridUnitStatic : GridUnit
    {
        [SerializeField] protected GridUnitStaticType gridUnitStaticType;
        public GridUnitStaticType GridUnitStaticType => gridUnitStaticType;

        [SerializeField] private bool hasInteractBtn;
        
        [ShowIf(nameof(hasInteractBtn))]
        [SerializeField] protected Canvas btnCanvas;
        [ShowIf(nameof(hasInteractBtn))]
        [SerializeField] private HButton interactBtn;

        private bool _isFirstInitDone;
        
        public override void OnInit(GameGridCell mainCellIn, HeightLevel startHeightIn = HeightLevel.One, bool isUseInitData = true,
            Direction skinDirection = Direction.None)
        {
            base.OnInit(mainCellIn, startHeightIn, isUseInitData, skinDirection);
            if (!hasInteractBtn || _isFirstInitDone || btnCanvas is null) return;
            _isFirstInitDone = true;
            btnCanvas.worldCamera = CameraFollow.Ins.MainCamera;
            interactBtn.onClick.AddListener(OnInteractBtnClick);
        }
        
        protected virtual void OnInteractBtnClick()
        {
            
        }
        
        protected override void OnEnterTriggerNeighbor(GridUnit triggerUnit)
        {
            if (hasInteractBtn && triggerUnit is Player)
            {
                btnCanvas.gameObject.SetActive(true);
            }
        }

        protected override void OnOutTriggerNeighbor(GridUnit triggerUnit)
        {
            if (hasInteractBtn && triggerUnit is Player)
            {
                btnCanvas.gameObject.SetActive(false);
            }
        }

        public override void OnDespawn()
        {
            if (hasInteractBtn) btnCanvas.gameObject.SetActive(false);
            base.OnDespawn();
        }
    }
}
