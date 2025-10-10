using UnityEngine;

public class StateMachine : MonoBehaviour
{
    [HideInInspector] public Rigidbody agent;
    private IState currentState;
    [HideInInspector] public IdleState idleState;

    private void Start()
    {
        agent = GetComponent<Rigidbody>();
        idleState = new IdleState(agent);
        ChangeState(idleState);
    }

    private void Update()
    {
        currentState?.OnUpdate();
        if (currentState is { IsComplete: true })
        {
            ChangeState(idleState);
        }
    }

    public void ChangeState(IState newState)
    {
        if (newState == currentState) return;

        currentState?.OnExit();
        currentState = newState;
        currentState?.OnEnter();
    }
}
