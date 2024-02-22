using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadStart : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    Slider slider;
    void Start()
    {
        SceneGameManager.Ins.LoadingSceneAsync(2);
        SceneGameManager.Ins._OnLoadingScene += Loading;
    }

    void Loading(int id, float progress)
    {
        slider.value = progress;
    }

    private void OnDestroy()
    {
        SceneGameManager.Ins._OnLoadingScene -= Loading;
    }
}
