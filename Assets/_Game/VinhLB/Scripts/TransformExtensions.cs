using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VinhLB
{
    public static class TransformExtensions
    {
        public static void SetLayer(this Transform transform, int layer, bool recursive = false)
        {
            transform.gameObject.layer = layer;

            if (recursive)
            {
                for (int i = transform.childCount - 1; i >= 0; i--)
                {
                    transform.GetChild(i).SetLayer(layer, true);
                }
            }
        }

        public static Transform ClearChildren(this Transform transform)
        {
            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                GameObject.Destroy(transform.GetChild(i).gameObject);
            }

            return transform;
        }
    } 
}
