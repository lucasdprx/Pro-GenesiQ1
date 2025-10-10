using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public class SelectorAbility : Ability
{
    private GameObject currentSelected;
    [SerializeField] private DecalProjector decalProjector;
    public override void Capacity()
    {
        if (!hit.transform) return;

        Transform hitTransform = hit.transform;
        TerrainCollider terrainCollider = hitTransform.GetComponent<TerrainCollider>();
        CollectItem collectable = hitTransform.GetComponent<CollectItem>();
        StateMachine human = currentSelected?.GetComponent<StateMachine>();

        if (human)
        {
            human.ChangeState(collectable ? new CollectState(collectable.transform, human.agent, collectable) : human.idleState);
            ResetSelection();
            return;
        }

        if (!terrainCollider)
        {
            ActivateDecal(hitTransform.gameObject);
        }
    }

    private void ResetSelection()
    {
        currentSelected = null;
        if (decalProjector)
        {
            decalProjector.gameObject.SetActive(false);
            decalProjector.transform.SetParent(null);
        }
    }

    private void ActivateDecal(GameObject target)
    {
        if (decalProjector)
        {
            decalProjector.gameObject.SetActive(true);
            decalProjector.transform.SetParent(target.transform);
            decalProjector.transform.localPosition = Vector3.zero;
            decalProjector.transform.localRotation = Quaternion.identity;
            decalProjector.size = new Vector3(2, 2, 2);
        }
        currentSelected = target;
    }

    private void OnDisable()
    {
        ResetSelection();
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
