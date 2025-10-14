using UnityEngine;

public class GameObjectPainter : Ability
{
    [Header("References")]
    [SerializeField] private GameObject objectPainter;
    [SerializeField] private GameObject objectPainterTransient;
    [SerializeField] private ResourceType resourceType;
    [SerializeField] private int resourceCost = 1;
    
    public bool rotateWithNormal;
    
    private GameObject transientObject;
    private Transform cameraTransform;

    protected override void Awake()
    {
        base.Awake();
        cameraTransform = Camera.main?.transform;
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
        transientObject.transform.eulerAngles = new Vector3(transientObject.transform.eulerAngles.x, cameraTransform.eulerAngles.y, transientObject.transform.eulerAngles.z);
    }

    public override void Capacity()
    {
        if (objectPainter is null || !hit.collider) return;

        if (!PlayerResources.Instance.SpendResource(resourceType, resourceCost)) return;
        
        GameObject newObject = Instantiate(objectPainter, hit.point, Quaternion.identity);
        newObject.transform.eulerAngles = transientObject.transform.eulerAngles;
    }

    private void OnDisable()
    {
        if (transientObject)
            transientObject.SetActive(false);
    }
    private void OnEnable()
    {
        transientObject.SetActive(true);
    }
}
