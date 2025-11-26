namespace Calendar.Scripts.Structure.StateMachine.States
{
    public interface IState : IExitableState
    {
        void Enter();
    }

    public interface IPayloadedState<TPayload> : IExitableState
    {
        void Enter(TPayload payload);
    }
    
    public interface IPayloadedState<TPayload,TPayload2 > : IExitableState
    {
        void Enter(TPayload payload, TPayload2 payload2 );
    }
    
    public interface IPayloadedState<TPayload,TPayload2,TPayload3 > : IExitableState
    {
        void Enter(TPayload payload, TPayload2 payload2, TPayload3 payload3);
    }

    public interface IExitableState
    {
        void Exit();
    }
}