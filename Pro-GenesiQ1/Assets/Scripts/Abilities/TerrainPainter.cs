using System;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;
public class TerrainPainter : Ability
{
    [Header("References")]
    [SerializeField] private Terrain terrain;
    [SerializeField] private DecalProjector projector;
    
    [Header("Brush Settings")]
    [Range(2, 100)]
    [SerializeField] private int brushSize = 16;

    [Range(0.01f, 1f)]
    [SerializeField] private float strength = 0.01f;
    [SerializeField] private KeyCode lowerKey = KeyCode.LeftShift;

    private TerrainData terrainData;
    private int heightResolution;

    public static event Action OnTerrainModified;

    protected override void Awake()
    {
        base.Awake();
        terrainData = Instantiate(terrain.terrainData);
        terrain.terrainData = terrainData;
        heightResolution = terrainData.heightmapResolution;

        terrain.GetComponent<TerrainCollider>().terrainData = terrainData;
    }


    protected override void Update()
    {
        base.Update();
        UpdateSizeProjector();
    }

    private void ScrollSize(Vector2 input)
    {
        if (Mathf.Abs(input.y) > 0.01f)
        {
            brushSize += (int)input.y;
            brushSize = Mathf.Clamp(brushSize, 2, 100);
        }
    }
    private void UpdateSizeProjector()
    {
        if (hit.collider is null || !projector) return;
        
        projector.transform.position = hit.point + Vector3.up * 5;
        float brushWorldSize = brushSize * terrainData.size.x / heightResolution;
        projector.size = new Vector3(brushWorldSize, 200f, brushWorldSize);
    }
    public override void Capacity()
    {
        Vector3 localPos = hit.point - terrain.transform.position;

        float normX = localPos.x / terrainData.size.x;
        float normZ = localPos.z / terrainData.size.z;

        int px = Mathf.RoundToInt(normX * (heightResolution - 1));
        int pz = Mathf.RoundToInt(normZ * (heightResolution - 1));

        int halfSize = brushSize / 2;

        int xStart = Mathf.Clamp(px - halfSize, 0, heightResolution - 1);
        int zStart = Mathf.Clamp(pz - halfSize, 0, heightResolution - 1);
        int xEnd = Mathf.Clamp(px + halfSize, 0, heightResolution - 1);
        int zEnd = Mathf.Clamp(pz + halfSize, 0, heightResolution - 1);

        int width = xEnd - xStart + 1;
        int height = zEnd - zStart + 1;

        float[,] heights = terrainData.GetHeights(xStart, zStart, width, height);

        for (int z = 0; z < height; z++)
        {
            for (int x = 0; x < width; x++)
            {
                float dx = (x + xStart - px) / (float)halfSize;
                float dz = (z + zStart - pz) / (float)halfSize;

                float falloff = Mathf.Clamp01(1f - (dx * dx + dz * dz));
                float delta = strength * falloff * Time.deltaTime;

                if (Input.GetKey(lowerKey)) delta = -delta;

                heights[z, x] = Mathf.Clamp01(heights[z, x] + delta);
            }
        }

        terrainData.SetHeights(xStart, zStart, heights);
        OnTerrainModified?.Invoke();
    }

    private void OnDestroy()
    {
        InputManager.OnScrollInput -= ScrollSize;
    }

    private void OnDisable()
    {
        InputManager.OnScrollInput -= ScrollSize;
        projector.gameObject.SetActive(false);
    }
    private void OnEnable()
    {
        InputManager.OnScrollInput += ScrollSize;
        projector.gameObject.SetActive(true);
    }
}