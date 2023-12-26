using System.Collections.Generic;
using _Game.Camera;
using _Game.DesignPattern;
using Cinemachine;
using DG.Tweening;
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

        [SerializeField] private float cameraMoveTime = 1f;

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
            if (currentVirtualCamera != null) currentVirtualCamera.Priority = CAMERA_PRIORITY_INACTIVE;
            currentVirtualCamera = virtualCameraDic[eCameraType];
            currentVirtualCamera.Priority = CAMERA_PRIORITY_ACTIVE;
        }

        public void ChangeCameraTargetPosition(Vector3 position, float moveTime = -1f)
        {
            if (moveTime < 0f) moveTime = cameraMoveTime;
            cameraTarget.DOKill();
            cameraTarget.DOMove(position, moveTime).SetEase(Ease.OutCubic);

            // cameraTarget.position = position;
        }

        public void ChangeCameraTarget(ECameraType eCameraType, Transform target)
        {
            virtualCameraDic[eCameraType].Follow = target;
            virtualCameraDic[eCameraType].LookAt = target;
        }
    }
}
