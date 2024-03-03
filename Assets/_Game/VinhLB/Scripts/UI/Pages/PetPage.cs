using System.Collections.Generic;
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
        GameObject[] playerSkins;
        [SerializeField]
        HButton buyBtn;
        [SerializeField]
        TMP_Text costBuyBtnTxt;

        private List<CollectionItem> _collectionItemList;
        private int currentPetIndex = 0;
        public override void Setup(object param = null)
        {
            base.Setup(param);

            if (_collectionItemList == null)
            {
                _collectionItemList = new List<CollectionItem>();

                foreach (KeyValuePair<CharacterType, UIResourceConfig> element
                         in DataManager.Ins.UIResourceDatabase.CharacterResourceConfigDict)
                {
                    CollectionItem item = Instantiate(_collectionItemPrefab, _collectionItemParentTF);
                    item.Initialize(_collectionItemList.Count, (int)element.Key, element.Value.Name, element.Value.IconSprite, DataManager.Ins.ConfigData.CharacterCosts[(int)element.Key]);
                    item._OnClick += OnItemClick;
                    if (DataManager.Ins.IsCharacterSkinUnlock((int)element.Key))
                        item.SetOwned();
                    if ((int)element.Key == DataManager.Ins.CurrentPlayerSkinIndex)
                        item.SetChoosing(true);
                    _collectionItemList.Add(item);
                }
                buyBtn.onClick.AddListener(OnBuy);
            }
            
            _collectionItemList[currentPetIndex].SetSelected(true);
            playerSkins[currentPetIndex].gameObject.SetActive(true);

        }

        private void OnItemClick(int id, int data)
        {
            if (id == currentPetIndex) return;

            
            else if (!DataManager.Ins.IsCharacterSkinUnlock(_collectionItemList[id].Data))
            {
                buyBtn.gameObject.SetActive(true);
                costBuyBtnTxt.text = DataManager.Ins.ConfigData.CharacterCosts[_collectionItemList[id].Data].ToString();
            }
            else
            {
                buyBtn.gameObject.SetActive(false);
                if (DataManager.Ins.IsCharacterSkinUnlock(_collectionItemList[currentPetIndex].Data))
                {
                    _collectionItemList[currentPetIndex].SetChoosing(false);
                    _collectionItemList[id].SetChoosing(true);
                    DataManager.Ins.SetCharacterSkinIndex(_collectionItemList[id].Data);
                }
            }
                
            _collectionItemList[currentPetIndex].SetSelected(false);
            playerSkins[currentPetIndex].gameObject.SetActive(false);
            currentPetIndex = id;
            playerSkins[currentPetIndex].gameObject.SetActive(true);
            _collectionItemList[currentPetIndex].SetSelected(true);
        }

        public override void UpdateUI()
        {
            base.UpdateUI();
            for(int i = 0; i < _collectionItemList.Count; i++)
            {
                if (DataManager.Ins.IsCharacterSkinUnlock(_collectionItemList[i].Data))
                    _collectionItemList[i].SetOwned();
            }

            if (DataManager.Ins.IsCharacterSkinUnlock(_collectionItemList[currentPetIndex].Data))
            {
                buyBtn.gameObject.SetActive(false);
            }
            else
            {
                buyBtn.gameObject.SetActive(true);
                costBuyBtnTxt.text = DataManager.Ins.ConfigData.CharacterCosts[_collectionItemList[currentPetIndex].Data].ToString();
            }
        }
        private void OnDestroy()
        {
            for (int i = 0; i < _collectionItemList.Count; i++)
            {
                _collectionItemList[i]._OnClick -= OnItemClick;
            }
            buyBtn.onClick.RemoveAllListeners();
        }

        private void OnBuy()
        {
            int buyCost = DataManager.Ins.ConfigData.CharacterCosts[_collectionItemList[currentPetIndex].Data];
            int character = _collectionItemList[currentPetIndex].Data;

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