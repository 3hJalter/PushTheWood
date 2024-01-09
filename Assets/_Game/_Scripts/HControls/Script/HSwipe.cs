using System;
using GG.Infrastructure.Utils.Swipe;
using UnityEngine;

namespace HControls
{
    public class HSwipe : HMonoBehaviour
    {
        [SerializeField] private SwipeListener swipeListener;

        private void OnEnable()
        {
            swipeListener.OnSwipe.AddListener(OnSwipe);
        }

        private void OnSwipe(string direction)
        {
            Debug.Log(direction);
        }

        private void OnDisable()
        {
            swipeListener.OnSwipe.RemoveListener(OnSwipe);
        }
    }
}
