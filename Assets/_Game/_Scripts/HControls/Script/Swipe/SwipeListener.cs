using System;
using System.Collections.Generic;
using _Game._Scripts.Utilities;
using _Game.Utilities;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace GG.Infrastructure.Utils.Swipe
{
    [Serializable]
    public class SwipeListenerEvent : UnityEvent<string>
    {
    }

    public class SwipeListener : MonoBehaviour
    {
        public UnityEvent onSwipeCancelled;
        public SwipeListenerEvent onSwipe;
        public UnityEvent onCancelSwipe;
        public UnityEvent onUnHold;
        public bool isOverlappingUI;
        
        
        [SerializeField] private float sensitivity = 100;
        [SerializeField] private float quickSensitive = 50;
        
        [SerializeField] private float swipeTouchTime = 0.12f;
        [SerializeField] private float quickSwipeTouchTime = 0.0035f;

        [SerializeField] private bool continuousDetection;

        [SerializeField] private SwipeDetectionMode swipeDetectionMode = SwipeDetectionMode.EightSides;

        private VectorToDirection _directions;

        private float _minMoveDistance = 0.1f;
        private float _quickMinMoveDistance = 0.1f;

        private Vector3 _offset;

        private Vector3 _swipeStartPoint;

        private bool _waitForSwipe = true;
        private bool _countingSwipeTime = false;
        
        private float _holdTimer = Constants.HOLD_TOUCH_TIME;
        private float _swipeTimer;

        private bool _isHolding;
        [ReadOnly] [SerializeField] private List<Vector2> samplePoints;
        [ReadOnly] [SerializeField] private List<Vector2> quickSamplePoints;
        public bool ContinuousDetection
        {
            get => continuousDetection;
            set => continuousDetection = value;
        }

        public float QuickSensitive
        {
            get => quickSensitive;
            set
            {
                quickSensitive = value;
                UpdateQuickSensitivity();
            }
        }

        public float Sensitivity
        {
            get => sensitivity;
            set
            {
                sensitivity = value;
                UpdateSensitivity();
            }
        }

        public SwipeDetectionMode SwipeDetectionMode
        {
            get => swipeDetectionMode;
            set => swipeDetectionMode = value;
        }

        private void Start()
        {
            samplePoints = new List<Vector2>();
            _swipeTimer = swipeTouchTime;
            UpdateSensitivity();
            UpdateQuickSensitivity();

            if (SwipeDetectionMode != SwipeDetectionMode.Custom)
                SetDetectionMode(DirectionPresets.GetPresetByMode(SwipeDetectionMode));
        }
        
        private void Update()
        {
            // Version 2
            if (Input.GetMouseButtonUp(0))
            {
                // Check if button Release
                if (_isHolding)
                {
                    onUnHold?.Invoke();
                    _isHolding = false;
                }
                if (!_waitForSwipe) OnCancelSwipe();
            } else if (Input.GetMouseButtonDown(0))
            {
                // Check if button Pressed
                if (!isOverlappingUI && UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject()) return;
                InitSwipe();
            } else if (Input.GetMouseButton(0))
            {
                // Check if button is being held
                if (_waitForSwipe) CheckSwipe();
            }
            // Check if Swipe is continuous detection
            if (!continuousDetection) CheckSwipeCancellation();
            if (_countingSwipeTime)
            {
                if (_swipeTimer <= Constants.HOLD_TOUCH_TIME) _swipeTimer += Time.deltaTime;
            }
        }

        public void SetDetectionMode(List<DirectionId> directions)
        {
            _directions = new VectorToDirection(directions);
        }

        private void UpdateSensitivity()
        {
            int screenShortSide = Screen.width < Screen.height ? Screen.width : Screen.height;
            _minMoveDistance = screenShortSide / sensitivity;
        }
        
        private void UpdateQuickSensitivity()
        {
            int screenShortSide = Screen.width < Screen.height ? Screen.width : Screen.height;
            _quickMinMoveDistance = screenShortSide / quickSensitive;
        }

        private void CheckSwipeCancellation()
        {
            if (Input.GetMouseButtonUp(0))
                if (_waitForSwipe)
                    if (onSwipeCancelled != null)
                        onSwipeCancelled.Invoke();
        }

        private void InitSwipe()
        {
            SampleSwipeStart();
            _waitForSwipe = true;
        }

        private bool isPredicted = false;
        
        private void CheckSwipe()
        {
            if (isPredicted) return;

            _offset = Input.mousePosition - _swipeStartPoint;
            
            if (_swipeTimer < quickSwipeTouchTime)
            {
                // TODO
                if (quickSamplePoints.Count < 1)
                {
                    quickSamplePoints.Add(Vector2.zero);
                    _countingSwipeTime = true;
                }
                
                Vector2 quickDistance = _offset - (Vector3)quickSamplePoints[^1];
                if (quickDistance.magnitude >= _quickMinMoveDistance)
                {
                    quickSamplePoints.Add(_offset);
                    if (quickSamplePoints.Count >= 2 && !isPredicted)
                    {
                        isPredicted = true;
                        float y = HUtilities.PredictYFromLinearRegression(quickSamplePoints, _offset.x);
                        _offset.Set(_offset.x, y, 0);
                        DevLog.Log(DevId.Hoang, "BEFORE NORMALIZE: " + _offset + ", SAMPLE COUNT: " + quickSamplePoints.Count);
                        _offset.Normalize();
                        DevLog.Log(DevId.Hoang, $"DIRECTION: {_offset}");
                        // normalize the _offset
                        onSwipe?.Invoke(_directions.GetSwipeId(_offset));
                        if (!continuousDetection) _waitForSwipe = false;
                        // SampleSwipeStart();
                        _isHolding = false;
                        _countingSwipeTime = false;
                        return;
                    }
                }
                
            }
            
            if (samplePoints.Count < 1)
            {
                samplePoints.Add(Vector2.zero);
                _countingSwipeTime = true;
            }
            
            //NOTE: Calculate distance from old sample point to new sample point.
            Vector2 distance = _offset - (Vector3)samplePoints[^1];
            
            
            if (distance.magnitude >= _minMoveDistance)
            {
                samplePoints.Add(_offset);
                if ((samplePoints.Count >= 4 || _swipeTimer > swipeTouchTime) && !isPredicted)
                {
                    isPredicted = true;
                    float y = HUtilities.PredictYFromLinearRegression(samplePoints, _offset.x);
                    _offset.Set(_offset.x, y, 0);
                    _offset.Normalize();
                    DevLog.Log(DevId.Hung, $"DIRECTION: {_offset}");
                    onSwipe?.Invoke(_directions.GetSwipeId(_offset));
                    if (!continuousDetection) _waitForSwipe = false;
                    // SampleSwipeStart();
                    _isHolding = false;
                    _countingSwipeTime = false;
                }                
            }
            else if (_holdTimer > 0) // Add Hold Gesture for this code
            {
                _holdTimer -= Time.deltaTime;
                if (_holdTimer > 0) return;
                _holdTimer = Constants.HOLD_TOUCH_TIME;
                onSwipe?.Invoke(Constants.NONE);
                if (!continuousDetection) _waitForSwipe = false;
                // SampleSwipeStart();
                _isHolding = true;
            }
        }
        
        private void OnCancelSwipe()
        {
            _waitForSwipe = false;
            _holdTimer = Constants.HOLD_TOUCH_TIME;
            onCancelSwipe?.Invoke();
        }

        private void SampleSwipeStart()
        {
            isPredicted = false;
            samplePoints.Clear();
            quickSamplePoints.Clear();
            _swipeStartPoint = Input.mousePosition;
            _swipeTimer = 0;
            _offset = Vector3.zero;
        }
    }
}
