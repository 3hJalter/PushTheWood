using _Game._Scripts.Managers;
using _Game.DesignPattern.StateMachine;
using _Game.GameGrid.Unit.DynamicUnit.Interface;
using _Game.GameGrid.Unit.DynamicUnit.Player.PlayerState;
using _Game.Managers;
using DG.Tweening;
using GameGridEnum;
using HControls;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace _Game.GameGrid.Unit.DynamicUnit.Enemy
{
    public class ArcherEnemy : GridUnitDynamic
    {
        [SerializeField] private Animator animator;

        private StateMachine<ArcherEnemy> stateMachine;
        public StateMachine<ArcherEnemy> StateMachine => stateMachine;
        public override StateEnum CurrentStateId
        {
            get => StateEnum.Idle;
            set
            {
                stateMachine.ChangeState(value);
            }
        }

        private string _currentAnim = Constants.INIT_ANIM;
        private bool _isAddState;


        private bool _isWaitAFrame;

        public Direction Direction { get; private set; } = Direction.None;
        public float AnimSpeed => animator.speed;

        private void FixedUpdate()
        {
            if (!GameManager.Ins.IsState(GameState.InGame)) return;
            if (_isWaitAFrame)
            {
                _isWaitAFrame = false;
                Direction = Direction.None;
                // TEST: Reset the Input if Direction is not none and Move is Swipe (Swipe only take one input per swipe)
                if (Direction != Direction.None && MoveInputManager.Ins.CurrentChoice is MoveInputManager.MoveChoice.Swipe) HInputManager.SetDefault();
                stateMachine?.UpdateState();
                return;
            }

            _isWaitAFrame = true;
            //stateMachine.Debug = true;
            stateMachine?.UpdateState();
        }
        public override void OnInit(GameGridCell mainCellIn, HeightLevel startHeightIn = HeightLevel.One,
            bool isUseInitData = true, Direction skinDirection = Direction.None, bool hasSetPosAndRot = false)
        {
            base.OnInit(mainCellIn, startHeightIn, isUseInitData, skinDirection, hasSetPosAndRot);
            if (!_isAddState)
            {
                _isAddState = true;
                stateMachine = new StateMachine<ArcherEnemy>(this);
                AddState();
            }
            //stateMachine.Debug = true;
            stateMachine.ChangeState(StateEnum.Idle);
        }
        private void AddState()
        {

        }
        public void ChangeAnim(string animName, bool forceAnim = false)
        {
            if (!forceAnim)
                if (_currentAnim.Equals(animName))
                    return;
            animator.ResetTrigger(_currentAnim);
            _currentAnim = animName;
            animator.SetTrigger(_currentAnim);
        }
        public void SetAnimSpeed(float speed)
        {
            animator.speed = speed;
        }
        public bool IsCurrentAnimDone()
        {
            return animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1;
        }
        public void LookDirection(Direction directionIn)
        {
            if (directionIn is Direction.None) return;
            skin.DOLookAt(Tf.position + Constants.DirVector3[directionIn], 0.2f, AxisConstraint.Y, Vector3.up);
        }
    }
}