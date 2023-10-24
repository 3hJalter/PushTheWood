using System.Collections.Generic;
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
        [SerializeField] private readonly Dictionary<CameraType, CinemachineVirtualCameraBase> virtualCameraDic = new();
        [SerializeField] private CinemachineVirtualCameraBase currentVirtualCamera;

        public bool IsCurrentCameraIs(CameraType cameraType)
        {
            return currentVirtualCamera == virtualCameraDic[cameraType];
        }

        public void ChangeCamera(CameraType cameraType)
        {
            if (currentVirtualCamera != null) currentVirtualCamera.Priority = CAMERA_PRIORITY_INACTIVE;
            currentVirtualCamera = virtualCameraDic[cameraType];
            currentVirtualCamera.Priority = CAMERA_PRIORITY_ACTIVE;
        }

        public void ChangeCameraTarget(CameraType cameraType, Transform target)
        {
            virtualCameraDic[cameraType].Follow = target;
            virtualCameraDic[cameraType].LookAt = target;
        }
    }

    public enum CameraType
    {
        MainMenuCamera = 0,
        InGameCamera = 1,
        WorldMapCamera = 2
    }
}
