
public class StateMachine
{

    IState currentState;

    public StateMachine(IState state)
    {
        currentState = state;
        currentState.Enter();
    }

    public void ChangeState(IState state)
    {
        currentState.Exit();
        currentState = state;
        currentState.Enter();
    }

    public void LogicUpdate()
    {
        currentState.LogicUpdate();
    }

    public void PhysicsUpdate()
    {
        currentState.PhysicsUpdate();
    }

}

