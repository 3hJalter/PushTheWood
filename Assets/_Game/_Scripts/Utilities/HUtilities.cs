﻿using _Game.Utilities;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace _Game._Scripts.Utilities
{
    public static class HUtilities
    {
        private static LinearRegression linearRegression = new LinearRegression();

        public static Vector3 Change(this Vector3 org, float? x = null, float? y = null, float? z = null)
        {
            return new Vector3(x ?? org.x, y ?? org.y, z ?? org.z);
        }

        public static Vector3Int Change(this Vector3Int org, int? x = null, int? y = null, int? z = null)
        {
            return new Vector3Int(x ?? org.x, y ?? org.y, z ?? org.z);
        }
        public static bool PercentRandom(float rate)
        {
            rate = Mathf.Clamp01(rate);
            float value = UnityEngine.Random.Range(0f, 1f);
            if (value < rate) return true;
            else return false;
        }
        public static bool PercentRandom(float rate, Action action)
        {
            if (PercentRandom(rate))
            {
                action?.Invoke();
                return true;
            }
            return false;
        }
        public static int WheelRandom(List<float> rates)
        {
            float totalRate = 0f;
            for (int i = 0; i < rates.Count; i++)
            {
                if (rates[i] < 0) rates[i] = 0;
                totalRate += rates[i];
            }

            float value = UnityEngine.Random.Range(0f, 1f) * totalRate;
            float currentAnchor = 0;
            for (int i = 0; i < rates.Count; i++)
            {
                if (currentAnchor <= value && value < rates[i] + currentAnchor)
                {
                    return i;
                }
                currentAnchor += rates[i];
            }
            return 0;
        }

        public static int WheelRandom(float[] rates)
        {
            float totalRate = 0f;
            for (int i = 0; i < rates.Length; i++)
            {
                if (rates[i] < 0) rates[i] = 0;
                totalRate += rates[i];
            }

            float value = UnityEngine.Random.Range(0f, 1f) * totalRate;
            float currentAnchor = 0;
            for (int i = 0; i < rates.Length; i++)
            {
                if (currentAnchor <= value && value < rates[i] + currentAnchor)
                {
                    return i;
                }
                currentAnchor += rates[i];
            }
            return 0;

        }
        public static float PredictYFromLinearRegression(List<Vector2> data, float x)
        {
            linearRegression.CalculateRegression(data);
            return linearRegression.PredictY(x);
        }
    }
}
