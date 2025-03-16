using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;

public class GridTest : MonoBehaviour
{
    public PlacementGrid grid;
    private void Start()
    {
        grid = new PlacementGrid(22, 11, 1f, transform.position);
    }

    private void Update()
    {
        if(InputManager.gameplayInstance.isLeftClickBeingPressed)
        {
            LeftClick();
        }
    }
    private void OnEnable()
    {
        //InputManager.gameplayInstance.OnLeftClickDown.AddListener(LeftClick);
        InputManager.gameplayInstance.OnRightClickDown.AddListener(RightClick);
    }
    private void OnDisable()
    {
        //InputManager.gameplayInstance.OnLeftClickDown.RemoveListener(LeftClick);
        InputManager.gameplayInstance.OnRightClickDown.RemoveListener(RightClick);
    }

    void LeftClick()
    {
        grid.SetValue(UtilsClass.GetMouseWorldPosition(), 20);
    }

    void RightClick()
    {
        Debug.Log(grid.GetValue(UtilsClass.GetMouseWorldPosition()));
    }
}
