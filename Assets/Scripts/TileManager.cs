using UnityEngine;
using UnityEngine.Tilemaps;

public class TileManager : MonoBehaviour
{
    [SerializeField]
    private Tilemap tilemap;
    [SerializeField]
    private TileDataSO[] tileDataArray; // Array of all TileDataSO assets

    private void OnEnable()
    {
        // Subscribe to InputManager's OnTap event.
        InputManager.OnTap += CheckTileAtMouse;
    }

    private void OnDisable()
    {
        // Unsubscribe when disabled.
        InputManager.OnTap -= CheckTileAtMouse;
    }

    // This method is called when a tap event occurs.
    private void CheckTileAtMouse()
    {
        Vector2 mousePosition = InputManager.MouseWorldPosition;
        Vector3Int gridPosition = tilemap.WorldToCell(mousePosition);

        TileBase clickedTile = tilemap.GetTile(gridPosition);

        if (clickedTile != null)
        {
            Debug.Log($"At position {gridPosition}, there is a {clickedTile.name}");
            CheckTileProperties(clickedTile);
        }
        else
        {
            Debug.Log($"No tile found at {gridPosition}");
        }
    }

    private void CheckTileProperties(TileBase tile)
    {
        foreach (TileDataSO data in tileDataArray)
        {
            if (System.Array.Exists(data.tiles, t => t == tile))
            {
                Debug.Log($"Tile has properties: {data.tileProperties}");

                // Example: Checking specific properties
                if ((data.tileProperties & TileType.HighTile) != 0)
                    Debug.Log("This is a High Tile!");
                if ((data.tileProperties & TileType.LowTile) != 0)
                    Debug.Log("This is a Low Tile!");
                if ((data.tileProperties & TileType.HealingTile) != 0)
                    Debug.Log("This is a Healing Tile!");

                return; // Stop checking further once found
            }
        }

        Debug.Log("Tile has no special properties.");
    }
}
