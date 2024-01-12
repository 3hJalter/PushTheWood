using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace VinhLB
{
    public class Fish : HMonoBehaviour
    {
        private const float MIN_DISTANCE = 0.1f;

        [SerializeField]
        private float _heightOffset = 0f;
        [SerializeField]
        private float _moveSpeed = 4.0f;
        [SerializeField]
        private float _rotateSpeed = 180.0f;

        private List<Vector3> _pointList;
        private Tween _movementTween;
        
        public void Initialize(List<Vector3> pointList)
        {
            _pointList = pointList;
            _pointList.ForEach(point => point.y += _heightOffset);
            
            Move();
        }

        public void ResetMovement()
        {
            Move();
        }

        private void Move()
        {
            Tf.position = _pointList[0];
            
            if (_movementTween != null)
            {
                _movementTween.Kill();
                _movementTween = null;
            }
            
            _movementTween = Tf.DOPath(_pointList.ToArray(), 30f, PathType.CatmullRom, PathMode.Full3D, 5)
                .SetOptions(true)
                .SetLookAt(0.01f)
                .SetEase(Ease.Linear)
                .SetLoops(-1, LoopType.Restart);
        }
    }
}