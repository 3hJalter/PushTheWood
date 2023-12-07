using System;
using System.Collections.Generic;
using _Game.DesignPattern;
using _Game.DesignPattern.StateMachine;
using _Game.GameGrid.Unit.DynamicUnit.PlayerState;
using _Game.GameRule.RuleEngine;
using UnityEngine;

namespace _Game.GameGrid.Unit.DynamicUnit
{
    public class NPlayerUnit : GridUnitDynamic
    {
        public Direction direction = Direction.None;
        public IVehicle vehicle;
        [SerializeField] private Animator animator;
        private string _currentAnim = Constants.INIT_ANIM;
        private IState<NPlayerUnit> _currentState;
        private Dictionary<StateEnum, IState<NPlayerUnit>> _states;

        #region RULE
        public RuleEngine movingRuleEngine;
        public RuleMovingData RuleMovingData => _ruleMovingData ??= new RuleMovingData(this);
        private RuleMovingData _ruleMovingData;
        #endregion
        
        private void Start()
        {
            OnInitState(StateEnum.Idle);
        }

        private void FixedUpdate()
        {
            _currentState.OnExecute(this);
        }

        public void ChangeAnim(string animName)
        {
            if (_currentAnim == animName) return;
            _currentAnim = animName;
            animator.Play(animName);
        }

        public void OnInteract(GridUnit blockUnit)
        {
            // TODO: the blockUnit need to has a method OnBeInteract   
        }
        
        public bool IsAnimDone(float percent = 1)
        {
            return animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= percent;
        }
        
        public void ChangeState(StateEnum stateEnum)
        {
            _currentState.OnExit(this);
            _currentState = _states[stateEnum];
            _currentState.OnEnter(this);
        }
        
        private void OnInitState(StateEnum beginState)
        {
            _states = new Dictionary<StateEnum, IState<NPlayerUnit>>
            {
                { StateEnum.Idle, new IdleState() },
                { StateEnum.Walk, new WalkState() },
                { StateEnum.JumpDown, new JumpDownState() },
                { StateEnum.JumpUp, new JumpUpState() },
                { StateEnum.Interact, new InteractState() },
                { StateEnum.Push, new PushState() },
                { StateEnum.CutTree, new CutTreeState() },
                { StateEnum.Die, new DieState() },
                { StateEnum.Happy, new HappyState() }
            };
            _currentState = _states[beginState];
            _currentState.OnEnter(this);
        }
    }
}
