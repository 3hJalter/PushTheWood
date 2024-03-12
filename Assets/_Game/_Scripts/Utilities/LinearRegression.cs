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

        public LinearRegression(Point[] data)
        {
            CalculateRegression(data);
        }

        private void CalculateRegression(Point[] data)
        {
            int n = data.Length;
            float sumX = 0;
            float sumY = 0;
            float sumXY = 0;
            float sumX2 = 0;

            foreach (Point p in data)
            {
                sumX += p.X;
                sumY += p.Y;
                sumXY += p.X * p.Y;
                sumX2 += p.X * p.X;
            }

            float meanX = sumX / n;
            float meanY = sumY / n;

            slope = (sumXY - n * meanX * meanY) / (sumX2 - n * meanX * meanX);
            yIntercept = meanY - slope * meanX;
        }

        public float PredictY(float x)
        {
            return slope * x + yIntercept;
        }
    }
}