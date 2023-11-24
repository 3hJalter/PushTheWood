using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VinhLB
{
    public class NormalsReplacementShaderCamera : MonoBehaviour
    {
        [SerializeField]
        private Camera _camera;
        [SerializeField]
        private Shader _normalsShader;

        private Camera _newCamera;
        private RenderTexture _renderTexture;

        private void Awake()
        {
            // Create a render texture matching the main camera's current dimensions.
            _renderTexture = new RenderTexture(_camera.pixelWidth, _camera.pixelHeight, 24);
            // Surface the render texture as a global variable, available to all shaders.
            Shader.SetGlobalTexture("_CameraNormalsTexture", _renderTexture);

            // Setup a copy of the camera to render the scene using the normals shader.
            GameObject go = new GameObject("NormalsCamera");
            _newCamera = go.AddComponent<Camera>();
            _newCamera.CopyFrom(_camera);
            _newCamera.transform.SetParent(transform);
            _newCamera.targetTexture = _renderTexture;
            _newCamera.SetReplacementShader(_normalsShader, "RenderType");
            _newCamera.depth = _camera.depth - 1;
        }
    }
}
