using _Game.DesignPattern;
using _Game.GameGrid;
using _Game.Utilities.Grid;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugManager : SimpleSingleton<DebugManager>
{
    private Grid<GameGridCell, GameGridCellData>.DebugGrid debugGrid;

    private void Awake()
    {
        debugGrid = new Grid<GameGridCell, GameGridCellData>.DebugGrid();
    }
    public void DebugGridData(Grid<GameGridCell, GameGridCellData> grid)
    {
        debugGrid.DrawGrid(grid, true);
    }
}
