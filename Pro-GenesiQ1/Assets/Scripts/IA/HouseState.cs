using UnityEngine;

public class HouseState : IState
{
    private readonly Rigidbody agent;
    public House house;
    private Transform houseTransform;
    private const float speed = 10;
    private float houseTime;
    private bool isOnHouse;
    public bool IsComplete { get; set; }
    
    public HouseState(Rigidbody agent)
    {
        this.agent = agent;
    }

    public void OnEnter()
    {
        houseTransform = house.transform;
    }

    public void OnUpdate()
    {
        if (IsComplete) return;
        
        if (Mathf.Abs(agent.transform.position.x - houseTransform.position.x) < 1f && Mathf.Abs(agent.transform.position.z - houseTransform.position.z) < 1f)
        {
            if (!isOnHouse)
            {
                isOnHouse = true;
                house.SetIsComplete();
            }
            
            agent.linearVelocity = Vector3.zero + new Vector3(0, agent.linearVelocity.y, 0);
        }
        else
        {
            agent.transform.LookAt(houseTransform.position);
            agent.transform.localEulerAngles = new Vector3(0, agent.transform.localEulerAngles.y, 0);
            agent.linearVelocity = agent.transform.forward * speed + new Vector3(0, agent.linearVelocity.y, 0);
        }
    }

    public void OnExit()
    {
        isOnHouse = false;
        IsComplete = false;
    }
}
