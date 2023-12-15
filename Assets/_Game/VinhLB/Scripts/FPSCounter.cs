using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VinhLB
{
    public class FPSCounter : HMonoBehaviour
    {
        [SerializeField]
        private float _updateInterval = 0.5f;

        private float _accum = 0.0f;
        private int _frames = 0;
        private float _timeLeft;
        private float _fps;

        private GUIStyle _textStyle;

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
                _fps = (_accum / _frames);
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