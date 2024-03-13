using System;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace VinhLB
{
    public class CollectionItem : HMonoBehaviour
    {
        public event Action<int, int> _OnClick;
        
        [SerializeField]
        private HButton _button;
        [SerializeField]
        private TMP_Text _nameText;
        [SerializeField]
        private Image _contentImage;
        [SerializeField]
        private TMP_Text _priceText;
        [SerializeField]
        private GameObject _priceGO;
        [SerializeField]
        private GameObject _selectedGO;
        [SerializeField]
        private GameObject _choosenGO;
        [SerializeField]
        private GameObject _lockedFrameGO;
        [SerializeField]
        private GameObject _lockedIconGO;
        [SerializeField]
        private GameObject _unlockedFrameGO;
        
        
        private int _id;
        private int _data;
        
        public int Id => _id;
        public int Data => _data;

        private void Awake()
        {
            _button.onClick.AddListener(OnClick);
        }

        public void Initialize(int id, int data, string text, Sprite sprite, int price)
        {
            _id = id;
            _data = data;
            _nameText.text = text;
            _contentImage.sprite = sprite;
            _priceText.text = price.ToString(Constants.VALUE_FORMAT, CultureInfo.InvariantCulture);
        }

        private void OnClick()
        {
            _OnClick?.Invoke(_id, _data);
        }

        public void SetSelected(bool value)
        {
            _selectedGO.SetActive(value);
        }

        public void SetOwned()
        {
            _priceGO.SetActive(false);
        }

        public void SetChosen(bool value)
        {
            _choosenGO.SetActive(value);
        }
    }
}