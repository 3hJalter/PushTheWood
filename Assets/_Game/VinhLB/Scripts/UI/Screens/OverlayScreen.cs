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
        private RectTransform _uiRewardKeyParentRectTF;
        [SerializeField]
        private RectTransform _uiLevelStarParentRectTF;
        [SerializeField]
        private RectTransform _floatingRewardKeyParentRectTf;

        public RectTransform UICoinParentRectTF => _uiCoinParentRectTF;
        public RectTransform UIAdTicketParentRectTF => _uiAdTicketParentRectTF;
        public RectTransform UIRewardKeyParentRectTF => _uiRewardKeyParentRectTF;
        public RectTransform UILevelStarParentRectTF => _uiLevelStarParentRectTF;
        public RectTransform FloatingRewardKeyParentRectTF => _floatingRewardKeyParentRectTf;
    }
}