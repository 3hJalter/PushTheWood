using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace VinhLB
{
    public class LoadingScreen : UICanvas
    {
        [SerializeField]
        private Slider _progressSlider;
        [SerializeField]
        private TMP_Text _progressText;
        [SerializeField]
        private TMP_Text _loadingText;

        private Tween _loadingTextTween;

        public override void Setup(object param = null)
        {
            base.Setup(param);
            
            SceneGameManager.Ins.OnLoadingScene += SceneGameManager_OnLoadingScene;
            SceneGameManager.Ins.OnSceneLoaded += SceneGameManager_OnSceneLoaded;

            UpdateUI(0f);
        }

        public override void Open(object param = null)
        {
            base.Open(param);

            if (_loadingTextTween == null)
            {
                _loadingTextTween = DOVirtual.Int(0, 2, 0.5f, (value) =>
                {
                    string loadingText = "Loading.";
                    for (int i = 0; i < value; i++)
                    {
                        loadingText += ".";
                    }
                    _loadingText.text = loadingText;
                }).SetLoops(-1, LoopType.Restart);
            }
            else
            {
                _loadingTextTween.Restart();
            }
        }

        private void UpdateUI(float progress)
        {
            _progressSlider.value = progress;
            _progressText.text = $"{Mathf.RoundToInt(progress * 100f)}%";
        }

        private void SceneGameManager_OnLoadingScene(int id, float progress)
        {
            UpdateUI(progress);
        }
        
        private void SceneGameManager_OnSceneLoaded(int id)
        {
            SceneGameManager.Ins.OnLoadingScene -= SceneGameManager_OnLoadingScene;
            SceneGameManager.Ins.OnSceneLoaded -= SceneGameManager_OnSceneLoaded;

            _loadingTextTween.Pause();
            
            Close();
        }
    }
}