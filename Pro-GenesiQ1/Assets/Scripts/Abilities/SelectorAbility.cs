using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public class SelectorAbility : Ability
{
    private GameObject currentSelected;
    [SerializeField] private DecalProjector decalProjector;
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
                human.ChangeState(human.idleState);
            }
            currentSelected = null;
            if (decalProjector)
            {
                decalProjector.gameObject.SetActive(false);
                decalProjector.transform.SetParent(null);
            }
            return;
        }

        if (!terrainCollider)
        {
            if (decalProjector)
                decalProjector.gameObject.SetActive(true);
            currentSelected = hit.transform.gameObject;
            decalProjector.transform.SetParent(currentSelected.transform);
            decalProjector.transform.localPosition = Vector3.zero;
            decalProjector.size = new Vector3(2, 2, 2);
        }
    }

    private void OnDisable()
    {
        currentSelected = null;
        if (decalProjector)
        {
            decalProjector.gameObject.SetActive(false);
            decalProjector.transform.SetParent(null);
        }
    }
    
    public void DeleteSelected()
    {
        if (decalProjector)
        {
            decalProjector.gameObject.SetActive(false);
            decalProjector.transform.SetParent(null);
        }
        if (currentSelected != null)
        {
            Destroy(currentSelected);
            currentSelected = null;
        }
    }
}
