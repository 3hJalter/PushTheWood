using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Grid = _Game.Utilities.HGrid.Grid;

public class TestGrid : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Grid grid = new Grid(10, 10, 10f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
