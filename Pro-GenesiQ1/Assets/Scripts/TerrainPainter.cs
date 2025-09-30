using System;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

namespace TerrainSystem
{
    [RequireComponent(typeof(Terrain))]
    [RequireComponent(typeof(TerrainCollider))]
    public class TerrainPainter : MonoBehaviour
    {
        [Header("Brush Settings")]
        [Tooltip("Brush size in pixels.")]
        [Range(2, 50)]
        [SerializeField] private int brushSize = 16;

        [Tooltip("Brush strength applied per second.")]
        [Range(0.01f, 1f)]
        [SerializeField] private float strength = 0.01f;

        [Tooltip("Key used to lower terrain instead of raising.")]
        [SerializeField] private KeyCode lowerKey = KeyCode.LeftShift;
        
        [SerializeField] private DecalProjector projector;

        private Terrain terrain;
        private TerrainData terrainData;
        private Camera mainCamera;
        private int heightResolution;

        public static event Action OnTerrainModified;

        private void Start()
        {
            mainCamera = Camera.main;
            if (mainCamera == null)
            {
                Debug.LogError("No Main Camera found. TerrainPainter requires a Camera tagged as 'MainCamera'.");
                enabled = false;
                return;
            }

            terrain = GetComponent<Terrain>();
            terrainData = Instantiate(terrain.terrainData);
            terrain.terrainData = terrainData;
            heightResolution = terrainData.heightmapResolution;
            InputManager.OnScrollInput += ScrollSize;

            GetComponent<TerrainCollider>().terrainData = terrainData;
        }

        private void ScrollSize(Vector2 input)
        {
            if (Mathf.Abs(input.y) > 0.01f)
            {
                brushSize += (int)input.y;
                brushSize = Mathf.Clamp(brushSize, 2, 50);
            }
        }

        private void Update()
        {
            if (!GetTerrainHit(out RaycastHit hit)) return;
            UpdateSizeProjector(hit.point);

            if (!Input.GetMouseButton(0)) return;
            ApplyBrush(hit.point);
        }

        private void UpdateSizeProjector(Vector3 hitPoint)
        {
            if (projector)
            {
                projector.transform.position = hitPoint + Vector3.up * 5;
                float brushWorldSize = brushSize * terrainData.size.x / heightResolution;
                projector.size = new Vector3(brushWorldSize, 200f, brushWorldSize);
            }
        }

        private bool GetTerrainHit(out RaycastHit hit)
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            return Physics.Raycast(ray, out hit);
        }

        private void ApplyBrush(Vector3 worldHitPoint)
        {
            Vector3 localPos = worldHitPoint - terrain.transform.position;

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
    }
}