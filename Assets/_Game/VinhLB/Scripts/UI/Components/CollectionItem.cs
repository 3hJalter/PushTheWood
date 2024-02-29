using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace VinhLB
{
    public class CollectionItem : HMonoBehaviour
    {
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

        public void Initialize(string text, Sprite sprite, int price)
        {
            _nameText.text = text;
            _contentImage.sprite = sprite;
            _priceText.text = price.ToString(Constants.VALUE_FORMAT);
        }
    }
}