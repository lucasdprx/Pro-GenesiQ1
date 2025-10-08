using UnityEngine;

public class GameObjectPainter : Ability
{
    [Header("References")]
    [SerializeField] private GameObject objectPainter;
    [SerializeField] private GameObject objectPainterTransient;
    
    public bool rotateWithNormal;
    
    private GameObject transientObject;

    protected override void Awake()
    {
        base.Awake();
        if (objectPainterTransient is not null)
        {
            transientObject = Instantiate(objectPainterTransient);
        }
    }

    protected override void Update()
    {
        base.Update();
        if (transientObject is null || hit.collider is null) return;
        transientObject.transform.position = hit.point;
        if (rotateWithNormal)
            transientObject.transform.up = hit.normal;
    }

    public override void Capacity()
    {
        if (objectPainter is null || !hit.collider) return;
        
        GameObject newObject = Instantiate(objectPainter, hit.point, Quaternion.identity);
        if (rotateWithNormal)
            newObject.transform.up = hit.normal;
    }

    private void OnDisable()
    {
        transientObject.SetActive(false);
    }
    private void OnEnable()
    {
        transientObject.SetActive(true);
    }
}
