using System;
using DG.Tweening;
using UnityEngine;

namespace VinhLB
{
    public class SplashScreen : UICanvas
    {
        [SerializeField]
        private CanvasGroup _contentCanvasGroup;

        public event Action OnOpenCallback;

        private void Awake()
        {
            _contentCanvasGroup.alpha = 0;
        }

        public override void Open(object param = null)
        {
            base.Open(param);

            if (param is Action onOpenCallback)
            {
                OnOpenCallback += onOpenCallback;
            }
            
            _contentCanvasGroup.alpha = 0;
            _contentCanvasGroup.DOFade(1f, 0.2f).SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    OnOpenCallback?.Invoke();
                    OnOpenCallback = null;
                });
        }
    }
}