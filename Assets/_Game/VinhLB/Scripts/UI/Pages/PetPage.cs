using System;
using System.Collections.Generic;
using System.Globalization;
using _Game.GameGrid;
using _Game.Managers;
using _Game.Resource;
using _Game.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace VinhLB
{
    public class PetPage : TabPage
    {
        [SerializeField]
        private CollectionItem _collectionItemPrefab;
        [SerializeField]
        private ScrollRect _scrollRect;
        [SerializeField]
        private GameObject _topBlurGO;
        [SerializeField]
        private GameObject _bottomBlurGO;
        [SerializeField]
        private Transform _collectionItemParentTF;
        [SerializeField]
        private HButton _buyButton;
        [SerializeField]
        private TMP_Text _costText;
        [SerializeField]
        private GameObject _infoGO;
        [SerializeField]
        private TMP_Text _infoText;

        private List<CollectionItem> _collectionItemList;
        private int _currentPetIndex = 0;

        private void Awake()
        {
            _scrollRect.onValueChanged.AddListener(OnRewardScrollRectValueChanged);
        }

        private void OnDestroy()
        {
            for (int i = 0; i < _collectionItemList.Count; i++)
            {
                _collectionItemList[i]._OnClick -= OnItemClick;
            }
            _buyButton.onClick.RemoveAllListeners();

            _scrollRect.onValueChanged.RemoveListener(OnRewardScrollRectValueChanged);
        }

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
                    _collectionItemList[i].ShowPrice(false);
                }
                else
                {
                    // Lock Weeny until player receives on day 7
                    if (i == (int)CharacterType.Weeny)
                    {
                        _collectionItemList[i].SetLocked(true, "Receive in day 7");
                        _collectionItemList[i].ShowPrice(false);
                    }
                    else
                    {
                        _collectionItemList[i].SetLocked(false);
                        _collectionItemList[i].ShowPrice(true);
                    }
                }

                if (i == _currentPetIndex)
                {
                    _collectionItemList[i].SetSelected(true);
                    _collectionItemList[i].SetChosen(true);
                }
                else
                {
                    _collectionItemList[i].SetSelected(false);
                    _collectionItemList[i].SetChosen(false);
                }
            }

            _scrollRect.verticalNormalizedPosition = 1f;
            OnRewardScrollRectValueChanged(_scrollRect.normalizedPosition);
        }

        public override void Close()
        {
            base.Close();

            LevelManager.Ins.player.ChangeSkin(DataManager.Ins.CurrentPlayerSkinIndex);
        }

        private void OnItemClick(int id, int data)
        {
            if (id == _currentPetIndex)
            {
                return;
            }

            if (!DataManager.Ins.IsCharacterSkinUnlock(_collectionItemList[id].Data))
            {
                if (!_collectionItemList[id].IsLocked)
                {
                    _buyButton.gameObject.SetActive(true);
                    _costText.text = DataManager.Ins.ConfigData.CharacterCosts[_collectionItemList[id].Data]
                        .ToString(Constants.VALUE_FORMAT, CultureInfo.InvariantCulture);
                    _infoGO.SetActive(false);
                }
                else
                {
                    _buyButton.gameObject.SetActive(false);
                    _infoGO.SetActive(true);
                    _infoText.text = _collectionItemList[id].UnlockInfo;
                }
            }
            else
            {
                _buyButton.gameObject.SetActive(false);
                _infoGO.SetActive(false);
                _collectionItemList[DataManager.Ins.CurrentPlayerSkinIndex].SetChosen(false);
                _collectionItemList[id].SetChosen(true);
                DataManager.Ins.SetCharacterSkinIndex(_collectionItemList[id].Data);
            }

            _collectionItemList[_currentPetIndex].SetSelected(false);
            _currentPetIndex = id;
            LevelManager.Ins.player.ChangeSkin(data);
            _collectionItemList[_currentPetIndex].SetSelected(true);
        }

        public override void UpdateUI()
        {
            base.UpdateUI();

            for (int i = 0; i < _collectionItemList.Count; i++)
            {
                if (DataManager.Ins.IsCharacterSkinUnlock(_collectionItemList[i].Data))
                {
                    _collectionItemList[i].ShowPrice(false);
                }
            }

            if (DataManager.Ins.IsCharacterSkinUnlock(_collectionItemList[_currentPetIndex].Data))
            {
                _buyButton.gameObject.SetActive(false);
                _infoGO.SetActive(false);
            }
            else
            {
                if (!_collectionItemList[_currentPetIndex].IsLocked)
                {
                    _buyButton.gameObject.SetActive(true);
                    _costText.text = DataManager.Ins.ConfigData
                        .CharacterCosts[_collectionItemList[_currentPetIndex].Data]
                        .ToString(Constants.VALUE_FORMAT, CultureInfo.InvariantCulture);
                    _infoGO.SetActive(false);
                }
                else
                {
                    _buyButton.gameObject.SetActive(false);
                    _infoGO.SetActive(true);
                    _infoText.text = _collectionItemList[_currentPetIndex].UnlockInfo;
                }
            }
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

        private void OnRewardScrollRectValueChanged(Vector2 value)
        {
            if (_scrollRect.verticalNormalizedPosition < 0.05f)
            {
                _bottomBlurGO.SetActive(false);
            }
            else
            {
                _bottomBlurGO.SetActive(true);
            }

            if (_scrollRect.verticalNormalizedPosition > 0.95f)
            {
                _topBlurGO.SetActive(false);
            }
            else
            {
                _topBlurGO.SetActive(true);
            }
        }
    }
}