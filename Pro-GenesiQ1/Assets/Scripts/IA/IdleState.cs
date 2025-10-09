using UnityEngine;
using UnityEngine.AI;

public class IdleState : IState
{
    private readonly NavMeshAgent agent;
    private float idleTime;
    public bool IsComplete { get; set; }
    
    public IdleState(NavMeshAgent agent)
    {
        this.agent = agent;
    }
    public void OnEnter()
    {
        idleTime = 0f;
        agent.SetDestination(agent.transform.position + GetRandomVector(10f));
    }

    public void OnUpdate()
    {
        if (agent.remainingDistance <= 0.5f)
        {
            idleTime += Time.deltaTime;
            if (idleTime >= 3f)
            {
                idleTime = 0f;
                agent.SetDestination(agent.transform.position + GetRandomVector(10f));
            }
        }
    }

    public void OnExit()
    {
        
    }


    private Vector3 GetRandomVector(float scale)
    {
        return new Vector3(Random.Range(-scale, scale), 0, Random.Range(-scale, scale));
    }
}
