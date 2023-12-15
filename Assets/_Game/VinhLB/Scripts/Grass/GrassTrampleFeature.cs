using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace VinhLB
{
    public class GrassTrampleFeature : ScriptableRendererFeature
    {
        private class CustomRenderPass : ScriptableRenderPass
        {
            private Vector4[] _tramplePositionArray;
            private float[] _trampleRadiusArray;
            private int _numTramplePositions;

            public int NumTramplePositions
            {
                get => _numTramplePositions;
                set => _numTramplePositions = value;
            }

            public CustomRenderPass(Vector4[] tramplePositionArray, float[] trampleRadiusArray)
            {
                _tramplePositionArray = tramplePositionArray;
                _trampleRadiusArray = trampleRadiusArray;
            }

            public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
            {
                CommandBuffer buffer = CommandBufferPool.Get("GrassTrampleFeature");
                buffer.SetGlobalVectorArray("_GrassTramplePositions", _tramplePositionArray);
                buffer.SetGlobalFloatArray("_GrassTrampleRadius", _trampleRadiusArray);
                buffer.SetGlobalInt("_NumGrassTramplePositions", _numTramplePositions);

                context.ExecuteCommandBuffer(buffer);
                CommandBufferPool.Release(buffer);
            }
        }

        [SerializeField]
        private int _maxTrackedTransform = 8;

        private List<GrassTrampleObject> _trackingTrampleObjectList;
        private Vector4[] _tramplePositionArray;
        private float[] _trampleRadiusArray;
        private CustomRenderPass _pass;

        public override void Create()
        {
            _trackingTrampleObjectList = new List<GrassTrampleObject>();
            _trackingTrampleObjectList.AddRange(FindObjectsOfType<GrassTrampleObject>());

            _tramplePositionArray = new Vector4[_maxTrackedTransform];
            _trampleRadiusArray = new float[_maxTrackedTransform];

            _pass = new CustomRenderPass(_tramplePositionArray, _trampleRadiusArray);
            _pass.renderPassEvent = RenderPassEvent.BeforeRendering;
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
#if UNITY_EDITOR
            _trackingTrampleObjectList.RemoveAll((tf) => tf == null);
#endif

            for (int i = 0; i < _tramplePositionArray.Length; i++)
            {
                _tramplePositionArray[i] = Vector4.zero;
                _trampleRadiusArray[i] = 0f;
            }

            int count = Mathf.Min(_trackingTrampleObjectList.Count, _tramplePositionArray.Length);
            for (int i = 0; i < count; i++)
            {
                Vector3 pos = _trackingTrampleObjectList[i].Tf.position;
                _tramplePositionArray[i] = new Vector4(pos.x, pos.y, pos.z, 1);
                _trampleRadiusArray[i] = _trackingTrampleObjectList[i].TrampleRadius;
            }
            _pass.NumTramplePositions = count;

            renderer.EnqueuePass(_pass);
        }

        public void AddTrackedTrampleObject(GrassTrampleObject obj)
        {
            _trackingTrampleObjectList.Add(obj);
        }
        
        public void RemoveTrackedTrampleObject(GrassTrampleObject obj)
        {
            _trackingTrampleObjectList.Remove(obj);
        }
    }
}
