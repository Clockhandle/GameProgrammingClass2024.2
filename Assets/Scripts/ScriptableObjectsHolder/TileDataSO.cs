using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
[System.Flags]
public enum TileType
{
    None = 0,
    HighTile = 1 << 0,
    LowTile = 1 << 1,
    HealingTile = 1 << 2
}
[CreateAssetMenu(fileName = "Tile Data", menuName = "Data/Tile Data")]
public class TileDataSO : ScriptableObject
{
    public TileBase[] tiles;
    public TileType tileProperties;
}
