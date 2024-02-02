using _Game.DesignPattern;
using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace VinhLB
{
    public class FloatingWorldUI : UIUnit
    {
        [Title("References")]
        // [SerializeField]
        // private Animator _animator;
        [SerializeField]
        private Image _icon;
        [SerializeField]
        private TMP_Text _text;

        public void Initialize(Sprite sprite, string text)
        {
            if (sprite != null)
            {
                _icon.sprite = sprite;
            }
            _text.text = text;

            DOTween.Kill(Tf);
            Tf.DOMoveY(4f, 1f).SetEase(Ease.OutBack).OnComplete(() => { this.Despawn(); });
        }
    }
}