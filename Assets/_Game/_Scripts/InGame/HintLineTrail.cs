using System;
using System.Collections.Generic;
using _Game.Utilities;
using DG.Tweening;
using MEC;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Game._Scripts.InGame
{
    public class HintLineTrail : HMonoBehaviour
    {
        [SerializeField] private float moveSpeed = 7f;
        [SerializeField] private TrailRenderer trail;
        [SerializeField] private Material trailMaterial;
        [ReadOnly]
        [SerializeField] private Vector3 currentDestination;
        [ReadOnly]
        [SerializeField] private bool isMoving;
        [ReadOnly]
        [SerializeField] private List<Vector3> path = new();
        private int _pathDestinationIndex;
        
        // _EmissiveIntensity id
        private static readonly int EmissiveIntensity = Shader.PropertyToID("_EmissiveIntensity");
        private const float EMISSIVE_INTENSITY = 2f;
        private float _initialEmissiveIntensity;
        
        private void Start()
        {
            _initialEmissiveIntensity = EMISSIVE_INTENSITY;
        }

        private void Update()
        {
            if (!isMoving || path.Count <= 0) return;
            // Move to destination
            transform.position = Vector3.MoveTowards(transform.position, currentDestination, moveSpeed * Time.deltaTime);
            // if reach to destination, set new destination
            if (!(Vector3.Distance(Tf.position, currentDestination) < 0.01f)) return;
            if (_pathDestinationIndex < path.Count - 1)
            {
                _pathDestinationIndex++;
                currentDestination = path[_pathDestinationIndex];
            }
            else
            {
                isMoving = false;
            }
        }
        
        public void SetPath(List<Vector3> pathIn)
        {
            if (pathIn.Count == 0) return;
            path = pathIn;
        }
        
        public void OnPlay()
        {
            if (path.Count == 0) return;
            // Reset the trail
            trail.Clear();
            fadeOut?.Kill();
            trailMaterial.SetFloat(EmissiveIntensity, _initialEmissiveIntensity);
            Timing.RunCoroutine(WaitOneFrameToSetEmitting(true));
            isMoving = true;
            Tf.position = path[0];
            _pathDestinationIndex = 1;
            currentDestination = path[_pathDestinationIndex];
        }
        
        public void OnCancel()
        {
            isMoving = false;
            trail.emitting = false;
            fadeOut = DOVirtual.Float(EMISSIVE_INTENSITY, 0, 1f, value =>
                trailMaterial.SetFloat(EmissiveIntensity, value)).SetEase(Ease.OutQuart);
        }
        
        // Wait one frame, then set the trail to emitting = true 
        private IEnumerator<float> WaitOneFrameToSetEmitting(bool isEmitting)
        {
            yield return Timing.WaitForOneFrame;
            trail.emitting = isEmitting;
        }

        private Tween fadeOut;
    }
}
