using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;

public class GridTest : MonoBehaviour
{
    private PlacementGrid grid;
    private void Start()
    {
        grid = new PlacementGrid(22, 11, 1f);
    }

    private void Update()
    {
        if(InputManager.gameplayInstance.isLeftMouseDown)
        {
            grid.SetValue(UtilsClass.GetMouseWorldPosition(), 20);
        }
    }
}
