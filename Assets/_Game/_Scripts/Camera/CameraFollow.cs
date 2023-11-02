using System.Collections.Generic;
using _Game.DesignPattern;
using UnityEngine;

namespace _Game.Camera
{
    public class CameraFollow : Singleton<CameraFollow>
    {
        [SerializeField] private UnityEngine.Camera mainCamera;
        [SerializeField] private ECameraType currentECameraType = ECameraType.None;
        [SerializeField] private Transform targetTf;

        // ReSharper disable once Unity.RedundantSerializeFieldAttribute
        // ReSharper disable once CollectionNeverUpdated.Local
        [SerializeField] private readonly Dictionary<ECameraType, CameraFollower> _cameraDic = new();

        private CameraFollower _currentCamera;
        private Transform _mainCameraTf;

        private void Awake()
        {
            _mainCameraTf = mainCamera.transform;
            ChangeCamera(ECameraType.MainMenuCamera);
        }

        // Testing
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.C))
            {
                // go next camera in cameraDictionary, if last one, go to first one
                ECameraType eCameraType = currentECameraType + 1;
                if (eCameraType > ECameraType.WorldMapCamera) eCameraType = ECameraType.MainMenuCamera;
                ChangeCamera(eCameraType);
            }
        }

        private void FixedUpdate()
        {
            if (targetTf is null || !_currentCamera.isFollowTarget)
            {
                _mainCameraTf.position = Vector3.Lerp(_mainCameraTf.position,_currentCamera.offsetPosition, 
                    _currentCamera.smooth);
            }
            else
            {
                _mainCameraTf.position = Vector3.Lerp(_mainCameraTf.position,
                    targetTf.position + _currentCamera.offsetPosition, _currentCamera.smooth);
            }
            _mainCameraTf.rotation = Quaternion.Lerp(_mainCameraTf.rotation, _currentCamera.offsetRotation,
                _currentCamera.smooth);
        }

        // Change to public if need
        public void SetTarget(Transform target)
        {
            targetTf = target;
        }
        
        public void ChangeCamera(ECameraType eCameraType, Transform target = null)
        {
            currentECameraType = eCameraType;
            _currentCamera = _cameraDic[eCameraType];
            if (target is not null) SetTarget(target);
        }
        
        public bool IsCurrentCameraIs(ECameraType eCameraType)
         {
             return _currentCamera == _cameraDic[eCameraType];
         }
    }

    // ReSharper disable once ClassNeverInstantiated.Global
    public class CameraFollower
    {
        public readonly bool isFollowTarget;
        [Range(0,1)]
        public readonly float smooth; // From 0 to 1
        public Vector3 offsetPosition;
        public Quaternion offsetRotation;

        public CameraFollower(Vector3 offsetPosition, Quaternion offsetRotation, float smooth, bool isFollowTarget)
        {
            this.offsetPosition = offsetPosition;
            this.offsetRotation = offsetRotation;
            this.smooth = smooth;
            this.isFollowTarget = isFollowTarget;
        }
    }
}
