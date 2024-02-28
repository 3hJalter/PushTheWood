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
        private TMP_Text _priceAmountText;
        [SerializeField]
        private GameObject _priceGO;
        [SerializeField]
        private GameObject _activeGO;

        public void Initialize()
        {
            
        }
    }
}