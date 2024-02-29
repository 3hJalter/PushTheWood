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

        private List<CollectionItem> _collectionItemList;

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
                    item.Initialize(element.Value.Name, element.Value.IconSprite, 0);
                    
                    _collectionItemList.Add(item);
                }
            }
        }
    }
}