using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VinhLB
{
	public class Testing : MonoBehaviour
	{
        private Grid<int> _intGrid;

        private void Start()
        {
            int gridSize = 5;
            float cellSize = 2f;
            _intGrid = new Grid<int>(gridSize, gridSize, cellSize, default, null, GridPlaneType.XZ);
            _intGrid.DrawGrid();
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    //Debug.Log(hit.point);
                    _intGrid.SetCellValue(hit.point, 69);
                }
            }
        }
    }
}
