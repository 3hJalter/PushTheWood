using UnityEngine;

namespace Game.AI
{
    public abstract class BaseState<D>
    {
        protected const int TRUE = 1;
        protected const int FALSE = 0;
        protected const int RECEIVED_FRAME_TIME = 1;
        protected D Data;
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
