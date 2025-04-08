using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace TowerDefense.Utils
{
    public static class GameUtils
    {
        #region Tile Utilities
        public static BoundsInt GetVisibleCellBounds(Tilemap tilemap)
        {
            return TileUtils.GetVisibleCellBounds(tilemap);
        }
        #endregion

        #region Unit Utilities
        public static TileType GetAllowedTileFlags(Unit unitData)
        {
            return UnitUtils.GetAllowedTileFlags(unitData);
        }
        #endregion
    }
}
