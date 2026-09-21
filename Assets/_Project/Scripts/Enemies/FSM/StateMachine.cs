public sealed class StateMachine
{
    private IState currentState;

    public IState CurrentState => currentState;

    public void SetState(IState newState)
    {
        if (newState == null)
        {
            return;
        }

        currentState?.Exit();

        currentState = newState;
        currentState.Enter();
    }

    public void Tick()
    {
        currentState?.Tick();
    }
}