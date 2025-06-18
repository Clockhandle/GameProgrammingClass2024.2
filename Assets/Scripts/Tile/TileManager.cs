using UnityEngine;
using UnityEngine.Tilemaps;
using TowerDefense.Utils;

public class TileManager : MonoBehaviour
{
    public static TileManager Instance { get; private set; }
    public Tilemap tilemap;
    [SerializeField]
    private TileDataSO[] tileDataArray;
    public TileDataLookUp tileDataLookUp;
    public TileOccupancyCheck tileOccupancyCheck;
    public ItileHighlighter tileHighlighter;
    public TilePlacementValidator tilePlacementValidator;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
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

        TileType allowedFlags = GameUtils.GetAllowedTileFlags(unitData);
        if (!tilePlacementValidator.IsPlacementValid(cellPos, allowedFlags)) return false;

        Vector3 snapPosition = tilemap.GetCellCenterWorld(cellPos);
        Instantiate(characterPrefab, snapPosition, Quaternion.identity);
        tileOccupancyCheck.SetTileToOccupied(cellPos, true);
        return true;
    }
    
    public bool TryPlaceCharacterProvisionally(Vector3 worldPosition, GameObject characterPrefab, out Unit placedUnit)
    {
        placedUnit = null;
        Vector3Int cellPos = tilemap.WorldToCell(worldPosition);

        Unit unitData = characterPrefab.GetComponent<Unit>();
        if(unitData == null) return false;

        TileType allowedFlags = GameUtils.GetAllowedTileFlags(unitData);
        if (!tilePlacementValidator.IsPlacementValid(cellPos, allowedFlags)) return false;

        Vector3 snapPosition = tilemap.GetCellCenterWorld(cellPos);
        GameObject newInstance = Instantiate(characterPrefab, snapPosition, Quaternion.identity);
        Unit newUnitComponent = newInstance.GetComponent<Unit>();

        if(newUnitComponent != null)
        {
            newUnitComponent.SetSourcePrefab(characterPrefab);
            newUnitComponent.InitializeAwaitDeploymentState(characterPrefab);
            tileOccupancyCheck.SetTileToOccupied(cellPos, true);
            placedUnit = newUnitComponent;
            return true;
        }
        else
        {
            Debug.LogError($"Instantiated prefab {characterPrefab.name} succeeded but failed to get Unit Component for new Instance!", newInstance);
            return false;
        }
    }
    public void HighlightPlaceableTiles(GameObject characterPrefab)
    {
        Unit unitData = characterPrefab.GetComponent<Unit>();
        TileType allowedFlags = GameUtils.GetAllowedTileFlags(unitData);

        BoundsInt bounds = tilemap.cellBounds;
        foreach (Vector3Int pos in bounds.allPositionsWithin)
        {
            Color highlightColor = tilePlacementValidator.IsPlacementValid(pos, allowedFlags) ? Color.green : Color.red;
            tileHighlighter.HighlightTile(pos, highlightColor);
        }
    }

    public Vector3Int GetClosestPlaceableTile(Vector3 worldPos, GameObject characterPrefab, out bool canPlace)
    {
        Vector3Int cellPos = tilemap.WorldToCell(worldPos);
        Unit unitData = characterPrefab.GetComponent<Unit>();
        TileType allowedFlags = GameUtils.GetAllowedTileFlags(unitData);

        canPlace = tilePlacementValidator.IsPlacementValid(cellPos, allowedFlags);
        return cellPos;
    }
}