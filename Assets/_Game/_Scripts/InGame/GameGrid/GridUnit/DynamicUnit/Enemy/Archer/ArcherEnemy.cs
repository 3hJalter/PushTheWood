using _Game._Scripts.Managers;
using _Game.DesignPattern.StateMachine;
using _Game.GameGrid.Unit.DynamicUnit.Enemy.EnemyStates;
using _Game.GameGrid.Unit.DynamicUnit.Interface;
using _Game.Managers;
using DG.Tweening;
using GameGridEnum;
using _Game.Utilities;
using UnityEngine;

namespace _Game.GameGrid.Unit.DynamicUnit.Enemy
{
    public class ArcherEnemy : GridUnitCharacter, IEnemy
    {
        protected StateMachine<ArcherEnemy> stateMachine;
        public StateMachine<ArcherEnemy> StateMachine => stateMachine;
        public override StateEnum CurrentStateId
        {
            get => StateEnum.Idle;
            set => stateMachine.ChangeState(value);
        }
    
        private void FixedUpdate()
        {
            if (!GameManager.Ins.IsState(GameState.InGame)) return;
            stateMachine?.UpdateState();
        }
        public override void OnInit(GameGridCell mainCellIn, HeightLevel startHeightIn = HeightLevel.One,
            bool isUseInitData = false, Direction skinDirection = Direction.None, bool hasSetPosAndRot = false)
        {
            base.OnInit(mainCellIn, startHeightIn, isUseInitData, skinDirection, hasSetPosAndRot); //DEV: Not use init data
            SaveInitData(LevelManager.Ins.CurrentLevel.Index);
            if (!_isAddState)
            {
                _isAddState = true;
                stateMachine = new StateMachine<ArcherEnemy>(this);
                AddState();
            }
            IsDead = false;
            Direction = Direction.None;
            AddToLevelManager();
            stateMachine.ChangeState(StateEnum.Idle);
        }
        public override void OnBePushed(Direction direction = Direction.None, GridUnit pushUnit = null)
        {
            LookDirection(Constants.InvDirection[direction]);
            Direction = direction;

            if (pushUnit is Player.Player player)
            {
                stateMachine.ChangeState(StateEnum.Idle);
                player.CheckingStunState();
                return;
            }
        }
        protected override void AddState()
        {
            stateMachine.AddState(StateEnum.Idle, new IdleArcherEnemyState());
            stateMachine.AddState(StateEnum.Attack, new AttackArcherEnemyState());
            stateMachine.AddState(StateEnum.Die, new DieArcherEnemyState());
        }   
        public override void OnCharacterDie()
        {
            DevLog.Log(DevId.Hoang, "TODO: Character Die Logic");
            IsDead = true;
            stateMachine.ChangeState(StateEnum.Die);
        }

        public override void OnDespawn()
        {
            RemoveFromLevelManager();
            base.OnDespawn();
        }

        public void AddToLevelManager()
        {
            LevelManager.Ins.enemies.Add(this);
            LevelManager.Ins.objectiveCounter++;
        }

        public void RemoveFromLevelManager()
        {
            LevelManager.Ins.enemies.Remove(this);
            EventGlobalManager.Ins.OnEnemyDie.Dispatch();
        }
    }
}