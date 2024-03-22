using System;
using _Game.Managers;
using UnityEngine;
using VinhLB;

public class LoadStart : MonoBehaviour
{
    private void Start()
    {
        UIManager.Ins.OpenUI<LoadingScreen>();
        
        StatusBarScreen statusBarScreen = UIManager.Ins.GetUI<StatusBarScreen>();
        statusBarScreen.Close();
        
        HomePage homePage = UIManager.Ins.GetUI<HomePage>();
        homePage.Close();
        
        SceneGameManager.Ins.LoadingSceneAsync(2);
    }
}
