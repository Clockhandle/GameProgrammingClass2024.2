using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileDataLookUp
{
    private TileDataSO[] tileDataArray;
    Dictionary<TileBase, TileDataSO> tileDataDictionary;


    public TileDataLookUp(TileDataSO[] tileDataArray)
    {
        Debug.Log($"Initialized {this.GetType().Name}!");
        this.tileDataArray = tileDataArray;
    }
    public void GenerateDataDictionary()
    {
        tileDataDictionary = new Dictionary<TileBase, TileDataSO>();

        foreach (TileDataSO data in tileDataArray)
        {
            foreach (TileBase tile in data.tiles)
            {
                if(!tileDataDictionary.ContainsKey(tile))
                {
                    tileDataDictionary.Add(tile, data);
                }
            }
        }
    }
        
    public TileDataSO GetTileData(TileBase tile)
    {
        if (tile != null && tileDataDictionary.TryGetValue(tile, out TileDataSO data)) return data;
        return null;
    }
}
