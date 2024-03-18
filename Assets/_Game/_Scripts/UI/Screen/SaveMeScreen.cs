using System.Collections.Generic;
using _Game._Scripts.InGame;
using _Game.DesignPattern;
using _Game.Managers;
using HControls;
using MEC;
using UnityEngine;

namespace _Game.UIs.Screen
{
    public class SaveMeScreen : UICanvas
    {
        [SerializeField] private HButton btnMoreTimeTicket;
        [SerializeField] private HButton btnClose;
        
        private const float TIMER = 3f;

        private float _progress; // DO THIS FOR FILL AMOUNT
        private void Awake()
        {
            btnClose.onClick.AddListener(OnGoLose);
            btnMoreTimeTicket.onClick.AddListener(OnMoreTimeTicket);
        }

        private void OnDestroy()
        {
            btnClose.onClick.RemoveAllListeners();
            btnMoreTimeTicket.onClick.RemoveAllListeners();
        }

        public override void Setup(object param = null)
        {
            base.Setup(param);
            GameManager.Ins.ChangeState(GameState.Pause);
            HInputManager.LockInput();
            btnClose.gameObject.SetActive(false);
            Timing.RunCoroutine(CountDown());
        }

        public override void Close()
        {
            HInputManager.LockInput(false);
            base.Close();
        }

        private void OnGoLose()
        {
            GameplayManager.Ins.OnLoseGame(LevelLoseCondition.Timeout);
            Close();
        }

        private void OnMoreTimeTicket()
        {
            AdsManager.Ins.RewardedAds.Show(() =>
            {
                GameManager.Ins.PostEvent(EventID.MoreTimeGame);
                Close();
            });
        }
        
        IEnumerator<float> CountDown()
        {
            _progress = 0;
            float time = TIMER;
            while (time > 0)
            {
                _progress = time / TIMER;
                time -= Time.deltaTime;
                yield return Timing.WaitForOneFrame;
            }
            btnClose.gameObject.SetActive(true);
        }
    }
}
