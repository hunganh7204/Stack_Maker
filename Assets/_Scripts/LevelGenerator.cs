using System;
using System.Collections.Generic;
using UnityEngine;

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

    private Dictionary<int, Queue<GameObject>> poolDictionary = new Dictionary<int, Queue<GameObject>>();
    private Dictionary<GameObject, int> activeTiles = new Dictionary<GameObject, int>();

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
                int tileType = tileNode.id;

                if (tileType == 99) continue;

                float yPos = 0f;
                if (tileType == 6) yPos = bridgeYOffset;

                Vector3 spawnPos = new Vector3(x * tileSize, yPos, (lengthZ - z - 1) * tileSize);
                GameObject prefabToUse = null;

                if (tileType == -1)
                {
                    startPosition = spawnPos;
                    hasStartPos = true;
                }

                switch (tileType)
                {
                    case 0:
                        prefabToUse = wallPrefab;
                        break;
                    case 1:
                        prefabToUse = floorPrefab;
                        break;
                    case 2:
                        prefabToUse = topLeftCornerPrefab;
                        break;
                    case 3:
                        prefabToUse = topRightCornerPrefab;
                        break;
                    case 4:
                        prefabToUse = bottomLeftCornerPrefab;
                        break;
                    case 5:
                        prefabToUse = bottomRightCornerPrefab;
                        break;
                    case 6:
                        prefabToUse = bridgePrefab;
                        break;
                    case -1:
                        prefabToUse = startPrefab;
                        break;
                    case -2:
                        prefabToUse = endPrefab;
                        break;
                }
                if (prefabToUse != null)
                {
                    GameObject spawnedTile = GetFromPool(tileType, prefabToUse, spawnPos);
                    if (tileType == 6 || tileType == -2)
                    {
                        //spawnedTile.transform.rotation = prefabToUse.transform.rotation * Quaternion.Euler(0, tileNode.rotY, 0); -> xoay theo truc cua prefab
                        spawnedTile.transform.rotation = Quaternion.Euler(0, tileNode.rotY, 0) * prefabToUse.transform.rotation; //xoay theo truc world
                    }
                }
            }
        }

    }

    private GameObject GetFromPool(int tileType, GameObject prefab, Vector3 position)
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
            int type = kvp.Value;

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
