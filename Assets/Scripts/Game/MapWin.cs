using Game;
using UnityEngine;

public class MapWin : MonoBehaviour
{
    public int Land;
    public bool WinBySkip;

    public void OnTriggerEnter(Collider other)
    {
        LevelManager.Inst.Land = Land;
        LevelManager.Inst.player._uiManager.ShowWin();
        if (WinBySkip)
            LevelManager.Inst.WinBySkip = true;
        else
            LevelManager.Inst.WinBySkip = false;
    }
}
