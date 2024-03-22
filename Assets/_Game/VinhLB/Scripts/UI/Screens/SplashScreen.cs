using System.Collections;
using DG.Tweening;
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
        private Coroutine _delayCloseCoroutine;

        public override void Setup(object param = null)
        {
            base.Setup(param);
            
            UpdateBackground();
        }

        public override void Open(object param = null)
        {
            base.Open(param);

            _canvasGroup.alpha = 1f;

            if (_delayCloseCoroutine != null)
            {
                StopCoroutine(_delayCloseCoroutine);
            }
            
            float delayTime = param != null ? (float)param : 1f;
            _delayCloseCoroutine = StartCoroutine(DelayCloseCoroutine(delayTime));
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

        private IEnumerator DelayCloseCoroutine(float delayTime)
        {
            yield return new WaitForSeconds(delayTime);
            
            DOVirtual.Float(1f, 0f, 0.25f, value => _canvasGroup.alpha = value)
                .OnComplete(Close);
            
            _delayCloseCoroutine = null;
        }
    }
}