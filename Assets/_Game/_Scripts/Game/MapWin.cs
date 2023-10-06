using _Game;
using UnityEngine;

public class MapWin : MonoBehaviour
{
    public int Land;
    public bool WinBySkip;

    public void OnTriggerEnter(Collider other)
    {
        LevelManager.Ins.landIndex = Land;
        LevelManager.Ins.OnWin();
        // LevelManager.Ins.player._uiManager.ShowWin();
        if (WinBySkip)
            LevelManager.Ins.winBySkip = true;
        else
            LevelManager.Ins.winBySkip = false;
    }
}
