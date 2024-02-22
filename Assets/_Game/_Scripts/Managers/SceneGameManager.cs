using _Game.DesignPattern;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[DefaultExecutionOrder(-10)]
public class SceneGameManager : Singleton<SceneGameManager>
{
    public event Action<int ,float> _OnLoadingScene;
    private int id = 0;
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void LoadingSceneAsync(int id)
    {
        StartCoroutine(LoadSceneAsyncCoroutine(id));
    }
    IEnumerator LoadSceneAsyncCoroutine(int id)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(id);
        this.id = id;
        while (!asyncLoad.isDone)
        {
            float loadingPercentage = Mathf.Clamp01(asyncLoad.progress / 0.9f);
            _OnLoadingScene?.Invoke(this.id, loadingPercentage);
            // Update your loading UI with the percentage
            yield return null;
        }
        // Scene is fully loaded; hide loading UI
    }
}
