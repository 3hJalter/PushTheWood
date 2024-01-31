using System;
using System.Collections;
using System.Collections.Generic;
using _Game.Managers;
using Sirenix.OdinInspector;
using UnityEngine;

namespace VinhLB
{
    public class TabGroup : HMonoBehaviour
    {
        [SerializeField]
        private List<TabButton> _tabButtonList;
        [SerializeField]
        private int _activeTabIndex;
        [SerializeField]
        private float _inactiveTabButtonWidth = 300f;
        [SerializeField]
        private bool _useCustomSprite;
        [ShowIf(nameof(_useCustomSprite), false)]
        [SerializeField]
        private Sprite _tabIdleSprite;
        [ShowIf(nameof(_useCustomSprite), false)]
        [SerializeField]
        private Sprite _tabHoverSprite;
        [ShowIf(nameof(_useCustomSprite), false)]
        [SerializeField]
        private Sprite _tabActiveSprite;

        private Dictionary<TabButton, TabPage> _tabGroupDict = new Dictionary<TabButton, TabPage>();
        private TabButton _selectedTabButton;

        private void Awake()
        {
            if (_tabButtonList.Count > 0)
            {
                for (int i = 0; i < _tabButtonList.Count; i++)
                {
                    _tabButtonList[i].SetPreferredWidth(_inactiveTabButtonWidth);
                }
                
                if (_activeTabIndex < 0 || _activeTabIndex >= _tabButtonList.Count)
                {
                    _activeTabIndex = 0;
                }
            }
        }

        public void ResetSelectedTab()
        {
            if (_tabButtonList.Count > 0)
            {
                OnTabSelected(_tabButtonList[_activeTabIndex], false, true);
            }
        }
        
        public void ClearSelectedTab()
        {
            if (_tabButtonList.Count > 0)
            {
                OnTabSelected(null, false, false);
            }
        }

        public void OnTabEnter(TabButton button)
        {
            // ResetTabs();
            //
            // if (_selectedTabButton == null || button != _selectedTabButton)
            // {
            //     if (!_useCustomSprite)
            //     {
            //         button.SetBackgroundAlpha(0.025f);
            //     }
            //     else
            //     {
            //         button.ChangeBackgroundSprite(_tabHoverSprite);
            //     }
            // }
        }

        public void OnTabExit(TabButton button)
        {
            // ResetTabs();
        }

        public void OnTabSelected(TabButton button, bool buttonAnimated, bool pageAnimated)
        {
            if (button == _selectedTabButton)
            {
                return;
            }

            _selectedTabButton = button;

            ResetTabs();

            if (_selectedTabButton != null)
            {
                if (!_useCustomSprite)
                {
                    _selectedTabButton.SetActiveState(true, buttonAnimated);
                }
                else
                {
                    _selectedTabButton.SetBackgroundSprite(_tabActiveSprite);
                }
                
                if (!_tabGroupDict.ContainsKey(_selectedTabButton))
                {
                    int index = _tabButtonList.IndexOf(_selectedTabButton);
                    _tabGroupDict[_selectedTabButton] = GetTabPage((TabPageType)index);
                }
            }
            
            foreach (KeyValuePair<TabButton, TabPage> element in _tabGroupDict)
            {
                if (element.Key == _selectedTabButton)
                {
                    element.Value.Open(pageAnimated);
                }
                else
                {
                    element.Value.Close();
                }
            }
        }

        private void ResetTabs()
        {
            for (int i = 0; i < _tabButtonList.Count; i++)
            {
                if (_tabButtonList[i] == _selectedTabButton)
                {
                    continue;
                }

                if (!_useCustomSprite)
                {
                    _tabButtonList[i].SetActiveState(false, false);
                }
                else
                {
                    _tabButtonList[i].SetBackgroundSprite(_tabIdleSprite);
                }
            }
        }

        private TabPage GetTabPage(TabPageType type)
        {
            switch (type)
            {
                case TabPageType.Shop:
                    return UIManager.Ins.GetUI<ShopPage>();
                case TabPageType.Home:
                    return UIManager.Ins.GetUI<HomePage>();
                case TabPageType.Inventory:
                    return UIManager.Ins.GetUI<InventoryPage>();
            }

            return null;
        }
    }
    
    public enum TabPageType
    {
        Shop = 0,
        Home = 1,
        Inventory = 2
    }
}
