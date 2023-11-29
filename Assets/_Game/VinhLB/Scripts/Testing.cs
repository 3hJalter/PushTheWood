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
            _intGrid = new Grid<int>(3, 3, 2f, Vector3.zero, null, GridPlaneType.XZ);
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
                    _intGrid.SetValue(hit.point, 69);
                }
            }
        }
    }
}
