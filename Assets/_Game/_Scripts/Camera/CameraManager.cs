using System.Collections.Generic;
using _Game.Camera;
using _Game.DesignPattern;
using Cinemachine;
using DG.Tweening;
using UnityEngine;

namespace _Game.Managers
{
    [DefaultExecutionOrder(-100)]
    public class CameraManager : Singleton<CameraManager>
    {
        private const int CAMERA_PRIORITY_ACTIVE = 99;
        private const int CAMERA_PRIORITY_INACTIVE = 1;
        [SerializeField] private CinemachineBrain brain;
        [SerializeField] private CinemachineVirtualCameraBase currentVirtualCamera;

        [SerializeField] private UnityEngine.Camera brainCamera;
        [SerializeField] private UnityEngine.Camera overlayCamera;
        [SerializeField] private Transform cameraTarget;

        [SerializeField] private float cameraMoveTime = 1f;
        
        // ReSharper disable once Unity.RedundantSerializeFieldAttribute
        // ReSharper disable once CollectionNeverUpdated.Local
        [SerializeField]
        private readonly Dictionary<ECameraType, CinemachineVirtualCameraBase> virtualCameraDic = new();

        public UnityEngine.Camera BrainCamera => brainCamera;
        public UnityEngine.Camera OverlayCamera => overlayCamera;

        public CinemachineVirtualCameraBase CurrentVirtualCamera => currentVirtualCamera;

        public bool IsCurrentCameraIs(ECameraType eCameraType)
        {
            return currentVirtualCamera == virtualCameraDic[eCameraType];
        }

        public void ChangeCamera(ECameraType eCameraType, float blendTime = 2f)
        {
            if (currentVirtualCamera != null)
            {
                currentVirtualCamera.Priority = CAMERA_PRIORITY_INACTIVE;
                currentVirtualCamera.enabled = false;
            }
            currentVirtualCamera = virtualCameraDic[eCameraType];
            currentVirtualCamera.Priority = CAMERA_PRIORITY_ACTIVE;
            currentVirtualCamera.enabled = true;
            brain.m_DefaultBlend.m_Time = blendTime;
        }

        public void ChangeCameraTargetPosition(Vector3 position, float moveTime = -1f)
        {
            // if the same position, do nothing
            if ((cameraTarget.position - position).sqrMagnitude < 0.01f) return;
            
            if (moveTime < 0f) moveTime = cameraMoveTime;
            cameraTarget.DOKill();
            cameraTarget.DOMove(position, moveTime).SetEase(Ease.Linear);
        }
        
        public void ChangeCameraTarget(ECameraType eCameraType, Transform target)
        {
            virtualCameraDic[eCameraType].Follow = target;
            virtualCameraDic[eCameraType].LookAt = target;
        }

        public Vector3 WorldToViewportPoint(Vector3 position)
        {
            return brainCamera.WorldToViewportPoint(position);
        }

    }
}
