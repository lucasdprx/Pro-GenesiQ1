using UnityEngine;

public class Ability : MonoBehaviour
{
    public bool inputHold;
    protected RaycastHit hit;
    private Camera cam;

    protected virtual void Awake()
    {
        cam = Camera.main;
    }

    protected virtual void Update()
    {
        hit = cam.GetTerrainHit();
    }

    public virtual void Capacity() {}
}
