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

        public void Initialize(Vector3 position, Vector2 size, Sprite sprite)
        {
            _rectTF.position = position;
            _rectTF.sizeDelta = size;
            _image.sprite = sprite;
        }
    }
}