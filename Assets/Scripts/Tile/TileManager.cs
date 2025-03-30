using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

public class TileManager : MonoBehaviour
{

    public Tilemap tilemap;
    [SerializeField]
    private TileDataSO[] tileDataArray; // Array of all TileDataSO assets
    public TileDataLookUp tileDataLookUp;
    public TileOccupancyCheck tileOccupancyCheck;
    public TileHighlighter tileHighlighter;

    private void Awake()
    {
        tileDataLookUp = new TileDataLookUp(tileDataArray);
        tileOccupancyCheck = new TileOccupancyCheck(tilemap);
        tileHighlighter = new TileHighlighter(tilemap);

        tileDataLookUp.GenerateDataDictionary();
        tileOccupancyCheck.GenerateOccupancyDictionary();
    }


    public bool TryPlaceCharacter(Vector3 worldPosition, GameObject characterPrefab)
    {
        Vector3Int cellPos = tilemap.WorldToCell(worldPosition);
        TileDataSO data = tileDataLookUp.GetTileData(tilemap.GetTile(cellPos));

        // Check if tile is valid for placement (e.g., has LowTile property) and unoccupied
        if (data != null && (data.tileProperties & TileType.LowTile) != 0 &&
            !tileOccupancyCheck.IsTileOccupied(cellPos))
        {
            Vector3 snapPosition = tilemap.GetCellCenterWorld(cellPos);
            Instantiate(characterPrefab, snapPosition, Quaternion.identity);
            tileOccupancyCheck.SetTileToOccupied(cellPos, true);
            return true;
        }
        return false;
    }

    public void HighlightPlaceableTiles()
    {
        BoundsInt bounds = tilemap.cellBounds;
        foreach (Vector3Int pos in bounds.allPositionsWithin)
        {
            TileBase tile = tilemap.GetTile(pos);
            if (tile != null)
            {
                TileDataSO data = tileDataLookUp.GetTileData(tile);
                if (data != null && (data.tileProperties & TileType.LowTile) != 0 &&
                    !tileOccupancyCheck.IsTileOccupied(pos))
                {
                    tileHighlighter.HighlightTile(pos, Color.green);
                }
                else
                {
                    tileHighlighter.HighlightTile(pos, Color.red);
                }
            }
        }
    }

    public Vector3Int GetClosestPlaceableTile(Vector3 worldPos, out bool canPlace)
    {
        Vector3Int cellPos = tilemap.WorldToCell(worldPos);
        TileDataSO data = tileDataLookUp.GetTileData(tilemap.GetTile(cellPos));

        canPlace = (data != null && (data.tileProperties & TileType.LowTile) != 0 && !tileOccupancyCheck.IsTileOccupied(cellPos));

        return cellPos;
    }
}
