using _Game._Scripts.Managers;
using UnityEngine;

public class MapWin : MonoBehaviour
{
    public int Land;
    public bool WinBySkip;

    public void OnTriggerEnter(Collider other)
    {
        LevelManager.Ins.landIndex = Land;
        LevelManager.OnWin();
        // LevelManager.Ins.player._uiManager.ShowWin();
        if (WinBySkip)
            LevelManager.Ins.winBySkip = true;
        else
            LevelManager.Ins.winBySkip = false;
    }
}
