using System;
using System.Collections.Generic;
using Calendar.Scripts.Structure.StateMachine.States;
using UnityEngine;

namespace Calendar.Scripts.Structure.StateMachine.GameStateMachine
{
    public class StateMachine : IStateMachine
    {
        private readonly IDictionary<Type, IExitableState> _states = new Dictionary<Type, IExitableState>(10);
        private IExitableState _activeState;
        
        public IExitableState ActiveState => _activeState;

        public void Enter<TState>() where TState : class, IState
        {
            Debug.Log($"Changing state to : {typeof(TState)}");
            ChangeState<TState>().Enter();
        }

        public void Enter<TState, TPayload>(TPayload payload) 
            where TState : class, IPayloadedState<TPayload> =>
            ChangeState<TState>().Enter(payload);

        public void Enter<TState, TPayload, TPayload2>(TPayload payload, TPayload2 payload2)
            where TState : class, IPayloadedState<TPayload, TPayload2>
        {
            ChangeState<TState>().Enter(payload, payload2);
        }

        public void Enter<TState, TPayload, TPayload2, TPayload3>(TPayload payload, TPayload2 payload2, TPayload3 payload3) where TState : class, IPayloadedState<TPayload, TPayload2, TPayload3>
        {
            ChangeState<TState>().Enter(payload, payload2, payload3);
        }

        public bool IsActive<TState>() where TState : class, IState
        {
            TState state = GetState<TState>();
            return _activeState == state;
        }

        public void AddState<TState>(TState instance) where TState : class, IState =>
            _states.Add(typeof(TState), instance);

        public void AddState<TState, TPayload>(TState instance) where TState : class, IPayloadedState<TPayload> =>
            _states.Add(typeof(TState), instance);

        public void AddState(Type type, IExitableState instance) => _states.Add(type, instance);

        private TState ChangeState<TState>() where TState : class, IExitableState
        {
            _activeState?.Exit();
            Debug.Log($"Exit {_activeState}");
            TState state = GetState<TState>();
            _activeState = state;
            return state;
        }

        private TState GetState<TState>() where TState : class, IExitableState =>
            _states[typeof(TState)] as TState;

        ~StateMachine() => _activeState.Exit();
    }
}