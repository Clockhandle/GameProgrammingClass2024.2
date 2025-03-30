using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering.Universal;
using UnityEngine.Tilemaps;

public class TileManager : MonoBehaviour
{

    public Tilemap tilemap;
    [SerializeField]
    private TileDataSO[] tileDataArray; // Array of all TileDataSO assets
    public TileDataLookUp tileDataLookUp;
    public TileOccupancyCheck tileOccupancyCheck;
    public TileHighlighter tileHighlighter;
    public TileType CurrentTileFlags { get; set; }

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

        // Fail early if no tile data or if the cell is occupied.
        if (data == null || tileOccupancyCheck.IsTileOccupied(cellPos))
            return false;

        // Get the unit's data from its prefab.
        Unit unitData = characterPrefab.GetComponent<Unit>();
        if (unitData == null)
        {
            Debug.LogError("Character prefab is missing UnitData component!");
            return false;
        }

        GetAllowedTileFlags(unitData);

        bool validPlacement = (data.tileProperties & CurrentTileFlags) != 0;
        // Check if tile is valid for placement (e.g., has LowTile property) and unoccupied
        if (validPlacement)
        {
            Vector3 snapPosition = tilemap.GetCellCenterWorld(cellPos);
            Instantiate(characterPrefab, snapPosition, Quaternion.identity);
            tileOccupancyCheck.SetTileToOccupied(cellPos, true);
            return true;
        }
        return false;
    }

    public void HighlightPlaceableTiles(GameObject characterPrefab)
    {
        Unit unitData = characterPrefab.GetComponent<Unit>();
        GetAllowedTileFlags(unitData);

        BoundsInt bounds = tilemap.cellBounds;
        foreach (Vector3Int pos in bounds.allPositionsWithin)
        {
            TileBase tile = tilemap.GetTile(pos);
            if (tile != null)
            {
                TileDataSO data = tileDataLookUp.GetTileData(tile);
                if (data != null && (data.tileProperties & CurrentTileFlags) != 0 &&
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

        canPlace = (data != null &&
                    (data.tileProperties & CurrentTileFlags) != 0 &&
                    !tileOccupancyCheck.IsTileOccupied(cellPos));
        return cellPos;
    }



    public void GetAllowedTileFlags(Unit unitData)
    {
        TileType allowed = 0;
        // If the unit is Ground, allow LowTile.
        if ((unitData.unitDataSO.type & UnitType.Ground) != 0)
        {
            allowed |= TileType.LowTile;
        }
        // If the unit is Ranged, allow HighTile.
        if ((unitData.unitDataSO.type & UnitType.Ranged) != 0)
        {
            allowed |= TileType.HighTile;
        }
        CurrentTileFlags = allowed;
    }

}
