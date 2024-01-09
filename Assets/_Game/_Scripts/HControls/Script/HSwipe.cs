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
            switch (direction)
            {
                case DirectionId.ID_LEFT:
                    HInputManager.SetDirectionInput(Direction.Left);
                    break;
                case DirectionId.ID_RIGHT:
                    HInputManager.SetDirectionInput(Direction.Right);
                    break;
                case DirectionId.ID_UP:
                    HInputManager.SetDirectionInput(Direction.Forward);
                    break;
                case DirectionId.ID_DOWN:
                    HInputManager.SetDirectionInput(Direction.Back);
                    break;
                default:
                    HInputManager.SetDirectionInput(Direction.None);
                    break;
            }
        }

        private void OnDisable()
        {
            swipeListener.OnSwipe.RemoveListener(OnSwipe);
        }
    }
}
