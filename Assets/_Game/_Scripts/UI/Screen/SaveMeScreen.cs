using _Game._Scripts.InGame;
using _Game.DesignPattern;
using _Game.Managers;
using _Game.Utilities.Timer;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace _Game.UIs.Screen
{
    public class SaveMeScreen : UICanvas
    {
        [SerializeField] private HButton btnMoreTime;
        [SerializeField] private TextMeshProUGUI costText;
        [SerializeField] private HButton btnMoreTimeTicket;
        [SerializeField] private HButton btnClose;
        
        private const float TIMER = 3f;

        private int _costAmount;
        private void Awake()
        {
            btnMoreTime.onClick.AddListener(OnMoreTime);
            btnClose.onClick.AddListener(OnGoLose);
            btnMoreTimeTicket.onClick.AddListener(OnMoreTimeTicket);
        }

        private void OnDestroy()
        {
            btnMoreTime.onClick.RemoveAllListeners();
            btnClose.onClick.RemoveAllListeners();
            btnMoreTimeTicket.onClick.RemoveAllListeners();
        }

        public override void Setup(object param = null)
        {
            base.Setup(param);
            GameManager.Ins.ChangeState(GameState.Pause);
            // cast param to int
            if (param is int cost)
            {
                // if cost large than current gold, set lock to true
                _costAmount = cost;
                btnMoreTime.gameObject.SetActive(cost <= DataManager.Ins.GoldCount);
                costText.text = cost.ToString();
            }
            else
            {
                btnMoreTime.gameObject.SetActive(false);
            }
            
            btnClose.gameObject.SetActive(false);
            
            TimerManager.Ins.WaitForTime(TIMER, () =>
            {
                btnClose.gameObject.SetActive(true);
            });
        }

        private void OnGoLose()
        {
            GameplayManager.Ins.OnLoseGame(LevelLoseCondition.Timeout);
            Close();
        }

        private void OnMoreTime()
        {
            GameManager.Ins.TrySpendGold(_costAmount);
            GameManager.Ins.PostEvent(EventID.MoreTimeGame);
            Close();
        }

        private void OnMoreTimeTicket()
        {
            // TODO: Show ads, then callback this below
            GameManager.Ins.PostEvent(EventID.MoreTimeGame);
            Close();
        }
    }
}
