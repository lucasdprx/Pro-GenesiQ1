using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Terrain))]
public class SimpleTerrainGenerator : MonoBehaviour
{
    [SerializeField] private Terrain terrain;

    [Header("Taille du terrain (unités)")]
    [SerializeField, Range(32, 1024)] private int width = 256;
    [SerializeField, Range(32, 1024)] private int depth = 256;
    [SerializeField, Range(1f, 500f)] private float maxHeight = 80f;

    [Header("Bruit (simple)")]
    [SerializeField, Range(1f, 500f)] private float scale = 50f;
    [SerializeField, Range(1, 6)] private int octaves = 3;
    [SerializeField, Range(0f, 1f)] private float persistence = 0.5f;
    [SerializeField, Range(1f, 4f)] private float lacunarity = 2f;
    [SerializeField] private int seed = 0;
    [SerializeField] private bool randomSeed = true;

    private void Awake()
    {
        if (randomSeed)
        {
            seed = Random.Range(int.MinValue, int.MaxValue);
        }
        
        if (!terrain) terrain = GetComponent<Terrain>();
        if (!terrain)
        {
            Debug.LogError("No terrain found in terrain generator.");
            return;
        }

        GenerateTerrain();
    }

    [ContextMenu("Generate Simple Terrain")]
    public void GenerateTerrain()
    {
        terrain.terrainData = Instantiate(terrain.terrainData);
        TerrainData td = terrain.terrainData;
        terrain.GetComponent<TerrainCollider>().terrainData = td;

        int side = Mathf.Max(width, depth);
        int resolution = Mathf.Clamp(Mathf.NextPowerOfTwo(side) + 1, 33, 4097);

        td.heightmapResolution = resolution;
        td.size = new Vector3(width, maxHeight, depth);

        float[,] heights = new float[resolution, resolution];

        System.Random prng = new(seed);
        float offsetX = prng.Next(-10000, 10000);
        float offsetY = prng.Next(-10000, 10000);

        for (int y = 0; y < resolution; y++)
        {
            for (int x = 0; x < resolution; x++)
            {
                float nx = (float)x / (resolution - 1);
                float ny = (float)y / (resolution - 1);

                float sampleX = (nx * width + offsetX) / scale;
                float sampleY = (ny * depth + offsetY) / scale;

                float value = 0f;
                float amplitude = 1f;
                float frequency = 1f;
                float amplitudeSum = 0f;

                for (int o = 0; o < octaves; o++)
                {
                    float perlin = Mathf.PerlinNoise(sampleX * frequency, sampleY * frequency) * 2f - 1f;
                    value += perlin * amplitude;

                    amplitudeSum += amplitude;
                    amplitude *= persistence;
                    frequency *= lacunarity;
                }

                value /= Mathf.Max(1e-6f, amplitudeSum);
                heights[y, x] = Mathf.InverseLerp(-1f, 1f, value);
            }
        }

        td.SetHeights(0, 0, heights);
    }
}