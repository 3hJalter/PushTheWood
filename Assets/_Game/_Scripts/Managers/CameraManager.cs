using System.Collections.Generic;
using _Game._Scripts.DesignPattern;
using Cinemachine;
using UnityEngine;

namespace _Game._Scripts.Managers
{
    public class CameraManager : Singleton<CameraManager>
    {
        private const int CAMERA_PRIORITY_ACTIVE = 99;
        private const int CAMERA_PRIORITY_INACTIVE = 1;
        private CinemachineVirtualCameraBase _currentVirtualCamera;
        // ReSharper disable once Unity.RedundantSerializeFieldAttribute
        // ReSharper disable once CollectionNeverUpdated.Local
        [SerializeField] private readonly Dictionary<CameraType, CinemachineVirtualCameraBase> virtualCameraDic = new();

        public bool IsCurrentCameraIs(CameraType cameraType)
        {
            return  _currentVirtualCamera == virtualCameraDic[cameraType];
        }
        
        public void ChangeCamera(CameraType cameraType)
        {
            if (_currentVirtualCamera != null)
            {
                _currentVirtualCamera.Priority = CAMERA_PRIORITY_INACTIVE;
            }
            _currentVirtualCamera = virtualCameraDic[cameraType];
            _currentVirtualCamera.Priority = CAMERA_PRIORITY_ACTIVE;
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
        WorldMapCamera = 2,
    }
}



