using _Game.DesignPattern;
using _Game.DesignPattern.StateMachine;
using _Game.GameGrid.Unit.DynamicUnit.Enemy;
using _Game.GameGrid.Unit.DynamicUnit.Interface;
using _Game.Utilities;
using _Game.Utilities.Grid;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace _Game.GameGrid.Unit
{
    public abstract class GridUnitCharacter : GridUnitDynamic, ICharacter 
    {
        [SerializeField]
        protected Animator animator;
        [HideInInspector]
        public bool IsDead = false;
        protected string _currentAnim = Constants.INIT_ANIM;
        protected bool _isAddState;
        protected bool _isWaitAFrame;
        [HideInInspector]
        public readonly List<GameGridCell> AttackRange = new List<GameGridCell>();
        public readonly List<DangerIndicator> AttackRangeVFX = new List<DangerIndicator>();
        
        public Direction Direction { get; protected set; } = Direction.None;
        public float AnimSpeed => animator.speed;
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

        protected void LookDirection(Direction directionIn)
        {
            if (directionIn is Direction.None) return;
            skinRotationDirection = directionIn;
            skin.DOLookAt(Tf.position + Constants.DirVector3[directionIn], 0.2f, AxisConstraint.Y, Vector3.up);
        }
        protected abstract void AddState();
       

        #region SAVING DATA
        public override IMemento Save()
        {
            IMemento save;
            if (overrideSpawnSave != null)
            {
                save = overrideSpawnSave;
                overrideSpawnSave = null;
            }
            else
            {
                save = new CharacterUnitMemento<GridUnitCharacter>(this, IsDead, _currentAnim, CurrentStateId, isSpawn, Tf.position, skin.rotation, startHeight, endHeight
                , unitTypeY, unitTypeXZ, belowUnits, neighborUnits, upperUnits, mainCell, cellInUnits, islandID, lastPushedDirection);
            }
            return save;
        }

        public abstract void OnCharacterDie();
        public override void OnDespawn()
        {
            base.OnDespawn();
            foreach(GameGridCell cell in AttackRange)
            {
                cell.Data.IsBlockDanger = false;
                cell.Data.IsDanger = false;
            }
            AttackRange.Clear();
            foreach(DangerIndicator indicator in AttackRangeVFX)
            {
                SimplePool.Despawn(indicator);
            }
            AttackRangeVFX.Clear();
        }

        public class CharacterUnitMemento<T> : DynamicUnitMemento<T> where T : GridUnitCharacter
        {
            protected bool isDead;
            protected string currentAnim;
            public CharacterUnitMemento(GridUnitCharacter main, bool isDead, string currentAnim, StateEnum currentState, params object[] data) : base((T)main, currentState, data)
            {
                this.isDead = isDead;
                this.currentAnim = currentAnim;
            }

            public override void Restore()
            {
                base.Restore();
                main.IsDead = isDead;
                main.ChangeAnim(currentAnim, true);
            }
        }
        #endregion
    }
}