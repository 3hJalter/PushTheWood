using System;
using _Game.Utilities;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace VinhLB
{
    public class ToggleSwitch : HMonoBehaviour, IPointerClickHandler
    {
        [Header("Setup")]
        [SerializeField]
        private Slider _slider;
        [SerializeField]
        [Range(0f, 1f)]
        private float _sliderValue;
        [SerializeField]
        private CanvasGroup _onTextCanvasGroup;
        [SerializeField]
        private CanvasGroup _offTextCanvasGroup;

        [Header("Animation")]
        [SerializeField]
        private bool _animate = true;
        [ShowIf(nameof(_animate), false)]
        [SerializeField]
        private float _animationDuration = 0.5f;
        [ShowIf(nameof(_animate), false)]
        [SerializeField]
        private Ease _slideEase = Ease.Linear;

        [Header("Events")]
        [SerializeField]
        private UnityEvent _onToggleOn;
        [SerializeField]
        private UnityEvent _onToggleOff;

        private Tween _animateSliderTween;
        
        public bool CurrentValue { get; private set; }

        private void OnValidate()
        {
            SetupSliderComponent();

            _slider.value = _sliderValue;
            
            UpdateTexts(_sliderValue);
        }
        
        private void Awake()
        {
            SetupSliderComponent();
        }
        
        public void OnPointerClick(PointerEventData eventData)
        {
            SetState(!CurrentValue, _animate);
        }
        
        public void SetState(bool state, bool animate)
        {
            CurrentValue = state;

            if (CurrentValue)
            {
                _onToggleOn?.Invoke();
            }
            else
            {
                _onToggleOff?.Invoke();
            }

            float endValue = CurrentValue ? 1f : 0f;
            if (animate)
            {
                if (_animateSliderTween != null)
                {
                    DOTween.Kill(_animateSliderTween);
                }

                float startValue = _slider.value;
                _animateSliderTween = DOVirtual.Float(startValue, endValue, _animationDuration, (value) =>
                {
                    _slider.value = value;

                    UpdateTexts(value);
                }).SetEase(_slideEase).OnComplete(() => { _slider.value = endValue; });
            }
            else
            {
                _slider.value = endValue;
                
                UpdateTexts(endValue);
            }
        }
        
        private void SetupSliderComponent()
        {
            if (_slider == null)
            {
                DevLog.Log(DevId.Vinh, "No slider component found!");
                return;
            }

            _slider.interactable = false;
            ColorBlock sliderColors = _slider.colors;
            sliderColors.disabledColor = Color.white;
            _slider.colors = sliderColors;
            _slider.transition = Selectable.Transition.None;
        }

        private void UpdateTexts(float value)
        {
            if (_onTextCanvasGroup is null || _offTextCanvasGroup is null)
            {
                DevLog.Log(DevId.Vinh, "No on/off text canvas group component found!");
                return;
            }
            
            float normalizedValue = (value - 0.5f) * 2f;
            if (normalizedValue > 0f)
            {
                _offTextCanvasGroup.alpha = 0f;
                _onTextCanvasGroup.alpha = normalizedValue;
            }
            else
            {
                _onTextCanvasGroup.alpha = 0f;   
                _offTextCanvasGroup.alpha = -normalizedValue;
            }
        }
    }
}