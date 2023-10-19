using System;
using System.Collections.Generic;
using UnityEngine;

namespace _Game.Utilities.Timer
{
    [DefaultExecutionOrder(-100)]
    public class TimerManager : MonoBehaviour
    {
        public enum LoopType
        {
            Update = 0,
            FixedUpdate = 1
        }

        private const int INIT_STIMER = 50;
        private readonly List<STimer> inUseSTimers = new();
        private readonly Queue<STimer> sTimers = new();
        public static TimerManager Inst { get; private set; }

        private void Awake()
        {
            if (Inst != null)
            {
                Destroy(gameObject);
                return;
            }

            Inst = this;
            DontDestroyOnLoad(gameObject);
            AddSTimerToPool();
        }

        private void Update()
        {
            TimerUpdate?.Invoke();
        }

        private void FixedUpdate()
        {
            TimerFixedUpdate?.Invoke();
        }

        private void LateUpdate()
        {
            TimerLateUpdate?.Invoke();
        }

        [HideInInspector] public event Action TimerUpdate;

        [HideInInspector] public event Action TimerFixedUpdate;

        [HideInInspector] public event Action TimerLateUpdate;

        public STimer PopSTimer()
        {
            AddSTimerToPool();
            STimer timer = sTimers.Dequeue();
            inUseSTimers.Add(timer);
            return timer;
        }

        public void PushSTimer(STimer timer, bool checkDuplicated = true) //DEV: Can using Heap to optimize
        {
            if (timer == null) return;
            if (checkDuplicated)
                if (sTimers.Contains(timer))
                    return;

            timer.IsUnscaleTime = false;
            sTimers.Enqueue(timer);
            inUseSTimers.Remove(timer);
        }

        public void WaitForFrame(int frame, Action action)
        {
            STimer timer = PopSTimer();
            Action timerAction = () =>
            {
                action?.Invoke();
                PushSTimer(timer, false);
            };
            timer.Start(frame, timerAction);
        }

        public void WaitForTime(float time, Action action, bool isUncaleTime = false)
        {
            STimer timer = PopSTimer();
            timer.IsUnscaleTime = isUncaleTime;
            Action timerAction = () =>
            {
                action?.Invoke();
                PushSTimer(timer, false);
            };
            timer.Start(time, timerAction);
        }

        public STimer WaitForTime(List<float> times, List<Action> events)
        {
            STimer timer = PopSTimer();
            timer.Start(times, events, () => PushSTimer(timer));
            return timer;
        }

        public void TriggerLoopAction(Action action, float time, int num = 0, bool isUnscaleTime = false)
        {
            if (action == null || num <= 0) return;
            STimer timer = PopSTimer();
            timer.IsUnscaleTime = isUnscaleTime;

            int i = 1;
            Action timerAction = () =>
            {
                if (i >= num) StopTimer(ref timer);
                action?.Invoke();
                i++;
            };

            timer.Start(time, timerAction, true);
        }

        public void StopTimer(ref STimer timer)
        {
            if (timer == null) return;

            timer.Stop();
            PushSTimer(timer);
            timer = null;
        }

        public void TriggerLoopAction(Action action, int frame, LoopType type) //NOTE: How it works???
        {
            STimer timer = PopSTimer();
            if (type == LoopType.Update)
            {
                timer.ClearEvent(STimer.EventType.FrameUpdate);
                timer.FrameUpdate += action;

                Action timerAction = () => PushSTimer(timer, false);
                timer.Start(frame, timerAction);
            }
            else if (type == LoopType.FixedUpdate)
            {
                timer.ClearEvent(STimer.EventType.FrameFixedUpdate);
                timer.FrameFixedUpdate += action;
                Action timerAction = () => { PushSTimer(timer, false); };
                timer.Start(frame, timerAction, STimer.EventType.FrameFixedUpdate);
            }
        }

        public void RecallAllSTimer()
        {
            for (int i = 0; i < inUseSTimers.Count; i++)
            {
                STimer timer = inUseSTimers[i];
                timer.Stop();
                sTimers.Enqueue(timer);
            }

            inUseSTimers.Clear();
        }

        private void AddSTimerToPool()
        {
            if (sTimers.Count == 0)
                for (int i = 0; i < INIT_STIMER; i++)
                {
                    STimer timer = new();
                    sTimers.Enqueue(timer);
                }
        }
    }
}
