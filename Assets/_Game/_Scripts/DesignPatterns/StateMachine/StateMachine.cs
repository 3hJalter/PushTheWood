using _Game.GameGrid.Unit;
using _Game.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace _Game.DesignPattern.StateMachine
{
    public class StateMachine<T> where T : GridUnit
    {
        Dictionary<StateEnum, IState<T>> states;
        IState<T> currentState;
        public IState<T> CurrentState => currentState;
        T main;
        public StateEnum OverrideState;
        public bool Debug = false;
        public StateMachine(T main)
        {
            states = new Dictionary<StateEnum, IState<T>> ();
            this.main = main;
            OverrideState = StateEnum.None;
        }
        public void AddState(StateEnum id,  IState<T> state)
        {
            if(states.ContainsKey(id))
            {
                states[id] = state;
            }
            else
            {
                states.Add(id, state);
            }
        }
        public void RemoveState(StateEnum id)
        {
            if (states.ContainsKey(id))
            {
                states[id] = null;
            }
        }
        public void ChangeState(StateEnum id)
        {
            if (OverrideState != StateEnum.None && OverrideState != id) return;
            if (Debug)
            {
                DevLog.Log(DevId.Hung, $"{currentState?.Id} -> {id}");
            }
            currentState?.OnExit(main);
            currentState = states[id];
            currentState.OnEnter(main);
        }
        public void UpdateState()
        {
            currentState.OnExecute(main);
        }
    }
}