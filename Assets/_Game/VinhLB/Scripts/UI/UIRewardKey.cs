using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VinhLB
{
    public class UIRewardKey : UIUnit
    {
        [SerializeField]
        CanvasGroup _canvasGroup;
        public CanvasGroup CanvasGroup => _canvasGroup;
    }
}