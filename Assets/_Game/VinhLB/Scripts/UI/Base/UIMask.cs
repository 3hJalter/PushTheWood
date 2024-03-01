using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace VinhLB
{
    public class UIMask : HMonoBehaviour
    {
        [SerializeField]
        private RectTransform _rectTF;
        [SerializeField]
        private Image _image;

        private Button _button;
        private UnityAction _onClickButton;

        public void Initialize(Vector3 position, Vector2 size, Sprite sprite, Button button, UnityAction onClickButton)
        {
            _rectTF.position = position;
            _rectTF.sizeDelta = size;
            _image.sprite = sprite;
            _button = button;
            _onClickButton = onClickButton;

            if (_button != null)
            {
                _button.onClick.AddListener(_onClickButton);
            }
        }

        public void Open()
        {
            gameObject.SetActive(true);
        }

        public void Close()
        {
            if (_button != null)
            {
                _button.onClick.RemoveListener(_onClickButton);
            }
            
            gameObject.SetActive(false);
        }
    }
}