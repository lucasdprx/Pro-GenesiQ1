using UnityEngine;
using UnityEngine.AI;

public class StateMachine : MonoBehaviour
{
    [HideInInspector] public NavMeshAgent agent;
    private IState currentState;
    private IdleState idleState;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        idleState = new IdleState(agent);
        ChangeState(idleState);
    }

    private void Update()
    {
        currentState?.OnUpdate();
    }

    private void ChangeState(IState newState)
    {
        if (newState == currentState) return;

        currentState?.OnExit();
        currentState = newState;
        currentState?.OnEnter();
    }
}
