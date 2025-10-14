using UnityEngine;

public class StateMachine : MonoBehaviour
{
    public GameObject humanPrefab;
    [HideInInspector] public Rigidbody agent;
    private IState currentState;
    [HideInInspector] public IdleState idleState;
    private HouseState houseState;
    private float houseTimer;
    private readonly Collider[] results = new Collider[10];

    private void Awake()
    {
        agent = GetComponent<Rigidbody>();
        idleState = new IdleState(agent);
        houseState = new HouseState(agent);
        ChangeState(idleState);
    }

    private void Update()
    {
        currentState?.OnUpdate();
        if (currentState is { IsComplete: true })
        {
            ChangeState(idleState);
        }

        if (currentState is HouseState) return;
        
        houseTimer += Time.deltaTime;
        if (houseTimer >= 2)
        {
            int size = Physics.OverlapSphereNonAlloc(transform.position, 15, results, 1 << LayerMask.NameToLayer("House"));
            if (size > 0)
            {
                House house = results[0].GetComponent<House>();
                if (house.AddHuman(this))
                {
                    houseState.house = house;
                    ChangeState(houseState);
                }
            }
            houseTimer = 0;
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
