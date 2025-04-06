using UnityEngine;
using UnityEngine.Tilemaps;
using TowerDefense.Utils;

public class TileManager : MonoBehaviour
{
    public Tilemap tilemap;
    [SerializeField]
    private TileDataSO[] tileDataArray;
    public TileDataLookUp tileDataLookUp;
    public TileOccupancyCheck tileOccupancyCheck;
    public ItileHighlighter tileHighlighter;
    public TilePlacementValidator tilePlacementValidator;
    public TileType CurrentTileFlags { get; set; }

    private void Awake()
    {
        tileDataLookUp = new TileDataLookUp(tileDataArray); // Updated: No Tilemap
        tileOccupancyCheck = new TileOccupancyCheck(tilemap);
        tileHighlighter = new TileHighlighter(tilemap);
        tilePlacementValidator = new TilePlacementValidator(tileDataLookUp, tileOccupancyCheck, tilemap);

        tileDataLookUp.GenerateDataDictionary();
        tileOccupancyCheck.GenerateOccupancyDictionary();
    }

    public bool TryPlaceCharacter(Vector3 worldPosition, GameObject characterPrefab)
    {
        Vector3Int cellPos = tilemap.WorldToCell(worldPosition);
        //TileDataSO data = tileDataLookUp.GetTileData(tilemap.GetTile(cellPos));

        Unit unitData = characterPrefab.GetComponent<Unit>();
        if (unitData == null) return false;

        CurrentTileFlags = UnitUtils.GetAllowedTileFlags(unitData);

        if (!tilePlacementValidator.IsPlacementValid(cellPos, CurrentTileFlags)) return false;

        Vector3 snapPosition = tilemap.GetCellCenterWorld(cellPos);
        Instantiate(characterPrefab, snapPosition, Quaternion.identity);
        tileOccupancyCheck.SetTileToOccupied(cellPos, true);
        return true;
    }

    public void HighlightPlaceableTiles(GameObject characterPrefab)
    {
        Unit unitData = characterPrefab.GetComponent<Unit>();
        CurrentTileFlags = UnitUtils.GetAllowedTileFlags(unitData);

        BoundsInt bounds = tilemap.cellBounds;
        foreach (Vector3Int pos in bounds.allPositionsWithin)
        {
            Color highlightColor = tilePlacementValidator.IsPlacementValid(pos, CurrentTileFlags) ? Color.green : Color.red;
            tileHighlighter.HighlightTile(pos, highlightColor);
        }
    }

    public Vector3Int GetClosestPlaceableTile(Vector3 worldPos, GameObject characterPrefab, out bool canPlace)
    {
        Vector3Int cellPos = tilemap.WorldToCell(worldPos);
        Unit unitData = characterPrefab.GetComponent<Unit>();
        CurrentTileFlags = UnitUtils.GetAllowedTileFlags(unitData);

        canPlace = tilePlacementValidator.IsPlacementValid(cellPos, CurrentTileFlags);
        return cellPos;
    }
}