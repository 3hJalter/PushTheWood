using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using MEC;
using UnityEngine;
using UnityEngine.UI;

namespace VinhLB
{
    public class SplashScreen : UICanvas
    {
        [SerializeField]
        private CanvasGroup _canvasGroup;
        [SerializeField]
        private RectTransform _backgroundRectTF;

        private bool _isFirstTime = true;
        private bool _isWidthFitting = false;
        private CoroutineHandle _delayCloseCoroutine;

        public event Action OnCloseCallback;
        public event Action OnOpenCallback;
        
        public override void Setup(object param = null)
        {
            base.Setup(param);
            
            UpdateBackground();
        }

        public override void Open(object param = null)
        {
            base.Open(param);
            
            _canvasGroup.alpha = 1f;

            // Stop the previous coroutine if it's still running
            if (Timing.IsRunning(_delayCloseCoroutine))
            {
                Timing.KillCoroutines(_delayCloseCoroutine);
            }
            
            float delayTime = param != null ? (float)param : 1f;
            _delayCloseCoroutine = Timing.RunCoroutine(DelayCloseCoroutine(delayTime));
        }

        public override void Close()
        {
            OnCloseCallback?.Invoke();
            OnCloseCallback = null;
            base.Close();
        }

        private void UpdateBackground()
        {
            float currentScreenRatio = (float)Screen.width / Screen.height;
            // DevLog.Log(DevId.Vinh, $"{Constants.REF_SCREEN_RATIO} | {currentScreenRatio}");
            if (currentScreenRatio >= Constants.REF_SCREEN_RATIO && (_isFirstTime || !_isWidthFitting))
            {
                // Fit width
                _backgroundRectTF.anchorMin = new Vector2(0, 0.5f);
                _backgroundRectTF.anchorMax = new Vector2(1f, 0.5f);
                _backgroundRectTF.SetPadding(0, 0);
                _backgroundRectTF.SetSizeDeltaHeight(2500f);
                _isFirstTime = false;
                _isWidthFitting = true;
            }
            else if (currentScreenRatio < Constants.REF_SCREEN_RATIO && (_isFirstTime || _isWidthFitting))
            {
                // Fit height
                _backgroundRectTF.anchorMin = new Vector2(0.5f, 0);
                _backgroundRectTF.anchorMax = new Vector2(0.5f, 1f);
                _backgroundRectTF.SetPadding(0, 0);
                _backgroundRectTF.SetSizeDeltaWidth(1500f);
                _isFirstTime = false;
                _isWidthFitting = false;
            }
        }

        private IEnumerator<float> DelayCloseCoroutine(float delayTime)
        {
            // Wait a frame to make sure the OnOpenCallback is set
            yield return Timing.WaitForOneFrame;
            OnOpenCallback?.Invoke();
            OnOpenCallback = null;
            yield return Timing.WaitForSeconds(delayTime);
            
            DOVirtual.Float(1f, 0f, 0.25f, value => _canvasGroup.alpha = value)
                .OnComplete(Close);
        }
    }
}