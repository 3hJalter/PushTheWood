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
        
        SceneGameManager.Ins.LoadingSceneAsync(2);
    }
}
