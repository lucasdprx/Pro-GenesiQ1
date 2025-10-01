using UnityEngine;

public static class CamExtension
{
    public static RaycastHit GetTerrainHit(this Camera cam)
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        return Physics.Raycast(ray, out RaycastHit hit) ? hit : default;
    }
}
