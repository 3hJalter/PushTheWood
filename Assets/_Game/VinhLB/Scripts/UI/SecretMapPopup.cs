using _Game.Managers;
using TMPro;
using UnityEngine;

namespace VinhLB
{
    public class SecretMapPopup : UICanvas
    {
        [SerializeField]
        private SecretMapItem[] _secretMapItems;

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
    }
}