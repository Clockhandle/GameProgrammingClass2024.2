using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.Tilemaps;

public interface ItileHighlighter
{
    void HighlightTile(Vector3Int cellPos, Color color);
    void ClearHighlights();
}


public class TileHighlighter : ItileHighlighter
{
    private Tilemap tilemap;
    public TileHighlighter(Tilemap tilemap)
    {
        Debug.Log($"Initialized {this.GetType().Name}!");
        this.tilemap = tilemap;
    }
    public void HighlightTile(Vector3Int cellPos, Color color)
    {
        tilemap.SetTileFlags(cellPos, TileFlags.None);
        tilemap.SetColor(cellPos, color);
    }

    public void HighlightTileGreen(Vector3Int cellPos)
    {
        tilemap.SetTileFlags(cellPos, TileFlags.None);
        tilemap.SetColor(cellPos, Color.green);
    }


    public void ClearHighlights()
    {
        BoundsInt bounds = tilemap.cellBounds;
        foreach (Vector3Int pos in bounds.allPositionsWithin)
        {
            tilemap.SetTileFlags(pos, TileFlags.None);
            tilemap.SetColor(pos, Color.white);
        }
    }
}
