using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace VinhLB
{
    public class CollectionItem : HMonoBehaviour
    {
        public event Action<int, int> _OnClick;
        [SerializeField]
        int id;
        int data;
        [SerializeField]
        private Button _button;
        [SerializeField]
        private TMP_Text _nameText;
        [SerializeField]
        private Image _contentImage;
        [SerializeField]
        private TMP_Text _priceText;
        [SerializeField]
        private GameObject _priceGO;
        [SerializeField]
        private GameObject _activeGO;
        [SerializeField]
        private GameObject _selectedGO;
        public int Data => data;

        private void Awake()
        {
            _button.onClick.AddListener(OnClick);
        }
        public void Initialize(int id, int data,string text, Sprite sprite, int price)
        {
            this.id = id;
            this.data = data;
            _nameText.text = text;
            _contentImage.sprite = sprite;
            _priceText.text = price.ToString(Constants.VALUE_FORMAT);
        }

        private void OnClick()
        {
            _OnClick?.Invoke(id, data);
        }
        public void SetSelected(bool value)
        {
            _activeGO.SetActive(value);
        }
        public void SetOwned()
        {
            _priceGO.SetActive(false);
        }
        public void SetChoosing(bool value)
        {
            _selectedGO.SetActive(value);
        }
    }
}