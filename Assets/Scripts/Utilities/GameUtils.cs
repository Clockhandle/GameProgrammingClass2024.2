using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace TowerDefense.Utils
{
    public static class GameUtils
    {
        public static BoundsInt GetVisibleCellBounds(Tilemap tilemap)
        {
            return TileUtils.GetVisibleCellBounds(tilemap);
        }

        public static TileType GetAllowedTileFlags(Unit unitData)
        {
            return UnitUtils.GetAllowedTileFlags(unitData);
        }
    }
}
