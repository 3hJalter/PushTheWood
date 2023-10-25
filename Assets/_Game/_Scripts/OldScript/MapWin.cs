using _Game.Managers;
using UnityEngine;

public class MapWin : MonoBehaviour
{
    public int Land;
    public bool WinBySkip;

    public void OnTriggerEnter(Collider other)
    {
        OldLevelManager.Ins.landIndex = Land;
        OldLevelManager.OnWin();
        // LevelManager.Ins.player._uiManager.ShowWin();
        if (WinBySkip)
            OldLevelManager.Ins.winBySkip = true;
        else
            OldLevelManager.Ins.winBySkip = false;
    }
}
