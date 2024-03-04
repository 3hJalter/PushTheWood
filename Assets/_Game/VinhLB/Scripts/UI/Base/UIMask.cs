using System;
using UnityEngine;
using UnityEngine.UI;

namespace VinhLB
{
    public class UIMask : HMonoBehaviour
    {
        [SerializeField]
        private RectTransform _rectTF;
        [SerializeField]
        private Image _image;

        private IClickable _clickableItem;
        private Action _onClickedCallback;

        public void Initialize(Vector3 position, Vector2 size, Sprite sprite, IClickable clickableItem,
            Action onClickedCallback)
        {
            _rectTF.position = position;
            _rectTF.sizeDelta = size;
            _image.sprite = sprite;
            _clickableItem = clickableItem;
            _onClickedCallback = onClickedCallback;

            if (_clickableItem != null)
            {
                _clickableItem.OnClickedCallback += _onClickedCallback;
            }
        }

        public void Open()
        {
            gameObject.SetActive(true);
        }

        public void Close()
        {
            if (_clickableItem != null)
            {
                _clickableItem.OnClickedCallback -= _onClickedCallback;
            }

            gameObject.SetActive(false);
        }
    }
}