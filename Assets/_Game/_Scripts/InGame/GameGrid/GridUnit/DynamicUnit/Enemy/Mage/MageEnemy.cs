using _Game._Scripts.Managers;
using _Game.DesignPattern.StateMachine;
using _Game.GameGrid.Unit.DynamicUnit.Enemy.EnemyStates;
using _Game.GameGrid.Unit.DynamicUnit.Interface;
using _Game.Managers;
using GameGridEnum;
using _Game.Utilities;

namespace _Game.GameGrid.Unit.DynamicUnit.Enemy
{
    public class MageEnemy : GridUnitCharacter, IEnemy
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
            AddToLevelManager();
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
            IsDead = true;
            stateMachine.ChangeState(StateEnum.Die);
        }

        public void AddToLevelManager()
        {
            LevelManager.Ins.OnAddEnemy(this);
        }

        public void RemoveFromLevelManager()
        {
            LevelManager.Ins.OnRemoveEnemy(this);
        }
        
        // protected override void OnMementoRestoreData()
        // {
        //     AddToLevelManager();
        // }
    }
}