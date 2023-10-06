using UnityEngine;
using UnityEngine.EventSystems;

namespace CnControls
{
    public class Touchpad : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler
    {
        /// <summary>
        ///     The name of the horizontal axis for this touchpad to update
        /// </summary>
        public string HorizontalAxisName = "Horizontal";

        /// <summary>
        ///     The name of the vertical axis for this touchpad to update
        /// </summary>
        public string VerticalAxisName = "Vertical";

        /// <summary>
        ///     Whether this touchpad should preserve inertia when the finger is lifted
        /// </summary>
        public bool PreserveInertia = true;

        /// <summary>
        ///     The speed of decay of inertia
        /// </summary>
        public float Friction = 3f;

        /// <summary>
        ///     Joystick movement direction
        ///     Specifies the axis along which it can move
        /// </summary>
        [Tooltip("Constraints on the joystick movement axis")]
        public ControlMovementDirection ControlMoveAxis = ControlMovementDirection.Both;

        private VirtualAxis _horizintalAxis;
        private bool _isCurrentlyTweaking;
        private int _lastDragFrameNumber;
        private VirtualAxis _verticalAxis;

        /// <summary>
        ///     Current event camera reference. Needed for the sake of Unity Remote input
        /// </summary>
        public Camera CurrentEventCamera { get; set; }

        private void Update()
        {
            if (_isCurrentlyTweaking && _lastDragFrameNumber < Time.renderedFrameCount - 2)
            {
                _horizintalAxis.Value = 0f;
                _verticalAxis.Value = 0f;
            }

            if (PreserveInertia && !_isCurrentlyTweaking)
            {
                _horizintalAxis.Value = Mathf.Lerp(_horizintalAxis.Value, 0f, Friction * Time.deltaTime);
                _verticalAxis.Value = Mathf.Lerp(_verticalAxis.Value, 0f, Friction * Time.deltaTime);
            }
        }

        private void OnEnable()
        {
            // When we enable, we get our virtual axis

            _horizintalAxis = _horizintalAxis ?? new VirtualAxis(HorizontalAxisName);
            _verticalAxis = _verticalAxis ?? new VirtualAxis(VerticalAxisName);

            // And register them in our input system
            CnInputManager.RegisterVirtualAxis(_horizintalAxis);
            CnInputManager.RegisterVirtualAxis(_verticalAxis);
        }

        private void OnDisable()
        {
            // When we disable, we just unregister our axis
            // It also happens before the game object is Destroyed
            CnInputManager.UnregisterVirtualAxis(_horizintalAxis);
            CnInputManager.UnregisterVirtualAxis(_verticalAxis);
        }

        public virtual void OnDrag(PointerEventData eventData)
        {
            // Some bitwise logic for constraining the touchpad along one of the axis
            // If the "Both" option was selected, non of these two checks will yield "true"
            if ((ControlMoveAxis & ControlMovementDirection.Horizontal) != 0) _horizintalAxis.Value = eventData.delta.x;
            if ((ControlMoveAxis & ControlMovementDirection.Vertical) != 0) _verticalAxis.Value = eventData.delta.y;

            _lastDragFrameNumber = Time.renderedFrameCount;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _isCurrentlyTweaking = true;
            OnDrag(eventData);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            _isCurrentlyTweaking = false;
            if (!PreserveInertia)
            {
                _horizintalAxis.Value = 0f;
                _verticalAxis.Value = 0f;
            }
        }
    }
}