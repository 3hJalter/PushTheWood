using System.Collections.Generic;
using _Game.Managers;
using _Game.Resource;
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
                    item.Initialize( _collectionItemList.Count, element.Value.Name, element.Value.IconSprite, DataManager.Ins.ConfigData.CharacterCosts[(int)element.Key]);
                    item._OnClick += OnItemClick;
                    _collectionItemList.Add(item);
                }
            }
            _collectionItemList[currentPetIndex].SetSelected(true);
            playerSkins[currentPetIndex].gameObject.SetActive(true);
        }

        private void OnItemClick(int id)
        {
            _collectionItemList[currentPetIndex].SetSelected(false);
            playerSkins[currentPetIndex].gameObject.SetActive(false);
            currentPetIndex = id;
            playerSkins[currentPetIndex].gameObject.SetActive(true);
            _collectionItemList[currentPetIndex].SetSelected(true);
        }

        private void OnDestroy()
        {
            for(int i = 0; i <  _collectionItemList.Count; i++)
            {
                _collectionItemList[i]._OnClick -= OnItemClick;
            }
        }
    }
}