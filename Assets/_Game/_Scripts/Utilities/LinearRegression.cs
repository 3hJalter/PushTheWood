using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

namespace _Game.Utilities
{
    public class LinearRegression
    {
        private float slope;
        private float yIntercept;

        public void CalculateRegression(List<Vector2> data)
        {
            int n = data.Count;
            float sumX = 0;
            float sumY = 0;
            float sumXY = 0;
            float sumX2 = 0;

            foreach (Vector2 p in data)
            {
                sumX += p.x;
                sumY += p.y;
                sumXY += p.x * p.y;
                sumX2 += p.x * p.x;
            }

            float meanX = sumX / n;
            float meanY = sumY / n;

            float xMul = sumX2 - n * meanX * meanX;
            if (xMul != 0)
                slope = (sumXY - n * meanX * meanY) / xMul;
            else
                slope = 0;
            yIntercept = meanY - slope * meanX;
        }

        public float PredictY(float x)
        {
            return slope * x + yIntercept;
        }
    }
}