using System.Collections.Generic;
using _Game.DesignPattern.StateMachine;
using _Game.GameGrid.Unit.DynamicUnit.ButtonUnitState;
using _Game.Utilities;
using DG.Tweening;
using GameGridEnum;
using UnityEngine;

namespace _Game.GameGrid.Unit.DynamicUnit
{
    public class ButtonUnit : GridUnitDynamic
    {
        private StateMachine<ButtonUnit> StateMachine { get; set; }
        [SerializeField] private Transform btnModelTransform;

        public Transform BtnModelTransform => btnModelTransform;

        [SerializeField] private List<MeshRenderer> btnMeshRenderer;
        [SerializeField] private Material btnOffMaterial;
        [SerializeField] private Material btnOnMaterial;

        private bool _isAddState;
        public Tween animTween;
        
        
        public override void OnInit(GameGridCell mainCellIn, HeightLevel startHeightIn = HeightLevel.One, bool isUseInitData = true,
            Direction skinDirection = Direction.None, bool hasSetPosAndRos = false)
        {
            base.OnInit(mainCellIn, startHeightIn, isUseInitData, skinDirection, hasSetPosAndRos);
            if (!_isAddState)
            {
                _isAddState = true;
                StateMachine = new StateMachine<ButtonUnit>(this);
                AddState();
            }
            StateMachine.ChangeState(StateEnum.Idle);
        }
        
        public void ChangeButton(bool isOn)
        {
            for (int index = 0; index < btnMeshRenderer.Count; index++)
            {
                MeshRenderer meshRenderer = btnMeshRenderer[index];
                meshRenderer.material = isOn ? btnOnMaterial : btnOffMaterial;
            }
        }

        private void AddState()
        {
            StateMachine.AddState(StateEnum.Idle, new IdleButtonState());
            StateMachine.AddState(StateEnum.Enter, new EnterButtonState());
        }
        
        protected override void OnEnterTriggerUpper(GridUnit triggerUnit)
        {
            if (CurrentStateId == StateEnum.Enter) return;
            StateMachine.ChangeState(StateEnum.Enter);
            DevLog.Log(DevId.Hoang, "Enter Button at time: " + Time.time);
        }
        
        protected override void OnOutTriggerUpper(GridUnit triggerUnit)
        {
            if (CurrentStateId == StateEnum.Idle) return;
            StateMachine.ChangeState(StateEnum.Idle);
            DevLog.Log(DevId.Hoang, "Exit Button at time: " + Time.time);
        }

        public override StateEnum CurrentStateId 
        { 
            get => StateMachine?.CurrentStateId ?? StateEnum.Idle;
            set => StateMachine.ChangeState(value);
        }
    }
}
