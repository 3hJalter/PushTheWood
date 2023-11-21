using _Game.Managers;
using _Game.UIs.Screen;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace _Game._Scripts.UIs.Tutorial
{
    public class TutorialScreen : UICanvas
    {
        [SerializeField] private TutorialContext context;
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private Image blockPanel;

        public override void Setup()
        {
            if (UIManager.Ins.IsOpened<InGameScreen>()) UIManager.Ins.CloseUI<InGameScreen>();
            ChangeBlockPanelActiveStatus(true);
            base.Setup();
        }

        public override void Open()
        {
            base.Open();
            DOVirtual.Float(0, 1, 0.5f, value => canvasGroup.alpha = value)
                .OnComplete(() => ChangeBlockPanelActiveStatus(false));
        }

        public override void Close()
        {
            base.Close();
            RemoveContext();
            UIManager.Ins.OpenUI<InGameScreen>();
        }

        private void RemoveContext()
        {
            Destroy(context.gameObject);
            context = null;
        }
        
        public void LoadContext(TutorialContext contextIn)
        {
            context = contextIn;
            context.Tf.SetParent(MRectTransform, false);
            context.TutorialScreen = this;
            // set top left right bottom to zero
            context.Rect.offsetMin = Vector2.zero;
            context.Rect.offsetMax = Vector2.zero;
                
        }
        
        private void ChangeBlockPanelActiveStatus(bool status)
        {
            blockPanel.enabled = status;
        }
    }
}
