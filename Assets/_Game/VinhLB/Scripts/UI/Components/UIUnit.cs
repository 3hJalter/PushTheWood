using _Game.DesignPattern;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace VinhLB
{
    using UnityEngine.UI;
    public class UIUnit : GameUnit
    {
        [Title("References")]
        // [SerializeField]
        // private Animator _animator;
        [SerializeField]
        protected Image _icon;
        [SerializeField]
        protected TMP_Text _text;
        [SerializeField]
        protected RectTransform _rectTransform;

        public Image Icon => _icon;
        public TMP_Text Text => _text;
        public RectTransform RectTransform => _rectTransform;
        // private RectTransform _rectTF;
        //
        // public RectTransform RectTF
        // {
        //     get
        //     {
        //         if (_rectTF == null)
        //         {
        //             _rectTF = GetComponent<RectTransform>();
        //         }
        //
        //         return _rectTF;
        //     }
        // }
    }
}