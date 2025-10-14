using UnityEngine;

public class House : MonoBehaviour
{
    public StateMachine human1;
    public StateMachine human2;

    public bool isComplete1;
    public bool isComplete2;

    public void ResetHuman()
    {
        human1 = null;
        human2 = null;
        isComplete1 = false;
        isComplete2 = false;
    }

    public bool AddHuman(StateMachine human)
    {
        if (!human1)
        {
            human1 = human;
            return true;
        }
        if (!human2)
        {
            human2 = human;
            return true;
        }
        
        return false;
    }

    public void SetIsComplete()
    {
        if (isComplete1)
        {
            isComplete2 = true;
            Instantiate(human2.humanPrefab, transform.position, Quaternion.identity);
            human1.ChangeState(human1.idleState);
            human2.ChangeState(human2.idleState);
            ResetHuman();
        }
        else
        {
            isComplete1 = true;
        }
    }

    public bool IsReady()
    {
        return isComplete1 && isComplete2;
    }
}
