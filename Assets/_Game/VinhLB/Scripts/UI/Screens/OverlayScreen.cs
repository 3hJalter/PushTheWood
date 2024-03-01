using UnityEngine;

namespace VinhLB
{
    public class OverlayScreen : UICanvas
    {
        [SerializeField]
        private RectTransform _uiCoinParentRectTF;
        [SerializeField]
        private RectTransform _uiAdTicketParentRectTF;
        [SerializeField]
        private RectTransform _uiRewardKeyRectTF;

        public RectTransform UICoinParentRectTF => _uiCoinParentRectTF;
        public RectTransform UIAdTicketParentRectTF => _uiAdTicketParentRectTF;
        public RectTransform UIRewardKeyRectTF => _uiRewardKeyRectTF;
    }
}