using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class TileData
{
    public int id = 99;      
    public float rotY = 0f;  

    public TileData(int id, float rotY)
    {
        this.id = id;
        this.rotY = rotY;
    }
}

[System.Serializable]
public class MapRow 
{
    public List<TileData> columns = new List<TileData>();
}

[System.Serializable]
public class LevelData
{
    public int levelID;
    public List<MapRow> grid = new List<MapRow>();
}
