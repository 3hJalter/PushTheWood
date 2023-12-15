using _Game.DesignPattern;
using HControls;
using UnityEngine;

namespace _Game._Scripts.Managers
{
    public class MoveInputManager : Singleton<MoveInputManager>
    {
        [SerializeField] private HSwitch hSwitch;
        [SerializeField] private HDpad dpad;

        private HSwitch HSwitch => hSwitch;
        private GameObject DpadObj => dpad.gameObject;
        
        public MoveChoice CurrentChoice { get; private set; }
        
        public void HideButton()
        {
            HInputManager.SetDefault();
            DpadObj.SetActive(false);
            HSwitch.gameObject.SetActive(false);
        }
        
        public void OnChangeMoveChoice(MoveChoice moveChoice)
        {
            HideButton();
            switch (moveChoice)
            {
                case MoveChoice.DPad:
                    DpadObj.SetActive(true);
                    break;
                case MoveChoice.Switch:
                {
                    HSwitch.gameObject.SetActive(true);
                    HSwitch.HideAllTime(false);
                    break;
                }
                case MoveChoice.Swipe:
                {
                    HSwitch.gameObject.SetActive(true);
                    HSwitch.HideAllTime(true);
                    break;
                }
                CurrentChoice = moveChoice;
            }
        }
        
        public enum MoveChoice
        {
            DPad,
            Switch,
            Swipe
        }
    }
}
