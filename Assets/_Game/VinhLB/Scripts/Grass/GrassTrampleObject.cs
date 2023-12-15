using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace VinhLB
{
	public class GrassTrampleObject : HMonoBehaviour
	{
		[SerializeField]
		private UniversalRendererData _rendererData;
        [SerializeField]
        private float _trampleRadius = 1f;

        public float TrampleRadius => _trampleRadius;

        private void OnEnable()
        {
            if (Utilities.TryGetRendererFeature<GrassTrampleFeature>(_rendererData, out GrassTrampleFeature feature))
            {
                feature.AddTrackedTrampleObject(this);
            }
        }
        
        private void OnDisable()
        {
            if (Utilities.TryGetRendererFeature<GrassTrampleFeature>(_rendererData, out GrassTrampleFeature feature))
            {
                feature.RemoveTrackedTrampleObject(this);
            }
        }
    } 
}
