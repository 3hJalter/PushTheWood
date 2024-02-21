using System;
using _Game.Utilities;
using DG.Tweening;
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
        private CanvasGroup _onTextCanvasGroup;
        [SerializeField]
        private CanvasGroup _offTextCanvasGroup;
        [SerializeField]
        [Range(0f, 1f)]
        private float _sliderValue;

        [Header("Animation")]
        [SerializeField]
        private float _animationDuration = 0.5f;
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
            Toggle();
        }

        private void SetupSliderComponent()
        {
            if (_slider == null)
            {
                DevLog.Log(DevId.Vinh, "No slider found!");
                return;
            }

            _slider.interactable = false;
            ColorBlock sliderColors = _slider.colors;
            sliderColors.disabledColor = Color.white;
            _slider.colors = sliderColors;
            _slider.transition = Selectable.Transition.None;
        }

        private void Toggle()
        {
            SetStateAndStartAnimation(!CurrentValue);
        }

        private void SetStateAndStartAnimation(bool state)
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

            if (_animateSliderTween != null)
            {
                DOTween.Kill(_animateSliderTween);
            }

            float startValue = _slider.value;
            float endValue = CurrentValue ? 1f : 0f;
            
            _animateSliderTween = DOVirtual.Float(startValue, endValue, _animationDuration, (value) =>
            {
                _slider.value = value;

                UpdateTexts(value);
            }).SetEase(_slideEase).OnComplete(() => { _slider.value = endValue; });
        }

        private void UpdateTexts(float value)
        {
            if (_onTextCanvasGroup == null || _offTextCanvasGroup == null)
            {
                DevLog.Log(DevId.Vinh, "No on/off text canvas group found!");
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