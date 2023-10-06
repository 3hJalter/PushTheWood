using _Game;
using UnityEngine;

public class MapWin : MonoBehaviour
{
    public int Land;
    public bool WinBySkip;

    public void OnTriggerEnter(Collider other)
    {
        LevelManager.Ins.Land = Land;
        LevelManager.Ins.OnWin();
        // LevelManager.Ins.player._uiManager.ShowWin();
        if (WinBySkip)
            LevelManager.Ins.WinBySkip = true;
        else
            LevelManager.Ins.WinBySkip = false;
    }
}
