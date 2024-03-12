using System;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Video;

namespace _Game._Scripts.UIs.Component
{
    public class VideoTutorial : MonoBehaviour
    {
        [Title("Video Player")]
        [SerializeField] private VideoPlayer videoPlayer;

        private int _numberVideoLoop;
        private int _currentVideoLoop;

        public event Action OnCallback;
        
        private void Awake()
        {
            videoPlayer.loopPointReached += EndReached;
        }

        private void OnDestroy()
        {
            videoPlayer.loopPointReached -= EndReached;
        }
        
        
        public void Play(int numberVideoLoop = 1, Action callback = null)
        {
            if (!gameObject.activeSelf) gameObject.SetActive(true);
            _numberVideoLoop = numberVideoLoop;
            OnCallback = callback;
            videoPlayer.Play();
        }
        
        public void Stop()
        {
            videoPlayer.Stop();
            _currentVideoLoop = 0;
            if (gameObject.activeSelf) gameObject.SetActive(false);
        }
        
        private void EndReached(VideoPlayer source)
        {
            if (_currentVideoLoop < _numberVideoLoop)
            {
                _currentVideoLoop++;
                videoPlayer.Play();
            }
            else
            {
                Stop();
                OnCallback?.Invoke();
            }
        }
    }
}
