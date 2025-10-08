using UnityEngine;

namespace TerrainSystem
{
    public class NavmeshBaker : MonoBehaviour
    {
        [SerializeField] private NavTileBaker tileBaker;

        private void Start()
        {
            if (!tileBaker) tileBaker = GetComponent<NavTileBaker>();
            if (!tileBaker)
            {
                Debug.LogError("No NavTileBaker found on NavmeshBaker GameObject.");
                return;
            }
            TerrainModifier.OnTerrainModified += OnTerrainModified;
        }

        private void OnTerrainModified(Vector3 point, float radius)
        {
            if (!tileBaker) return;
            tileBaker.NotifyTerrainChanged(point, radius);
        }

        private void OnDestroy()
        {
            TerrainModifier.OnTerrainModified -= OnTerrainModified;
        }
    }
}