using System;
using Calendar.Scripts.Services;
using Calendar.Scripts.Structure.StateMachine.States;
using Game.Calendar.Scripts.Services;

namespace Calendar.Scripts.Structure.StateMachine.GameStateMachine
{
    public interface IStateMachine : IGlobalService
    {
        IExitableState ActiveState { get; }
        void Enter<TState>() where TState : class, IState;
        void Enter<TState, TPayload>(TPayload payload) where TState : class, IPayloadedState<TPayload>;

        void Enter<TState, TPayload, TPayload2>(TPayload payload, TPayload2 payload2)
            where TState : class, IPayloadedState<TPayload, TPayload2>;
        
        void Enter<TState, TPayload, TPayload2, TPayload3>(TPayload payload, TPayload2 payload2, TPayload3 payload3)
            where TState : class, IPayloadedState<TPayload, TPayload2, TPayload3>;

        bool IsActive<TState>() where TState : class, IState;
        void AddState<TState>(TState instance) where TState : class, IState;
        void AddState<TState, TPayload>(TState instance) where TState : class, IPayloadedState<TPayload>;
        void AddState(Type type, IExitableState instance);
    }
}