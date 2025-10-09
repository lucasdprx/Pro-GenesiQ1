using UnityEngine;
using UnityEngine.AI;

public class CollectState : IState
{
    private readonly Transform target;
    private readonly NavMeshAgent agent;
    private float collectTime;
    public bool IsComplete { get; set; }
    private CollectItem collectItem;
    
    public CollectState(Transform target, NavMeshAgent agent, CollectItem collectItem)
    {
        this.target = target;
        this.agent = agent;
        this.collectItem = collectItem;
    }
    public void OnEnter()
    {
        agent.ResetPath();
        agent.SetDestination(target.position);
    }

    public void OnUpdate()
    {
        if (agent.remainingDistance <= 1.5f && !IsComplete)
        {
            agent.ResetPath();
            collectTime += Time.deltaTime;
            if (collectTime >= 2f)
            {
                PlayerResources.Instance.AddResource(collectItem.resourceType, collectItem.amount);
                IsComplete = true;
            }
        }
    }

    public void OnExit()
    {
        collectTime = 0f;
        collectItem = null;
    }

}
