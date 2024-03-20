using System;
using _Game.Managers;
using UnityEngine;
using VinhLB;

public class LoadStart : MonoBehaviour
{
    private void Start()
    {
        UIManager.Ins.OpenUI<LoadingScreen>();
        UIManager.Ins.GetUI<StatusBarScreen>().Close();
        UIManager.Ins.GetUI<HomePage>().Close();
        
        SceneGameManager.Ins.LoadingSceneAsync(2);
    }
}
