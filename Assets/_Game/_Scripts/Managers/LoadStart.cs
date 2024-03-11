using System.Collections;
using System.Collections.Generic;
using _Game.Managers;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
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
