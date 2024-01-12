using _Game.DesignPattern;
using _Game.GameGrid;
using _Game.Utilities.Grid;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugManager : SimpleSingleton<DebugManager>
{
    private Grid<GameGridCell, GameGridCellData>.DebugGrid debugGrid;
    [SerializeField]
    GameObject FpsDebug;
    [SerializeField]
    GameObject LogDebug;
    public bool IsDebugGridLogic => debugGrid != null ? true : false;
    public void DebugGridData(Grid<GameGridCell, GameGridCellData> grid)
    {
        debugGrid?.DrawGrid(grid, true);
    }
    public void OnInit(bool isDebugGridLogic, bool isDebugFps, bool isDebugLog)
    {
        if(isDebugGridLogic)
        {
            debugGrid = new Grid<GameGridCell, GameGridCellData>.DebugGrid();
        }

        FpsDebug.SetActive(isDebugFps);
        LogDebug.SetActive(isDebugLog);
    }
}
