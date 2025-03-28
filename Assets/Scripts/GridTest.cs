using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;

public class GridTest : MonoBehaviour
{
    public PlacementGrid grid;

    private void OnEnable()
    {
        // Subscribe to the tap event from the InputManager.
        InputManager.OnTap += LeftClick;
    }

    private void OnDisable()
    {
        // Unsubscribe when the script is disabled.
        InputManager.OnTap -= LeftClick;
    }

    private void Start()
    {
        grid = new PlacementGrid(22, 11, 1f, transform.position);
    }

    void LeftClick()
    {
        grid.SetValue(UtilsClass.GetMouseWorldPosition(), 20);
    }

}
