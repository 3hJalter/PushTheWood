using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace VinhLB
{
    public class TabButton : HMonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        [SerializeField]
        private TabGroup _tabGroup;
        [SerializeField]
        private Image _background;
        [SerializeField]
        private Image _icon;
        [SerializeField]
        private LayoutElement _layoutElement;

        private bool _active;
        private Transform _iconTransform;

        public void SetActiveState(bool active, bool animated)
        {
            // if (active == _active)
            // {
            //     return;
            // }

            _active = active;

            if (_iconTransform is null)
            {
                _iconTransform = _icon.transform;
            }
            _iconTransform.DOKill();
            
            if (_active)
            {
                SetBackgroundAlpha(0.1f);
                
                if (animated)
                {
                    _iconTransform.DOScale(1.25f, 0.25f).SetEase(Ease.OutBack);
                }
                else
                {
                    _iconTransform.localScale = Vector3.one * 1.25f;
                }

                _layoutElement.flexibleWidth = 1f;
            }
            else
            {
                SetBackgroundAlpha(0f);
                
                _iconTransform.localScale = Vector3.one;
                
                _layoutElement.flexibleWidth = 0f;
            }
        }
        
        public void SetBackgroundAlpha(float value)
        {
            Color color = _background.color;
            color.a = Mathf.Clamp01(value);
            _background.color = color;
        }

        public void ChangeBackgroundSprite(Sprite sprite)
        {
            _background.sprite = sprite;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            _tabGroup.OnTabSelected(this, true);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _tabGroup.OnTabEnter(this);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _tabGroup.OnTabExit(this);
        }
    }
}
