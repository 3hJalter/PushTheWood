using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICell<T>
{
    public void SetGridPosition(int x, int y);
    public (int,int) GetGridPosition();
    public abstract string ToString();
    public T GetCellValue();
    public void SetCellValue(T value);
}
