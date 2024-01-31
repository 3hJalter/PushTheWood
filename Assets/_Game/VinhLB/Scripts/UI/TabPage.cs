using _Game.Utilities;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace VinhLB
{
    public abstract class TabPage : UICanvas
    {
        [SerializeField]
        private CanvasGroup _canvasGroup;
        [SerializeField]
        private Image _blockPanel;
        
        public override void Open(object param = null)
        {
            base.Open(param);
            
            _blockPanel.gameObject.SetActive(false);

            if (param is bool animated)
            {
                // DevLog.Log(DevId.Vinh, $"{GetType().Name}: {animated}");
                if (animated)
                {
                    _canvasGroup.alpha = 0f;
                    _blockPanel.gameObject.SetActive(true);
                    
                    DOVirtual.Float(0f, 1f, 1f, value => _canvasGroup.alpha = value)
                        .OnComplete(() => _blockPanel.gameObject.SetActive(false));
                }
            }
        }
    }
}