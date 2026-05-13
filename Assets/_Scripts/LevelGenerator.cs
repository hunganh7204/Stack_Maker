using System;
using System.Collections.Generic;
using UnityEngine;

public enum TileType
{
    Start = -1,
    End = -2,
    Wall = 0,
    Floor = 1,
    TopLeft = 2,
    TopRight = 3,
    BottomLeft = 4,
    BottomRight = 5,
    Bridge = 6,
    Empty = 99
}

public class LevelGenerator : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject wallPrefab;
    [SerializeField] private GameObject floorPrefab;
    [SerializeField] private GameObject topLeftCornerPrefab;
    [SerializeField] private GameObject topRightCornerPrefab;
    [SerializeField] private GameObject bottomLeftCornerPrefab;
    [SerializeField] private GameObject bottomRightCornerPrefab;
    [SerializeField] private GameObject bridgePrefab;
    [SerializeField] private GameObject startPrefab;
    [SerializeField] private GameObject endPrefab;

    [Header("Settings")]
    public float tileSize = 1f;
    public Transform levelParent;

    public LevelData currentLevelData;

    [SerializeField] private float bridgeYOffset = 0.5f;

    private Dictionary<TileType, Queue<GameObject>> poolDictionary = new Dictionary<TileType, Queue<GameObject>>();
    private Dictionary<GameObject, TileType> activeTiles = new Dictionary<GameObject, TileType>();

    private Vector3 startPosition;
    private bool hasStartPos = false;

    public Vector3 GetStartPosition()
    {
        return startPosition;
    }
    public bool HasStartPosition()
    {
        return hasStartPos;
    }
    public void BuildLevelFromData(LevelData data)
    {
        currentLevelData = data;

        if (currentLevelData == null || currentLevelData.grid == null)
        {
            return;
        }

        ClearMapToPool();
        hasStartPos = false;

        int lengthZ = currentLevelData.grid.Count;

        for (int z = 0; z < lengthZ; z++)
        {
            MapRow row = currentLevelData.grid[z];
            for(int x=0; x<row.columns.Count; x++)
            {
                TileData tileNode = row.columns[x];
                TileType currentType = (TileType)tileNode.id;

                if (currentType == TileType.Empty) continue;

                float yPos = 0f;
                if (currentType == TileType.Bridge) yPos = bridgeYOffset;

                Vector3 spawnPos = new Vector3(x * tileSize, yPos, (lengthZ - z - 1) * tileSize);
                GameObject prefabToUse = null;

                if (currentType == TileType.Start)
                {
                    startPosition = spawnPos;
                    hasStartPos = true;
                }

                switch (currentType)
                {
                    case TileType.Wall:
                        prefabToUse = wallPrefab;
                        break;
                    case TileType.Floor:
                        prefabToUse = floorPrefab;
                        break;
                    case TileType.TopLeft:
                        prefabToUse = topLeftCornerPrefab;
                        break;
                    case TileType.TopRight:
                        prefabToUse = topRightCornerPrefab;
                        break;
                    case TileType.BottomLeft:
                        prefabToUse = bottomLeftCornerPrefab;
                        break;
                    case TileType.BottomRight:
                        prefabToUse = bottomRightCornerPrefab;
                        break;
                    case TileType.Bridge:
                        prefabToUse = bridgePrefab;
                        break;
                    case TileType.Start:
                        prefabToUse = startPrefab;
                        break;
                    case TileType.End:
                        prefabToUse = endPrefab;
                        break;
                }
                if (prefabToUse != null)
                {
                    GameObject spawnedTile = GetFromPool(currentType, prefabToUse, spawnPos);
                    if (currentType == TileType.Bridge || currentType == TileType.End)
                    {
                        //spawnedTile.transform.rotation = prefabToUse.transform.rotation * Quaternion.Euler(0, tileNode.rotY, 0); -> xoay theo truc cua prefab
                        spawnedTile.transform.rotation = Quaternion.Euler(0, tileNode.rotY, 0) * prefabToUse.transform.rotation; //xoay theo truc world
                    }
                }
            }
        }

    }

    private GameObject GetFromPool(TileType tileType, GameObject prefab, Vector3 position)
    {
        if(!poolDictionary.ContainsKey(tileType))
        {
            poolDictionary[tileType] = new Queue<GameObject>();
        }

        GameObject tile;

        if(poolDictionary[tileType].Count > 0)
        {
            tile = poolDictionary[tileType].Dequeue();
            tile.transform.position = position;
            tile.SetActive(true);
            tile.tag = prefab.tag;
            foreach (Transform child in tile.transform)
            {
                child.gameObject.SetActive(true);
            }
        }
        else
        {
            tile = Instantiate(prefab, position, prefab.transform.rotation);
            if (levelParent != null) tile.transform.SetParent(levelParent);
        }

        activeTiles.Add(tile, tileType);

        return tile;
    }

    private void ClearMapToPool()
    {
        foreach (var kvp in activeTiles)
        {
            GameObject obj = kvp.Key;
            TileType type = kvp.Value;

            if(obj != null)
            {
                obj.SetActive(false);
                if (!poolDictionary.ContainsKey(type))
                {
                    poolDictionary[type] = new Queue<GameObject>();
                }
                poolDictionary[type].Enqueue(obj);
            }
        }

        activeTiles.Clear();
    }
}
