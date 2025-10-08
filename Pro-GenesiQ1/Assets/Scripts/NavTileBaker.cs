using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace TerrainSystem
{
    public class NavTileBaker : MonoBehaviour
    {
        [Header("Terrain / Grid")]
        public Terrain terrain;
        [Tooltip("Tile size in meters.")]
        public int tileSize = 64;
        [Tooltip("Total overlap (in meters) between tiles. Interpreted as TOTAL overlap, distributed symmetrically.")]
        public float tilePadding = 1.0f;
        [Tooltip("Layers included in the bake.")]
        public LayerMask includedLayers = ~0;

        [Header("NavMesh")]
        [Tooltip("Agent Type ID (see Navigation Settings).")]
        public int agentTypeID = 0;
        public NavMeshBuildSettings buildSettings;

        private class Tile
        {
            public NavMeshData navData;
            public NavMeshDataInstance instance;
            public AsyncOperation buildOp;
            public bool pendingRebuild = false;
        }

        private readonly Dictionary<Vector2Int, Tile> tiles = new Dictionary<Vector2Int, Tile>();
        private Vector3 terrainOrigin;
        private Vector2Int gridSize;

        private readonly List<NavMeshBuildSource> scratchSources = new List<NavMeshBuildSource>();
        private readonly List<NavMeshBuildMarkup> scratchMarkups = new List<NavMeshBuildMarkup>();

        const float TILE_EPS = 0.01f;

        #region Unity Lifecycle
        void OnValidate()
        {
            tileSize = Mathf.Max(1, tileSize);
            tilePadding = Mathf.Max(0f, tilePadding);
        }

        void Start()
        {
            if (terrain == null)
            {
                Debug.LogError("[NavTileBaker] Terrain not assigned!");
                enabled = false;
                return;
            }

            if (terrain.transform.lossyScale != Vector3.one)
            {
                Debug.LogWarning("[NavTileBaker] The Terrain GameObject has a non-uniform scale (lossyScale != 1). This may break size coordinates and cause gaps. Better: use terrainData.size and keep scale at (1,1,1).");
            }

            buildSettings = NavMesh.GetSettingsByID(agentTypeID);
            if (buildSettings.agentTypeID == -1)
            {
                Debug.LogWarning($"[NavTileBaker] No NavMeshBuildSettings found for agentTypeID={agentTypeID}, using default settings.");
                buildSettings = NavMesh.CreateSettings();
            }

            InitGrid();
            BuildAllTilesAsync();
        }

        void OnDestroy()
        {
            foreach (var kv in tiles)
            {
                var t = kv.Value;
                if (t.instance.valid)
                    t.instance.Remove();
            }
            tiles.Clear();
        }
        #endregion

        #region Grid Setup
        void InitGrid()
        {
            terrainOrigin = terrain.transform.position;
            var size = terrain.terrainData.size;
            gridSize.x = Mathf.CeilToInt(size.x / tileSize);
            gridSize.y = Mathf.CeilToInt(size.z / tileSize);
        }

        Vector2Int WorldToTileCoord(Vector3 worldPos)
        {
            var local = worldPos - terrainOrigin;
            float realTileSize = tileSize + tilePadding;
            int x = Mathf.FloorToInt(local.x / realTileSize);
            int z = Mathf.FloorToInt(local.z / realTileSize);
            x = Mathf.Clamp(x, 0, gridSize.x - 1);
            z = Mathf.Clamp(z, 0, gridSize.y - 1);
            return new Vector2Int(x, z);
        }

        Bounds GetTileBounds(Vector2Int coord)
        {
            float minX = terrainOrigin.x + coord.x * tileSize;
            float minZ = terrainOrigin.z + coord.y * tileSize;
            float sizeX = tileSize;
            float sizeZ = tileSize;

            var terrainSize = terrain.terrainData.size;
            if ((coord.x + 1) * tileSize > terrainSize.x) sizeX = terrainSize.x - coord.x * tileSize;
            if ((coord.y + 1) * tileSize > terrainSize.z) sizeZ = terrainSize.z - coord.y * tileSize;

            float totalSizeX = sizeX + tilePadding;
            float totalSizeZ = sizeZ + tilePadding;

            float halfY = GetVerticalHalfExtents();

            var center = new Vector3(minX + sizeX * 0.5f, terrainOrigin.y + halfY, minZ + sizeZ * 0.5f);
            var size = new Vector3(totalSizeX, halfY * 2f, totalSizeZ);
            return new Bounds(center, size);
        }

        float GetVerticalHalfExtents()
        {
            var sizeY = terrain.terrainData.size.y;
            return Mathf.Max(5f, sizeY * 0.5f);
        }
        #endregion

        #region Tile Management
        Tile EnsureTile(Vector2Int coord)
        {
            if (tiles.TryGetValue(coord, out var t))
                return t;

            t = new Tile();
            t.navData = new NavMeshData(agentTypeID);
            var bounds = GetTileBounds(coord);
            t.navData.position = bounds.center;
            t.navData.rotation = Quaternion.identity;
            t.instance = NavMesh.AddNavMeshData(t.navData);
            tiles[coord] = t;
            return t;
        }

        void BuildAllTilesAsync()
        {
            for (int x = 0; x < gridSize.x; x++)
                for (int z = 0; z < gridSize.y; z++)
                    RebuildTileAsync(new Vector2Int(x, z));
        }
        #endregion

        #region Terrain Notifications
        public void NotifyTerrainChanged(Bounds changedWorldBounds)
        {
            RebuildTilesIntersecting(changedWorldBounds);
        }

        public void NotifyTerrainChanged(Vector3 point, float radius)
        {
            Vector3 minWorld = new Vector3(point.x - radius - TILE_EPS, 0f, point.z - radius - TILE_EPS);
            Vector3 maxWorld = new Vector3(point.x + radius + TILE_EPS, 0f, point.z + radius + TILE_EPS);

            Vector2Int minTile = WorldToTileCoord(minWorld);
            Vector2Int maxTile = WorldToTileCoord(maxWorld);

            if (minTile.x > maxTile.x) (minTile.x, maxTile.x) = (maxTile.x, minTile.x);
            if (minTile.y > maxTile.y) (minTile.y, maxTile.y) = (maxTile.y, minTile.y);

            RebuildTilesRange(minTile, maxTile);
        }

        void RebuildTilesRange(Vector2Int min, Vector2Int max)
        {
            min.x = Mathf.Clamp(min.x, 0, gridSize.x - 1);
            min.y = Mathf.Clamp(min.y, 0, gridSize.y - 1);
            max.x = Mathf.Clamp(max.x, 0, gridSize.x - 1);
            max.y = Mathf.Clamp(max.y, 0, gridSize.y - 1);

            for (int x = min.x; x <= max.x; x++)
                for (int z = min.y; z <= max.y; z++)
                    RebuildTileAsync(new Vector2Int(x, z));
        }

        void RebuildTilesIntersecting(Bounds changed)
        {
            var min = WorldToTileCoord(changed.min);
            var max = WorldToTileCoord(changed.max);

            for (int x = min.x; x <= max.x; x++)
                for (int z = min.y; z <= max.y; z++)
                    RebuildTileAsync(new Vector2Int(x, z));
        }
        #endregion

        #region Async Build System
        void RebuildTileAsync(Vector2Int coord)
        {
            var tile = EnsureTile(coord);

            if (tile.buildOp != null && !tile.buildOp.isDone)
            {
                print("oi");
                tile.pendingRebuild = true;
                return;
            }

            StartCoroutine(BuildTileCoroutine(tile, coord));
        }

        IEnumerator BuildTileCoroutine(Tile tile, Vector2Int coord)
        {
            var bounds = GetTileBounds(coord);

            scratchSources.Clear();
            scratchMarkups.Clear();

            NavMeshBuilder.CollectSources(
                bounds,
                includedLayers.value,
                NavMeshCollectGeometry.RenderMeshes,
                0,
                scratchMarkups,
                scratchSources
            );

            var terrainSource = CreateTerrainSource();
            if (terrainSource.HasValue)
            {
                print("Negronado terreno");
                scratchSources.Add(terrainSource.Value);
            }

            tile.buildOp = NavMeshBuilder.UpdateNavMeshDataAsync(tile.navData, buildSettings, scratchSources, bounds);
            yield return tile.buildOp;

            tile.buildOp = null;

            if (tile.pendingRebuild)
            {
                print("Negronado terrenodddddddd");
                tile.pendingRebuild = false;
                RebuildTileAsync(coord);
            }
        }

        NavMeshBuildSource? CreateTerrainSource()
        {
            if (terrain == null) return null;

            var src = new NavMeshBuildSource
            {
                shape = NavMeshBuildSourceShape.Terrain,
                sourceObject = terrain.terrainData,
                transform = terrain.transform.localToWorldMatrix,
                area = 0
            };
            return src;
        }
        #endregion

        #region Debug / Tools / Gizmos

        [ContextMenu("Rebuild entire terrain")]
        void RebuildAllTilesMenu() => BuildAllTilesAsync();

        [ContextMenu("Clear all tiles")]
        void ClearAllTiles()
        {
            foreach (var t in tiles.Values)
            {
                if (t.instance.valid)
                    t.instance.Remove();
            }
            tiles.Clear();
        }
        #endregion
    }
}
