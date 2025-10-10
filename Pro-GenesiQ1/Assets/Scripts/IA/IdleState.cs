using UnityEngine;

public class IdleState : IState
{
    private readonly Rigidbody agent;
    private float idleTime;
    private Vector3 target;
    private const float speed = 5;
    public bool IsComplete { get; set; }
    
    public IdleState(Rigidbody agent)
    {
        this.agent = agent;
    }
    public void OnEnter()
    {
        idleTime = 0f;
        target = agent.transform.position + GetRandomVector(30f);
        agent.transform.LookAt(target);
        agent.transform.localEulerAngles = new Vector3(0, agent.transform.localEulerAngles.y, 0);
    }

    public void OnUpdate()
    {
        if (Mathf.Abs(agent.transform.position.x - target.x) < 0.5f && Mathf.Abs(agent.transform.position.z - target.z) < 0.5f)
        {
            agent.linearVelocity = Vector3.zero + new Vector3(0, agent.linearVelocity.y, 0);
            idleTime += Time.deltaTime;
            if (idleTime >= 3f)
            {
                idleTime = 0f;
                target = agent.transform.position + GetRandomVector(30f);
                agent.transform.LookAt(target);
                agent.transform.localEulerAngles = new Vector3(0, agent.transform.localEulerAngles.y, 0);
            }
        }
        else
        {
            agent.linearVelocity = agent.transform.forward * speed + new Vector3(0, agent.linearVelocity.y, 0);
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
