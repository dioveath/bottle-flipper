
public abstract class BottleState : IState
{

    protected Bottle _bottle;

    public BottleState(Bottle bottle)
    {
        this._bottle = bottle;
    }

    public abstract void Enter();
    public abstract void Exit();
    public abstract void LogicUpdate();
    public abstract void PhysicsUpdate();

}
