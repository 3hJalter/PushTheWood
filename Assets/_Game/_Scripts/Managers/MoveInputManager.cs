using System.Collections.Generic;
using _Game.DesignPattern;
using _Game.Managers;
using HControls;
using MEC;
using UnityEngine;

namespace _Game._Scripts.Managers
{
    public class MoveInputManager : Singleton<MoveInputManager>
    {
        public enum MoveChoice
        {
            Swipe,
            DPad,
            Switch,
            SwipeContinuous,
        }

        [SerializeField] private GameObject container;
        [SerializeField] private HSwitch hSwitch;
        [SerializeField] private HDpad dpad;
        [SerializeField] private HSwipe hSwipe;

        public HSwipe HSwipe => hSwipe;

        public HDpad Dpad => dpad;
        private HSwitch HSwitch => hSwitch;
        private GameObject DpadObj => dpad.gameObject;

        private MoveChoice CurrentChoice { get; set; }
        
        public void OnForceResetMove()
        {
            if (CurrentChoice == MoveChoice.DPad)
            {
                int index = (int) HInputManager.GetDirectionInput();
                if (index != -1) dpad.OnButtonPointerUp(index);
            } else HInputManager.SetDirectionInput(Direction.None);
        }
        
        public void WaitToForceResetMove(float time)
        {
            Timing.RunCoroutine(WaitToResetMove(time));
        }
        
        private IEnumerator<float> WaitToResetMove(float time)
        {
            yield return time;
            OnForceResetMove();
        }
        
        private void HideButton(bool isSetDefault = true)
        {
            if (isSetDefault) HInputManager.SetDefault();
            DpadObj.SetActive(false);
            HSwitch.gameObject.SetActive(false);
            hSwipe.gameObject.SetActive(false);
        }

        public void OnChangeMoveChoice(MoveChoice moveChoice, bool isSetDefault = true)
        {
            HideButton(isSetDefault);
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
                    hSwipe.gameObject.SetActive(true);
                    break;
                }
                case MoveChoice.SwipeContinuous:
                {
                    HSwitch.gameObject.SetActive(true);
                    HSwitch.HideAllTime(true);
                    break;
                }
            }

            CurrentChoice = moveChoice;
            DataManager.Ins.GameData.setting.moveChoice = (int) moveChoice;
        }
        
        private bool _isFirstOpen;
        public void ShowContainer(bool isShow, bool isSetDefault = true)
        {
            container.SetActive(isShow);
            if (!isShow) HideButton(isSetDefault);
            else
            {
                if (!_isFirstOpen)
                {
                    _isFirstOpen = true;
                    CurrentChoice = (MoveChoice) DataManager.Ins.GameData.setting.moveChoice;
                }
                OnChangeMoveChoice(CurrentChoice, isSetDefault);
            }
        }
    }
}
