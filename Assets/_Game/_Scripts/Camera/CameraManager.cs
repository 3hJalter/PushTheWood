using System;
using System.Collections;
using System.Collections.Generic;
using _Game._Scripts.InGame;
using _Game.Camera;
using _Game.DesignPattern;
using _Game.GameGrid;
using Cinemachine;
using DG.Tweening;
using MEC;
using UnityEngine;

namespace _Game.Managers
{
    public class CameraManager : Singleton<CameraManager>
    {
        private const int CAMERA_PRIORITY_ACTIVE = 99;
        private const int CAMERA_PRIORITY_INACTIVE = 1;
        [SerializeField] private CinemachineVirtualCameraBase currentVirtualCamera;

        [SerializeField] private UnityEngine.Camera brainCamera;
        [SerializeField] private Transform cameraTarget;
        [SerializeField] private WorldMapTarget worldMapCameraTarget; // may be redundant later
        
        [SerializeField] private float cameraMoveTime = 1f;
        [SerializeField] private Vector2 worldCameraXYPos;

        public Vector2 WorldCameraXYPos => worldCameraXYPos;


        // ReSharper disable once Unity.RedundantSerializeFieldAttribute
        // ReSharper disable once CollectionNeverUpdated.Local
        [SerializeField]
        private readonly Dictionary<ECameraType, CinemachineVirtualCameraBase> virtualCameraDic = new();

        public UnityEngine.Camera BrainCamera => brainCamera;

        public CinemachineVirtualCameraBase CurrentVirtualCamera => currentVirtualCamera;

        public bool IsCurrentCameraIs(ECameraType eCameraType)
        {
            return currentVirtualCamera == virtualCameraDic[eCameraType];
        }

        public void ChangeCamera(ECameraType eCameraType)
        {
            if (currentVirtualCamera != null)
            {
                currentVirtualCamera.Priority = CAMERA_PRIORITY_INACTIVE;
                currentVirtualCamera.enabled = false;
            }
            currentVirtualCamera = virtualCameraDic[eCameraType];
            currentVirtualCamera.Priority = CAMERA_PRIORITY_ACTIVE;
            currentVirtualCamera.enabled = true;
        }

        public void ChangeCameraTargetPosition(Vector3 position, float moveTime = -1f)
        {
            if (moveTime < 0f) moveTime = cameraMoveTime;
            // Timing.KillCoroutines("MoveCameraTargetPosition");
            // Timing.RunCoroutine(MoveCameraTargetPosition(position, moveTime));
            cameraTarget.DOKill();
            cameraTarget.DOMove(position, moveTime).SetEase(Ease.Linear);

            // cameraTarget.position = position;
        }

        public void ChangeWorldTargetPosition()
        {
            worldMapCameraTarget.Tf.position = new Vector3(
                worldCameraXYPos.x,
                worldCameraXYPos.y,
                LevelManager.Ins.CurrentLevel.GetCenterPos().z);
        }
        
        public void ChangeCameraTarget(ECameraType eCameraType, Transform target)
        {
            virtualCameraDic[eCameraType].Follow = target;
            virtualCameraDic[eCameraType].LookAt = target;
        }
    }
}
