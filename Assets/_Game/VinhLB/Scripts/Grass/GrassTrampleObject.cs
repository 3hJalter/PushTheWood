using System;
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

        private GrassTrampleFeature _feature;

        private void Awake()
        {
            Utilities.TryGetRendererFeature<GrassTrampleFeature>(_rendererData, out _feature);
        }

        private void OnEnable()
        {
            if (_feature != null)
            {
                _feature.AddTrackedTrampleObject(this);
            }
        }
        
        private void OnDisable()
        {
            if (_feature != null)
            {
                _feature.RemoveTrackedTrampleObject(this);
            }
        }
    } 
}
