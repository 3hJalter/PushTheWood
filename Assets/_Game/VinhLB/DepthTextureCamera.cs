using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VinhLB
{
    public class DepthTextureCamera : MonoBehaviour
    {
        [SerializeField]
        private Camera _camera;
        [SerializeField]
        private DepthTextureMode _depthTextureMode;

        private void Awake()
        {
            UpdateDepthTextureMode();
        }

        private void OnValidate()
        {
            UpdateDepthTextureMode();
        }

        private void UpdateDepthTextureMode()
        {
            if (_camera == null)
            {
                return;
            }

            _camera.depthTextureMode = _depthTextureMode;
        }
    }
}
