using _Game.Managers;
using TMPro;
using UnityEngine;

namespace VinhLB
{
    public class SecretMapPopup : UICanvas
    {
        [SerializeField]
        HButton[] levelButtons;
        [SerializeField]
        TMP_Text secretMapPieceTxt;

        public override void UpdateUI()
        {
            for(int i = 0; i < levelButtons.Length; i++) 
            { 
                if(i < GameManager.Ins.SecretLevelUnlock)
                {
                    levelButtons[i].interactable = true;
                }
                else
                {
                    levelButtons[i].interactable = false;
                }
            }
            secretMapPieceTxt.text = GameManager.Ins.SecretMapPieces.ToString();
        }
    }
}