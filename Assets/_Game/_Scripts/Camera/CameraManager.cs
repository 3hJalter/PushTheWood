using System.Collections.Generic;
using _Game.Camera;
using _Game.DesignPattern;
using Cinemachine;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Game.Managers
{
    public class CameraManager : Singleton<CameraManager>
    {
        private const int CAMERA_PRIORITY_ACTIVE = 99;
        private const int CAMERA_PRIORITY_INACTIVE = 1;

        // ReSharper disable once Unity.RedundantSerializeFieldAttribute
        // ReSharper disable once CollectionNeverUpdated.Local
        [SerializeField] private readonly Dictionary<ECameraType, CinemachineVirtualCameraBase> virtualCameraDic = new();
        [SerializeField] private CinemachineVirtualCameraBase currentVirtualCamera;

        [SerializeField] private UnityEngine.Camera brainCamera;

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

        public void ChangeCameraTarget(ECameraType eCameraType, Transform target)
        {
            virtualCameraDic[eCameraType].Follow = target;
            virtualCameraDic[eCameraType].LookAt = target;
        }   
    }

    
}
