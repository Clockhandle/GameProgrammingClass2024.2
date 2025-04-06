using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TowerDefense.Utils
{
    public static class UnitUtils
    {
        public static TileType GetAllowedTileFlags(Unit unitData)
        {
            TileType allowed = 0;
            if ((unitData.unitDataSO.type & UnitType.Ground) != 0)
            {
                allowed |= TileType.LowTile;
            }
            if ((unitData.unitDataSO.type & UnitType.Ranged) != 0)
            {
                allowed |= TileType.HighTile;
            }
            return allowed;
        }
    }
}