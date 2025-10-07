using UnityEngine;

public class PaintTexture : Ability
{
    public int textureIndex = 1;
    public float brushSize = 10f;
    public float strength = 1f;
    public AnimationCurve brushFalloff = AnimationCurve.EaseInOut(0, 1, 1, 0);

    [SerializeField] private Terrain terrain;
    [SerializeField] private TerrainModifier terrainModifier;
    private TerrainData terrainData;

    private void Start()
    {
        terrainData = terrainModifier.terrainData;
        terrain.terrainData = terrainData;
    }
    
    public void SetTextureIndex(int index)
    {
        textureIndex = index;
    }

    public override void Capacity()
    {
        base.Capacity();
        Vector3 terrainPos = hit.point - terrain.transform.position;

        int mapX = (int)((terrainPos.x / terrainData.size.x) * terrainData.alphamapWidth);
        int mapZ = (int)((terrainPos.z / terrainData.size.z) * terrainData.alphamapHeight);

        int brushSizeInAlphamaps = Mathf.RoundToInt((brushSize / terrainData.size.x) * terrainData.alphamapWidth);

        int offsetX = Mathf.Clamp(mapX - brushSizeInAlphamaps / 2, 0, terrainData.alphamapWidth - brushSizeInAlphamaps);
        int offsetZ = Mathf.Clamp(mapZ - brushSizeInAlphamaps / 2, 0, terrainData.alphamapHeight - brushSizeInAlphamaps);

        float[,,] alphamaps = terrainData.GetAlphamaps(offsetX, offsetZ, brushSizeInAlphamaps, brushSizeInAlphamaps);

        for (int x = 0; x < brushSizeInAlphamaps; x++)
        {
            for (int z = 0; z < brushSizeInAlphamaps; z++)
            {
                float dx = (x - brushSizeInAlphamaps / 2f) / (brushSizeInAlphamaps / 2f);
                float dz = (z - brushSizeInAlphamaps / 2f) / (brushSizeInAlphamaps / 2f);
                float dist = Mathf.Sqrt(dx * dx + dz * dz);

                if (dist <= 1f)
                {
                    float falloff = brushFalloff.Evaluate(dist);
                    float paintStrength = strength * falloff;

                    float total = 0f;
                    for (int i = 0; i < terrainData.alphamapLayers; i++)
                    {
                        if (i == textureIndex)
                            alphamaps[z, x, i] = Mathf.Clamp01(alphamaps[z, x, i] + paintStrength);
                        else
                            alphamaps[z, x, i] = Mathf.Clamp01(alphamaps[z, x, i] * (1 - paintStrength));

                        total += alphamaps[z, x, i];
                    }

                    for (int i = 0; i < terrainData.alphamapLayers; i++)
                        alphamaps[z, x, i] /= total;
                }
            }
        }

        terrainData.SetAlphamaps(offsetX, offsetZ, alphamaps);
    }
}
