using UnityEngine;
using UnityEngine.Tilemaps;

public class TilePlacementValidator
{
    private TileDataLookUp tileDataLookUp;
    private TileOccupancyCheck tileOccupancyCheck;
    private Tilemap tilemap;
    public TilePlacementValidator(TileDataLookUp tileDataLookUp, TileOccupancyCheck tileOccupancyCheck, Tilemap tilemap)
    {
        this.tileDataLookUp = tileDataLookUp;
        this.tileOccupancyCheck = tileOccupancyCheck;
        this.tilemap = tilemap;
    }

    public bool IsPlacementValid(Vector3Int cellPos, TileType allowedTileFlags)
    {
        TileBase tile = tilemap.GetTile(cellPos);
        TileDataSO data = tileDataLookUp.GetTileData(tile);

        return data != null && (data.tileProperties & allowedTileFlags) != 0 && !tileOccupancyCheck.IsTileOccupied(cellPos);
    }
}