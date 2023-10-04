using System.Collections.Generic;
using UnityEngine;

namespace Game.AI
{
    public enum STATE
    {
        NONE = -1,
        TREE_UP = 0,
        TREE_DOWN = 1,
        TREE_WATER = 2
    }

    public class StateMachine<T, D> where D : BaseState<T>
    {
        private readonly Dictionary<STATE, D> states = new();
        public bool Report = false;

        //DEVELOP:Change condition to "If Name = null -> State = null"
        public D CurrentState { get; private set; }
        public STATE CurrentStateName { get; private set; }
        public bool IsStarted { get; private set; }

        public void Start(D initState)
        {
            if (CurrentState != null) CurrentState.Exit();

            CurrentState = initState;
            CurrentState.Enter();
            IsStarted = true;
        }

        public void ChangeState(D newState)
        {
            if (newState != null)
            {
                CurrentState.Exit();
                CurrentState = newState;
                CurrentState.Enter();
            }
        }

        public void Start(STATE state)
        {
            Start(states[state]);
            CurrentStateName = state;

            if (Report) Debug.Log("StateMachine_Start --> " + state);
        }

        public void ChangeState(STATE state)
        {
            if (states.ContainsKey(state) && states[state] != null)
            {
                ChangeState(states[state]);
                if (Report) Debug.Log(CurrentStateName + " --> " + state);
                CurrentStateName = state;
            }
            else
            {
                Debug.LogError("NUll STATE:" + state);
            }
        }

        public void Stop()
        {
            if (CurrentState != null) CurrentState.Exit();
            CurrentState = null;
            CurrentStateName = STATE.NONE;
            IsStarted = false;
        }

        public D GetState(STATE name)
        {
            return states[name];
        }

        #region Modify States Data

        public void PushState(STATE state, D stateScript)
        {
            if (states.ContainsKey(state))
                return;
            states.Add(state, stateScript);
        }

        public void ChangeState(STATE state, D stateScript)
        {
            if (!states.ContainsKey(state))
                PushState(state, stateScript);
            else
                states[state] = stateScript;
        }

        #endregion
    }
}
