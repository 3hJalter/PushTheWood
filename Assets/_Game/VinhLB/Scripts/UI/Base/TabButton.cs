using System.Collections;
using System.Collections.Generic;
using _Game.Managers;
using _Game.UIs.Popup;
using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace VinhLB
{
    public class TabButton : HMonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        [System.Serializable]
        private enum AnimType
        {
            GoUp = 0,
            Enlarge = 1
        }

        public event System.Action OnButtonClicked;

        [SerializeField]
        private TabGroup _tabGroup;
        [SerializeField]
        private bool _interactable = true;
        [SerializeField]
        private Image _background;
        [SerializeField]
        private GameObject _activeGO;
        [SerializeField]
        private Image _inactiveIcon;
        [SerializeField]
        private Image _activeIcon;
        [SerializeField]
        private TMP_Text _nameText;
        [SerializeField]
        private AnimType _animType;
        // 'GoUp' anim type
        [ShowIf(nameof(_animType), AnimType.GoUp, false)]
        [SerializeField]
        private LayoutElement _layoutElement;
        // 'Enlarge' anim type
        [ShowIf(nameof(_animType), AnimType.Enlarge, false)]
        [SerializeField]
        private Transform _contentTF;

        private bool _active;
        private Transform _activeIconTransform;
        private Tween _activeIconAnimTween;

        public void SetActiveState(bool active, bool animated)
        {
            // if (active == _active)
            // {
            //     return;
            // }

            _active = active;

            if (_activeIconTransform is null)
            {
                _activeIconTransform = _activeIcon.transform;
            }
            _activeIconTransform.DOKill();
            _activeIconAnimTween = null;

            if (_active)
            {
                _activeGO.SetActive(true);

                _inactiveIcon.gameObject.SetActive(false);
                _activeIcon.gameObject.SetActive(true);
                
                if (_animType == AnimType.GoUp)
                {
                    _nameText.gameObject.SetActive(true);

                    _layoutElement.flexibleWidth = 1f;

                    if (animated)
                    {
                        float duration = 0.2f;
                        Sequence sequence = DOTween.Sequence();
                        sequence.AppendCallback(PlayIconAnim)
                            .Join(_activeIconTransform.DOLocalMove(Vector3.up * 40f, duration).SetEase(Ease.OutBack))
                            .Join(_activeIconTransform.DOScale(1.25f, duration).SetEase(Ease.OutBack));
                    }
                    else
                    {
                        _activeIconTransform.localPosition = Vector3.up * 40f;
                        _activeIconTransform.localRotation = Quaternion.identity;
                        _activeIconTransform.localScale = Vector3.one * 1.25f;
                    }
                }
                else if (_animType == AnimType.Enlarge)
                {
                    _nameText.color = Color.white;
                    
                    _contentTF.localScale = Vector3.one * 1.25f;
                }
            }
            else
            {
                _activeGO.SetActive(false);

                _activeIcon.gameObject.SetActive(false);
                _inactiveIcon.gameObject.SetActive(true);
                
                if (_animType == AnimType.GoUp)
                {
                    _nameText.gameObject.SetActive(false);

                    _layoutElement.flexibleWidth = 0f;

                    _activeIconTransform.localPosition = Vector3.zero;
                    _activeIconTransform.localRotation = Quaternion.identity;
                    _activeIconTransform.localScale = Vector3.one;
                }
                else if (_animType == AnimType.Enlarge)
                {
                    _nameText.color = Color.gray;
                    
                    _contentTF.localScale = Vector3.one;
                }
            }
        }

        public void SetBackgroundAlpha(float value)
        {
            Color color = _background.color;
            color.a = Mathf.Clamp01(value);
            _background.color = color;
        }

        public void SetBackgroundSprite(Sprite sprite)
        {
            _background.sprite = sprite;
        }

        public void SetPreferredWidth(float value)
        {
            _layoutElement.preferredWidth = value;
        }

        public void PlayIconAnim()
        {
            if (_activeIconAnimTween != null)
            {
                return;
            }

            _activeIconAnimTween = _activeIconTransform.DOPunchRotation(Vector3.one * 10f, 0.4f, 8, 1f)
                .OnComplete(() => { _activeIconAnimTween = null; });
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (_interactable)
            {
                _tabGroup.OnTabSelected(this, true, false);

                OnButtonClicked?.Invoke();
            }
            else
            {
                UIManager.Ins.OpenUI<NotificationPopup>(Constants.FEATURE_COMING_SOON);
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (_interactable)
            {
                _tabGroup.OnTabEnter(this);
            }
            else
            {
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (_interactable)
            {
                _tabGroup.OnTabExit(this);
            }
            else
            {
            }
        }
    }
}