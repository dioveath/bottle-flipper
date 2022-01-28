using UnityEngine;

public class BottleIdleState : BottleState
{


    public BottleIdleState(Bottle bottle) : base(bottle)
    {
    }


    public override void Enter()
    {
        Debug.Log("Idle State Enter!");
    }

    public override void Exit()
    {
        Debug.Log("Idle State Exit!");
    }

    public override void LogicUpdate()
    {
        Debug.Log("Idle Logic Update!");
    }

    public override void PhysicsUpdate()
    {
        Debug.Log("Idle Physics Update!");
    }

}
