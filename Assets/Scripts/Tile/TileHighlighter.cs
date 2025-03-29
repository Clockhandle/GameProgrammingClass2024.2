using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


public class TileHighlighter
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
