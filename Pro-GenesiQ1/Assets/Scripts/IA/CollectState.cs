using UnityEngine;
using UnityEngine.AI;

public class CollectState : IState
{
    private readonly Transform target;
    private readonly NavMeshAgent agent;
    private float collectTime;
    
    public CollectState(Transform target, NavMeshAgent agent)
    {
        this.target = target;
        this.agent = agent;
    }
    public void OnEnter()
    {
        agent.ResetPath();
        agent.SetDestination(target.position);
    }

    public void OnUpdate()
    {
        if (agent.remainingDistance <= 1)
        {
            agent.ResetPath();
            collectTime += Time.deltaTime;
            if (collectTime >= 2f)
            {
                Debug.Log("Item collectemd!");
            }
        }
    }

    public void OnExit()
    {
        throw new System.NotImplementedException();
    }
}
