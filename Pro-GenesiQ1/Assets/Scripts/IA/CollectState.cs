using UnityEngine;

public class CollectState : IState
{
    private readonly Transform target;
    private readonly Rigidbody agent;
    private float collectTime;
    public bool IsComplete { get; set; }
    private CollectItem collectItem;
    private const float speed = 5f;

    public CollectState(Transform target, Rigidbody agent, CollectItem collectItem)
    {
        this.target = target;
        this.agent = agent;
        this.collectItem = collectItem;
    }
    public void OnEnter()
    {
        agent.transform.LookAt(target);
        agent.transform.localEulerAngles = new Vector3(0, agent.transform.localEulerAngles.y, 0);
    }

    public void OnUpdate()
    {
        if (Vector3.Distance(agent.position, target.position) < 1.5f)
        {
            agent.linearVelocity = Vector3.zero + new Vector3(0, agent.linearVelocity.y, 0);
            collectTime += Time.deltaTime;
            if (collectTime >= 2f)
            {
                PlayerResources.Instance.AddResource(collectItem.resourceType, collectItem.amount);
                IsComplete = true;
            }
        }
        else
        {
            agent.linearVelocity = agent.transform.forward * speed + new Vector3(0, agent.linearVelocity.y, 0);
        }
    }

    public void OnExit()
    {
        collectTime = 0f;
        collectItem = null;
    }

}
