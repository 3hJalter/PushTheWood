using _Game.Data;
using _Game.GameGrid;
using _Game.Managers;
using _Game.UIs.Screen;
using TMPro;
using UnityEngine;

namespace VinhLB
{
    public class SecretMapPopup : UICanvas
    {
        [SerializeField]
        private SecretMapItem[] _secretMapItems;

        private void Awake()
        {
            for(int i = 0;  i < _secretMapItems.Length; i++)
            {
                _secretMapItems[i]._OnPlayButtonClick += OnPlayButtonClick;
            }
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
            
            // secretMapPieceTxt.text = GameManager.Ins.SecretMapPieces.ToString();
        }

        private void OnPlayButtonClick(int index)
        {
            LevelManager.Ins.OnGoLevel(LevelType.Secret, index);
            UIManager.Ins.CloseAll();
            UIManager.Ins.OpenUI<InGameScreen>();
        }

        private void OnDestroy()
        {
            for (int i = 0; i < _secretMapItems.Length; i++)
            {
                _secretMapItems[i]._OnPlayButtonClick -= OnPlayButtonClick;
            }
        }
    }
}