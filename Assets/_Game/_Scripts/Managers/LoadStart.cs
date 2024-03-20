using System;
using _Game.Managers;
using UnityEngine;
using VinhLB;

public class LoadStart : MonoBehaviour
{
    private void Awake()
    {
        GleyNotifications.Initialize();
    }

    private void Start()
    {
        UIManager.Ins.OpenUI<LoadingScreen>();
        UIManager.Ins.GetUI<StatusBarScreen>().Close();
        UIManager.Ins.GetUI<HomePage>().Close();
        
        SceneGameManager.Ins.LoadingSceneAsync(2);
        
        // GleyNotifications.SendNotification("Test Title", "Test content", new TimeSpan(0, 1, 0));
    }
}
