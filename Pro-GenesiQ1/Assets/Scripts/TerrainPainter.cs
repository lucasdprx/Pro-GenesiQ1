using System;
using UnityEngine;

namespace TerrainSystem
{
    [RequireComponent(typeof(Terrain))]
    public class TerrainPainter : MonoBehaviour
    {
        public int brushSize = 16;
        [SerializeField] private int minBrushSize = 2;
        [SerializeField] private int maxBrushSize = 50;
        public float strength = 0.01f;
        public KeyCode lowerKey = KeyCode.LeftShift;

        private int heightRes;


        private Terrain terrain;
        private TerrainData tData;
        private Camera cam;

        public static event Action onTerrainModified;

        private void Start()
        {
            cam = Camera.main;
            terrain = GetComponent<Terrain>();
            tData = Instantiate(terrain.terrainData);
            terrain.terrainData = tData;
            heightRes = tData.heightmapResolution;

            GetComponent<TerrainCollider>().terrainData = tData;
        }

        private void Update()
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            if (!Physics.Raycast(ray, out RaycastHit hit)) return;


            if (!Input.GetMouseButton(0)) return;


            Vector3 terrainPos = hit.point - terrain.transform.position;
            float normX = terrainPos.x / tData.size.x;
            float normZ = terrainPos.z / tData.size.z;

            int px = Mathf.RoundToInt(normX * (heightRes - 1));
            int pz = Mathf.RoundToInt(normZ * (heightRes - 1));

            int half = brushSize / 2;
            int xStart = Mathf.Clamp(px - half, 0, heightRes - 1);
            int zStart = Mathf.Clamp(pz - half, 0, heightRes - 1);
            int xEnd = Mathf.Clamp(px + half, 0, heightRes - 1);
            int zEnd = Mathf.Clamp(pz + half, 0, heightRes - 1);

            int w = xEnd - xStart + 1;
            int h = zEnd - zStart + 1;

            float[,] heights = tData.GetHeights(xStart, zStart, w, h);

            for (int z = 0; z < h; z++)
            {
                for (int x = 0; x < w; x++)
                {
                    float dx = (x + xStart - px) / (float)half;
                    float dz = (z + zStart - pz) / (float)half;
                    float falloff = Mathf.Clamp01(1f - (dx * dx + dz * dz));
                    float delta = strength * falloff * Time.deltaTime;
                    if (Input.GetKey(lowerKey)) delta = -delta;
                    heights[z, x] = Mathf.Clamp01(heights[z, x] + delta);
                }
            }

            tData.SetHeights(xStart, zStart, heights);
            onTerrainModified?.Invoke();
        }
    }
}