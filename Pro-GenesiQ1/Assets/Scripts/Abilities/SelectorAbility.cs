using UnityEngine;

public class SelectorAbility : Ability
{
    private GameObject currentSelected;
    public override void Capacity()
    {
        if (!hit.transform) return;
        
        TerrainCollider terrainCollider = hit.transform.GetComponent<TerrainCollider>();
        CollectItem collectable = hit.transform.GetComponent<CollectItem>();
        StateMachine human = currentSelected?.GetComponent<StateMachine>();
        
        if (human)
        {
            if (collectable)
            {
                human.ChangeState(new CollectState(collectable.transform, human.agent, collectable));
            }
            else
            {
                human.agent.SetDestination(hit.point);
            }
            currentSelected = null;
            return;
        }
        
        if (!terrainCollider)
            currentSelected = hit.transform.gameObject;
    }

    private void OnDisable()
    {
        currentSelected = null;
    }
    
    public void DeleteSelected()
    {
        if (currentSelected != null)
        {
            Destroy(currentSelected);
            currentSelected = null;
        }
    }
}
