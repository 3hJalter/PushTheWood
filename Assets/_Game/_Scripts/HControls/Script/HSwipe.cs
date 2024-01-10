﻿using System;
using _Game.Camera;
using _Game.Managers;
using _Game.Utilities;
using GG.Infrastructure.Utils.Swipe;
using UnityEngine;

namespace HControls
{
    public class HSwipe : HMonoBehaviour
    {
        [SerializeField] private SwipeListener swipeListener;

        private void OnEnable()
        {
            swipeListener.onSwipe.AddListener(OnSwipe);
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
                case Constants.NONE:
                    HInputManager.SetDirectionInput(Direction.None);
                    // TODO: Zoom In function when swipe out
                    CameraManager.Ins.ChangeCamera(ECameraType.ZoomOutCamera);
                    break;
            }
        }

        private void OnDisable()
        {
            swipeListener.onSwipe.RemoveListener(OnSwipe);
            // if (CameraManager.Ins.IsCurrentCameraIs(ECameraType.ZoomOutCamera))
            //     CameraManager.Ins.ChangeCamera(ECameraType.InGameCamera);
        }
    }
}
