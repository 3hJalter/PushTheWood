using System;
using UnityEngine;

namespace _Game._Scripts.Utilities
{
    public static class HUtilities
    {
        public static Direction InverseDirection(Direction direction)
        {
            return direction switch
            {
                Direction.Left => Direction.Right,
                Direction.Right => Direction.Left,
                Direction.Forward => Direction.Back,
                Direction.Back => Direction.Forward,
                _ => Direction.None
            };
        }
        
        public static void DoAfterFrames(ref int frames, Action action, Action onWait = null)
        {
            if (frames > 0)
            {
                frames--;
                onWait?.Invoke();
            }
            else
            {
                action();
            }
        }
        
        public static void DoAfterSeconds(ref float time, Action action, Action onWait = null)
        {
            if (time > 0)
            {
                time -= Time.deltaTime;
                onWait?.Invoke();
            }
            else
            {
                action();
                time = 0;
            }
        }
    }
}
