using _Game.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace VinhLB
{
    public class BoosterWatchVideoPopup : UICanvas
    {
        [SerializeField]
        private Image _bossterIcon;
        [SerializeField]
        private TMP_Text _boosterText;
        [SerializeField]
        private Image _currencyIcon;
        [SerializeField]
        private TMP_Text _currencyAmountText;

        public override void Setup()
        {
            base.Setup();
            GameManager.Ins.ChangeState(GameState.Pause);
        }

        public void OnClickBuyButton()
        { 
            Close();   
        }

        public void OnClickClaimButton()
        {
            Close();
        }

        public override void Close()
        {
            GameManager.Ins.ChangeState(GameState.InGame);
            base.Close();
        }
    }
}