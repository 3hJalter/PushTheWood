using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace _Game.Utilities {
    public enum DevId
    {
        Hung,
        Hoang,
        Vinh,
        Kien
    }
    public static class DevLog
    {
        private static List<string> DevColors = new List<string>() { "#6fff59", "000000", "000000", "000000"};
        public static void Log(DevId devId, string log)
        {
            Debug.Log($"<color={DevColors[(int)devId]}>[{devId}] {log}</color>");
        }
    }
}