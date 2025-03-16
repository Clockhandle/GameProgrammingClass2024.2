using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;
using UnityEngine.Tilemaps;

public class PlacementGrid
{
    private int width;
    private int height;
    private Vector3 originPosition;
    private float cellSize;
    public int[,] gridArray;
    public TextMesh[,] gridValue;
    public bool showDebug = true;
    public PlacementGrid(int width, int height, float cellSize, Vector3 originPosition)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        this.originPosition = originPosition;

        gridArray = new int[width, height];
        gridValue = new TextMesh[width, height];

        if (showDebug)
        {
            for (int x = 0; x < gridArray.GetLength(0); ++x)
            {
                for (int y = 0; y < gridArray.GetLength(1); ++y)
                {

                    gridValue[x, y] = UtilsClass.CreateWorldText(gridArray[x, y].ToString(), null, GetWorldPosition(x, y) + new Vector3(cellSize, cellSize) * 0.5f, 8, Color.white, TextAnchor.MiddleCenter);
                    Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x + 1, y), Color.white, 100f);
                    Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x, y + 1), Color.white, 100f);
                }
            }
            Debug.DrawLine(GetWorldPosition(0, height), GetWorldPosition(width, height), Color.white, 100f);
            Debug.DrawLine(GetWorldPosition(width, 0), GetWorldPosition(width, height), Color.white, 100f);
        }
    }

    private Vector3 GetWorldPosition(int x, int y)
    {
        return new Vector3(x, y) * cellSize + originPosition;
    }

    private void GetXY(Vector3 worldPosition, out int x, out int y)
    {
        x = Mathf.FloorToInt((worldPosition - originPosition).x / cellSize);
        y = Mathf.FloorToInt((worldPosition - originPosition).y / cellSize);
    }

    public void SetValue(int x, int y, int value)
    {
        if (x >= 0 && y >= 0 && x < width && y < height)
        {
            gridArray[x, y] = value;
            gridValue[x, y].text = gridArray[x, y].ToString();
        }
        
    }

    public void SetValue(Vector3 worldPosition, int value)
    {
        int x, y;
        GetXY(worldPosition, out x, out y);
        SetValue(x, y, value);
    }

    public int GetValue(int x, int y)
    {
        if (x >= 0 && y >= 0 && x < width && y < height)
        {
            return gridArray[x, y];
        }
        else
        {
            return -1;
        }
    }

    public int GetValue(Vector3 worldPosition)
    {
        int x, y;
        GetXY(worldPosition, out x,out y);
        return GetValue(x, y);
    }
}

//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.Tilemaps;

//public enum GridType { Empty, Ground, HighGround, Water }

//public class TilemapGridSystem : MonoBehaviour
//{
//    [SerializeField] private Tilemap tilemap;

//    private Dictionary<Vector3Int, GridType> tileDataLookup = new Dictionary<Vector3Int, GridType>();

//    private void Start()
//    {
//        BoundsInt bounds = tilemap.cellBounds;

//        foreach (Vector3Int position in bounds.allPositionsWithin)
//        {
//            TileBase tile = tilemap.GetTile(position);

//            if (tile is CustomTile customTile && customTile.tileData != null)
//            {
//                tileDataLookup[position] = customTile.tileData.gridType;
//            }
//            else
//            {
//                tileDataLookup[position] = GridType.Empty; // Default to empty
//            }
//        }
//    }

//    public GridType GetGridType(Vector3 worldPosition)
//    {
//        Vector3Int cellPosition = tilemap.WorldToCell(worldPosition);
//        return tileDataLookup.TryGetValue(cellPosition, out GridType type) ? type : GridType.Empty;
//    }
//}
