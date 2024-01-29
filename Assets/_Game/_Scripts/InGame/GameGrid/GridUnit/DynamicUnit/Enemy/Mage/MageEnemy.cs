using _Game.DesignPattern.StateMachine;
using _Game.GameGrid.Unit.DynamicUnit.Enemy.EnemyStates;
using _Game.GameGrid.Unit.DynamicUnit.Interface;
using _Game.Managers;
using DG.Tweening;
using GameGridEnum;
using _Game.Utilities;
using UnityEngine;
using System.Collections.Generic;

namespace _Game.GameGrid.Unit.DynamicUnit.Enemy
{
    public class MageEnemy : GridUnitCharacter
    {
        private StateMachine<MageEnemy> stateMachine;
        public StateMachine<MageEnemy> StateMachine => stateMachine;
        public override StateEnum CurrentStateId
        {
            get => StateEnum.Idle;
            set => stateMachine.ChangeState(value);
        }

        private void FixedUpdate()
        {
            if (!GameManager.Ins.IsState(GameState.InGame)) return;
            if (_isWaitAFrame)
            {
                _isWaitAFrame = false;
                //Direction = Direction.None;
                // TEST: Reset the Input if Direction is not none and Move is Swipe (Swipe only take one input per swipe)
                stateMachine?.UpdateState();
                return;
            }

            _isWaitAFrame = true;
            stateMachine?.UpdateState();
        }
        public override void OnInit(GameGridCell mainCellIn, HeightLevel startHeightIn = HeightLevel.One,
            bool isUseInitData = false, Direction skinDirection = Direction.None, bool hasSetPosAndRot = false)
        {
            base.OnInit(mainCellIn, startHeightIn, isUseInitData, skinDirection, hasSetPosAndRot); //DEV: Not use init data
            if (!_isAddState)
            {
                _isAddState = true;
                stateMachine = new StateMachine<MageEnemy>(this);
                AddState();
            }
            IsDead = false;
            Direction = Direction.None;
            stateMachine.ChangeState(StateEnum.Idle);
        }
        public override void OnBePushed(Direction direction = Direction.None, GridUnit pushUnit = null)
        {
            if (pushUnit is Player.Player player)
            {
                LookDirection(Constants.InvDirection[direction]);
                stateMachine.ChangeState(StateEnum.Idle);
                player.CheckingStunState();
                return;
            }
            IsDead = true;
        }
        protected override void AddState()
        {
            stateMachine.AddState(StateEnum.Idle, new IdleMageEnemyState());
            stateMachine.AddState(StateEnum.Attack, new AttackMageEnemyState());
            stateMachine.AddState(StateEnum.Die, new DieMageEnemyState());
        }
        public override void OnCharacterDie()
        {
            DevLog.Log(DevId.Hoang, "TODO: Mage Die Logic");
            IsDead = true;
            stateMachine.ChangeState(StateEnum.Die);
        }
    }
}