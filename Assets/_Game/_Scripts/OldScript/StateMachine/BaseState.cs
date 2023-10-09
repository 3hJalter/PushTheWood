using UnityEngine;

namespace _Game._Scripts.DesignPattern
{
    public abstract class BaseState<TStateHolder>
    {
        protected const int TRUE = 1;
        protected const int FALSE = 0;
        protected const int RECEIVED_FRAME_TIME = 1;
        protected TStateHolder stateHolder;
        protected bool isEndState = true;
        protected float StartTime { get; private set; }

        public virtual void Enter()
        {
            StartTime = Time.time;
            isEndState = false;
        }

        public virtual void Exit()
        {
            isEndState = true;
        }

        public virtual int LogicUpdate()
        {
            return TRUE;
        }

        public virtual int PhysicUpdate()
        {
            return TRUE;
        }
    }
}
