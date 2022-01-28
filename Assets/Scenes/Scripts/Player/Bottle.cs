using UnityEngine;

public class Bottle : MonoBehaviour
{

    StateMachine bottleStateMachine;

    void Start()
    {
        bottleStateMachine = new StateMachine(new BottleIdleState(this));
    }

    void Update()
    {
        bottleStateMachine.LogicUpdate();
    }

    void FixedUpdate()
    {
        bottleStateMachine.PhysicsUpdate();
    }

}
