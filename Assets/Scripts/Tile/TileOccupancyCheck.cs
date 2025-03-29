using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileOccupancyCheck
{
    private Tilemap tilemap;
    Dictionary<Vector3Int, bool> occupancyDictionary;
    
    public TileOccupancyCheck(Tilemap tilemap)
    {
        Debug.Log($"Initialized {this.GetType().Name}!");
        this.tilemap = tilemap;
    }
    //public void Initialize(Tilemap tilemap)
    //{
    //    Debug.Log($"Initialized {this.GetType().Name}!");
    //    this.tilemap = tilemap;
    //    GenerateOccupancyDictionary();
    //}

    public void GenerateOccupancyDictionary()
    {
        occupancyDictionary = new Dictionary<Vector3Int, bool>();
        BoundsInt bounds = tilemap.cellBounds;
        foreach(Vector3Int pos in bounds.allPositionsWithin)
        {
            occupancyDictionary[pos] = false;
        }
    }

    public bool IsTileOccupied(Vector3Int cellPos)
    {
        return occupancyDictionary.ContainsKey(cellPos) && occupancyDictionary[cellPos];
    }

    public void SetTileToOccupied(Vector3Int cellPos, bool occupied)
    {
        if (occupancyDictionary.ContainsKey(cellPos))
        {
            occupancyDictionary[cellPos] = occupied;
        }
        else
        {
            Debug.Log("No cell in OccupancyDictionary!");
        }
    }
}
