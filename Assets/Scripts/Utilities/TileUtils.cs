using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace TowerDefense.Utils
{
    public static class TileUtils
    {
        public static BoundsInt GetVisibleCellBounds(Tilemap tilemap)
        {
            Camera mainCamera = Camera.main;
            Vector3 bottomLeftWorld = mainCamera.ViewportToWorldPoint(new Vector3(0, 0, mainCamera.nearClipPlane));
            Vector3 topRightWorld = mainCamera.ViewportToWorldPoint(new Vector3(1, 1, mainCamera.nearClipPlane));

            Vector3Int bottomLeftCell = tilemap.WorldToCell(bottomLeftWorld);
            Vector3Int topRightCell = tilemap.WorldToCell(topRightWorld);

            int buffer = 1;
            bottomLeftCell -= new Vector3Int(buffer, buffer, 0);
            topRightCell += new Vector3Int(buffer, buffer, 0);

            return new BoundsInt(bottomLeftCell.x, bottomLeftCell.y, 0,
                                 topRightCell.x - bottomLeftCell.x + 1,
                                 topRightCell.y - bottomLeftCell.y + 1, 1);
        }
    }
}
