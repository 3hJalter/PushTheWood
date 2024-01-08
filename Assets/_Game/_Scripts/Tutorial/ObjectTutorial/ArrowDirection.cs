using System;
using DG.Tweening;
using UnityEngine;

namespace _Game._Scripts.Tutorial.ObjectTutorial
{
    public class ArrowDirection : BaseObjectTutorial
    {
        [SerializeField] private RectTransform rectImg;
        private const float HEIGHT_PER_CELL = 400;
        private Tween tween;
        
        public void PointerToHeight(int cellNums, bool isLoop, float timeTransitionPerCell = 1f)
        {
            // rectImg.sizeDelta = new Vector2(rectImg.siz
            tween = DOVirtual.Float(0, cellNums * HEIGHT_PER_CELL, cellNums * timeTransitionPerCell, value =>
            {
                rectImg.sizeDelta = new Vector2(rectImg.sizeDelta.x, value);
            }).SetEase(Ease.OutQuad).SetLoops(isLoop ? -1 : 0);
        }

        public void OnDestroy()
        {
            tween?.Kill();
        }

        public void OnDisable()
        {
            tween?.Kill();
        }
    }
}
