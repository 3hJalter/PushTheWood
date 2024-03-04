using System.Collections.Generic;
using System.Globalization;
using _Game.Managers;
using _Game.Resource;
using _Game.Utilities;
using TMPro;
using UnityEngine;

namespace VinhLB
{
    public class PetPage : TabPage
    {
        [SerializeField]
        private CollectionItem _collectionItemPrefab;
        [SerializeField]
        private Transform _collectionItemParentTF;
        [SerializeField]
        private Transform _playerSkinParentTF;
        [SerializeField]
        private GameObject[] _playerSkins;
        [SerializeField]
        private HButton _buyButton;
        [SerializeField]
        private TMP_Text _costText;

        private List<CollectionItem> _collectionItemList;
        private int _currentPetIndex = 0;

        public override void Setup(object param = null)
        {
            base.Setup(param);

            _currentPetIndex = DataManager.Ins.CurrentPlayerSkinIndex;

            if (_collectionItemList == null)
            {
                _collectionItemList = new List<CollectionItem>();

                foreach (KeyValuePair<CharacterType, UIResourceConfig> element
                         in DataManager.Ins.UIResourceDatabase.CharacterResourceConfigDict)
                {
                    CollectionItem item = Instantiate(_collectionItemPrefab, _collectionItemParentTF);
                    item.Initialize((int)element.Key, (int)element.Key, element.Value.Name,
                        element.Value.IconSprite, DataManager.Ins.ConfigData.CharacterCosts[(int)element.Key]);
                    item._OnClick += OnItemClick;

                    _collectionItemList.Add(item);
                }
                _buyButton.onClick.AddListener(OnBuy);
            }

            for (int i = 0; i < _collectionItemList.Count; i++)
            {
                if (DataManager.Ins.IsCharacterSkinUnlock(_collectionItemList[i].Id))
                {
                    _collectionItemList[i].SetOwned();
                }

                if (i == _currentPetIndex)
                {
                    _collectionItemList[i].SetSelected(true);
                    _collectionItemList[i].SetChosen(true);
                    _playerSkins[i].gameObject.SetActive(true);
                }
                else
                {
                    _collectionItemList[i].SetSelected(false);
                    _collectionItemList[i].SetChosen(false);
                    _playerSkins[i].gameObject.SetActive(false);
                }
            }
        }

        public override void Open(object param = null)
        {
            base.Open(param);

            _playerSkinParentTF.gameObject.SetActive(true);
        }

        public override void Close()
        {
            base.Close();

            _playerSkinParentTF.gameObject.SetActive(false);
        }

        private void OnItemClick(int id, int data)
        {
            if (id == _currentPetIndex) return;

            if (!DataManager.Ins.IsCharacterSkinUnlock(_collectionItemList[id].Data))
            {
                _buyButton.gameObject.SetActive(true);
                _costText.text = DataManager.Ins.ConfigData.CharacterCosts[_collectionItemList[id].Data]
                    .ToString(Constants.VALUE_FORMAT, CultureInfo.InvariantCulture);
            }
            else
            {
                _buyButton.gameObject.SetActive(false);
                if (DataManager.Ins.IsCharacterSkinUnlock(_collectionItemList[_currentPetIndex].Data))
                {
                    _collectionItemList[_currentPetIndex].SetChosen(false);
                    _collectionItemList[id].SetChosen(true);
                    DataManager.Ins.SetCharacterSkinIndex(_collectionItemList[id].Data);
                }
                _collectionItemList[_currentPetIndex].SetChosen(false);
                _collectionItemList[id].SetChosen(true);
                DataManager.Ins.SetCharacterSkinIndex(_collectionItemList[id].Data);
            }

            _collectionItemList[_currentPetIndex].SetSelected(false);
            _playerSkins[_currentPetIndex].gameObject.SetActive(false);
            _currentPetIndex = id;
            _playerSkins[_currentPetIndex].gameObject.SetActive(true);
            _collectionItemList[_currentPetIndex].SetSelected(true);
        }

        public override void UpdateUI()
        {
            base.UpdateUI();

            for (int i = 0; i < _collectionItemList.Count; i++)
            {
                if (DataManager.Ins.IsCharacterSkinUnlock(_collectionItemList[i].Data))
                    _collectionItemList[i].SetOwned();
            }

            if (DataManager.Ins.IsCharacterSkinUnlock(_collectionItemList[_currentPetIndex].Data))
            {
                _buyButton.gameObject.SetActive(false);
            }
            else
            {
                _buyButton.gameObject.SetActive(true);
                _costText.text = DataManager.Ins.ConfigData.CharacterCosts[_collectionItemList[_currentPetIndex].Data]
                    .ToString(Constants.VALUE_FORMAT, CultureInfo.InvariantCulture);
            }
        }

        private void OnDestroy()
        {
            for (int i = 0; i < _collectionItemList.Count; i++)
            {
                _collectionItemList[i]._OnClick -= OnItemClick;
            }
            _buyButton.onClick.RemoveAllListeners();
        }

        private void OnBuy()
        {
            int buyCost = DataManager.Ins.ConfigData.CharacterCosts[_collectionItemList[_currentPetIndex].Data];
            int character = _collectionItemList[_currentPetIndex].Data;

            if (GameManager.Ins.TrySpendGold(buyCost))
            {
                DataManager.Ins.SetUnlockCharacterSkin(character, true);
                GameManager.Ins.PostEvent(_Game.DesignPattern.EventID.OnUpdateUIs);
            }
            else
            {
                //NOTE: Notice not enough money
                DevLog.Log(DevId.Hung, "Not Enough Money To Buy Characters");
            }
        }
    }
}