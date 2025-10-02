using UnityEngine;

public class ObjectPosition : MonoBehaviour
{
    [SerializeField] private Transform startPoint;
    
    private Transform objectTransform;
    private void Awake()
    {
        TerrainModifier.OnTerrainModified += RefreshPosition;
        objectTransform = transform;
    }

    private void RefreshPosition()
    {
        if (Physics.Raycast(startPoint.position, -objectTransform.up, out RaycastHit hit,
                Mathf.Infinity,LayerMask.GetMask("Terrain")))
        {
            objectTransform.position = hit.point;
            objectTransform.up = hit.normal;
        }
    }
}