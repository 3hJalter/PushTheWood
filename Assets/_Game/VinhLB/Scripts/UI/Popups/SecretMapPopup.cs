using _Game.Data;
using _Game.GameGrid;
using _Game.Managers;
using _Game.UIs.Screen;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace VinhLB
{
    public class SecretMapPopup : UICanvas
    {
        [SerializeField]
        private ScrollRect _scrollRect;
        [SerializeField]
        private GameObject _topBlurGO;
        [SerializeField]
        private GameObject _bottomBlurGO;
        [SerializeField]
        private SecretMapItem[] _secretMapItems;
        
        private void Awake()
        {
            for(int i = 0;  i < _secretMapItems.Length; i++)
            {
                _secretMapItems[i]._OnPlayButtonClick += OnPlayButtonClick;
            }
            
            _scrollRect.onValueChanged.AddListener(OnRewardScrollRectValueChanged);
        }
        
        private void OnDestroy()
        {
            for (int i = 0; i < _secretMapItems.Length; i++)
            {
                _secretMapItems[i]._OnPlayButtonClick -= OnPlayButtonClick;
            }
            
            _scrollRect.onValueChanged.RemoveListener(OnRewardScrollRectValueChanged);
        }

        public override void Setup(object param = null)
        {
            base.Setup(param);
            
            _scrollRect.verticalNormalizedPosition = 1f;
            OnRewardScrollRectValueChanged(_scrollRect.normalizedPosition);
        }

        public override void UpdateUI()
        {
            for (int i = 0; i < _secretMapItems.Length; i++)
            {
                if (i < GameManager.Ins.SecretLevelUnlock)
                {
                    _secretMapItems[i].SetButtons(true);
                }
                else
                {
                    _secretMapItems[i].SetButtons(false);
                }
            }
        }

        private void OnPlayButtonClick(int index)
        {
            LevelManager.Ins.OnGoLevel(LevelType.Secret, index);
            UIManager.Ins.CloseAll();
            UIManager.Ins.OpenUI<InGameScreen>();
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