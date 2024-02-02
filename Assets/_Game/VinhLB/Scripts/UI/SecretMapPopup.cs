using _Game.Managers;
using UnityEngine;

namespace VinhLB
{
    public class SecretMapPopup : UICanvas
    {
        [SerializeField]
        HButton[] levelButtons;

        public override void UIUpdate()
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
        }
    }
}