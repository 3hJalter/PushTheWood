using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace _Game.Utilities {
    public enum DevId
    {
        SYSTEM = -1,
        Hung,
        Hoang,
        Vinh,
        Kien,      
    }
    public static class DevLog
    {
        private static List<string> DevColors = new List<string>() { "000000", "#6fff59", "#fa4b4b", "000000", "000000"};
        public static void Log(DevId devId, string log)
        {
            Debug.Log($"<color={DevColors[(int)devId + 1]}>[{devId}] {log}</color>");
        }
    }
}