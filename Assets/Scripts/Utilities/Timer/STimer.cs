using System;
using System.Collections.Generic;
using UnityEngine;

namespace Utilitys.Timer
{
    public class STimer
    {
        public enum EventType
        {
            FrameUpdate = 0,
            FrameFixedUpdate = 1
        }
        // Start is called before the first frame update

        private bool isStartFrame;

        public STimer()
        {
            TimerManager.Inst.TimerUpdate += Update;
            TimerManager.Inst.TimerFixedUpdate += FixedUpdate;
            TimerManager.Inst.TimerLateUpdate += LateUpdate;
        }

        public void Start(float time, int code = -1)
        {
            this.code = code;
            Start(time);
        }

        public void Start(int frame, int code = -1, EventType type = EventType.FrameUpdate)
        {
            this.code = code;
            Start(frame, type);
        }

        public void Start(int frame, Action action, EventType type = EventType.FrameUpdate)
        {
            CallBack = action;
            Start(frame, type);
        }

        public void Start(float time, Action action, bool isLoop = false)
        {
            this.isLoop = isLoop;
            loopTime = time;
            CallBack = action;
            Start(time);
        }

        public void Start(float time)
        {
            if (time > 0)
            {
                IsStart = true;
                TimeRemaining = time;
            }
            else
            {
                TriggerEvent();
                Stop();
            }

            isStartFrame = true;
        }

        public void Start(int frame, EventType type)
        {
            if (type == EventType.FrameUpdate)
            {
                if (frame > 0)
                {
                    IsStart = true;
                    timeFrame = frame;
                    FrameUpdate?.Invoke();
                }
                else
                {
                    TriggerEvent();
                    Stop();
                }
            }
            else if (type == EventType.FrameFixedUpdate)
            {
                if (frame > 0)
                {
                    IsStart = true;
                    timeFixedFrame = frame;
                    FrameFixedUpdate?.Invoke();
                }
                else
                {
                    TriggerEvent();
                    Stop();
                }
            }

            isStartFrame = true;
        }

        public void Start(List<float> times, List<Action> events, Action callBack = null)
        {
            //NOTE: Init time list and action list if it null
            if (this.times == null)
            {
                this.times = new List<float>();
                this.events = new List<Action>();
            }

            //NOTE:Clear and add new data
            this.times.Clear();
            this.events.Clear();
            for (int i = 0; i < times.Count; i++)
            {
                this.times.Add(times[i]);
                this.events.Add(events[i]);
            }

            //NOTE: Starting STimer
            IsStart = false;
            CallBack = callBack;
            maxTime = times[times.Count - 1];
            eventIndex = 0;

            //CASE: One time and one event or all events happens in 0
            if (maxTime > 0)
                Start(maxTime);
            else
                for (int i = 0; i < events.Count; i++)
                    events[i]?.Invoke();

        }

        public void Stop()
        {
            IsStart = false;
            events = null;
            times = null;
            TimeRemaining = 0;
            timeFrame = 0;
            eventIndex = 0;
            CallBack = null;
            isLoop = false;
        }

        private void Update()
        {
            if (IsStart) CounterUpdate();
        }

        private void FixedUpdate()
        {
            if (IsStart) CounterFixedUpdate();
        }

        private void LateUpdate()
        {
            isStartFrame = false;
        }

        private void CounterUpdate()
        {
            if (TimeRemaining > 0)
            {
                if (!IsUnscaleTime)
                    TimeRemaining -= Time.deltaTime;
                else
                    TimeRemaining -= Time.unscaledDeltaTime;
                if (times != null && events != null)
                {
                    int maxEventIndex = events.Count;
                    for (; eventIndex < maxEventIndex;)
                        if (maxTime - TimeRemaining >= times[eventIndex])
                        {
                            eventIndex += 1;
                            events[eventIndex - 1].Invoke();
                        }
                        else
                        {
                            break;
                        }

                }

                if (TimeRemaining <= 0)
                {
                    TriggerEvent();
                    if (!isLoop)
                        Stop();
                    else
                        TimeRemaining = loopTime;
                }
            }

            if (timeFrame > 0)
            {
                timeFrame -= 1;
                if (timeFrame <= 0)
                {
                    TriggerEvent();
                    if (!isLoop)
                        Stop();
                    else
                        timeFrame = loopFrame;
                }
            }

            if (!isStartFrame)
                FrameUpdate?.Invoke();
        }

        private void CounterFixedUpdate()
        {
            if (timeFixedFrame > 0)
            {
                timeFixedFrame -= 1;
                if (timeFixedFrame <= 0)
                {
                    TriggerEvent();
                    if (!isLoop)
                        Stop();
                    else
                        timeFixedFrame = loopFrame;
                }
            }

            if (!isStartFrame)
                FrameFixedUpdate?.Invoke();
        }

        private void TriggerEvent()
        {
            TimeOut?.Invoke(code);
            CallBack?.Invoke();
        }

        public void ClearEvent(EventType type)
        {
            switch (type)
            {
                case EventType.FrameUpdate:
                    FrameUpdate = null;
                    break;
                case EventType.FrameFixedUpdate:
                    FrameFixedUpdate = null;
                    break;
            }
        }

        ~STimer()
        {
            TimerManager.Inst.TimerUpdate -= Update;
            TimerManager.Inst.TimerFixedUpdate -= FixedUpdate;
            TimerManager.Inst.TimerLateUpdate -= LateUpdate;
        }

        #region Multiple Event Calls

        private List<Action> events;
        private List<float> times;
        private float maxTime;
        private int eventIndex;

        #endregion

        #region Event

        private event Action CallBack;
        public event Action FrameUpdate;
        public event Action FrameFixedUpdate;
        public event Action<int> TimeOut;

        #endregion

        #region Property

        private int timeFrame;
        private int timeFixedFrame;
        private int code = -1;

        private bool isLoop;
        private float loopTime;
        private readonly int loopFrame = 0;
        public float TimeRemaining { get; private set; }

        public bool IsStart { get; private set; }

        public bool IsUnscaleTime { get; set; } = false;

        #endregion
    }
}
