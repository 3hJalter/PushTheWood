using UnityEngine;

namespace VinhLB
{
    public class FPSCounter : HMonoBehaviour
    {
        [SerializeField] private float _updateInterval = 0.5f;

        private float _accum;
        private float _fps;
        private int _frames;

        private GUIStyle _textStyle;
        private float _timeLeft;

        private void Start()
        {
            _timeLeft = _updateInterval;

            _textStyle = new GUIStyle();
            _textStyle.fontStyle = FontStyle.Bold;
            _textStyle.normal.textColor = Color.white;
        }

        private void Update()
        {
            _timeLeft -= Time.unscaledDeltaTime;
            _accum += 1f / Time.unscaledDeltaTime;
            ++_frames;

            if (_timeLeft <= 0f)
            {
                _fps = _accum / _frames;
                _timeLeft = _updateInterval;
                _accum = 0f;
                _frames = 0;
            }
        }

        private void OnGUI()
        {
            GUI.Label(new Rect(5, 5, 100, 25), _fps.ToString("F2") + " FPS", _textStyle);
        }
    }
}
