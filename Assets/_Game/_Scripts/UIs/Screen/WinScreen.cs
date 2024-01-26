using _Game.GameGrid;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.UIs.Screen
{
    public class WinScreen : UICanvas
    {
        [SerializeField]
        private CanvasGroup _canvasGroup;
        [SerializeField]
        private Image _blockPanel;

        public override void Setup()
        {
            base.Setup();
            
            _canvasGroup.alpha = 0f;
        }

        public override void Open()
        {
            base.Open();
            
            DOVirtual.Float(0f, 1f, 0.25f, value => _canvasGroup.alpha = value)
                .OnComplete(() => _blockPanel.gameObject.SetActive(false));
        }
        
        public void OnClickNextButton()
        {
            LevelManager.Ins.OnNextLevel();
            Close();
        }
    }
}
