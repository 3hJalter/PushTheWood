using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace VinhLB
{
    public static class Extensions
    {
        #region Transform
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

        public static void ClearChildren(this Transform transform)
        {
            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                Object.Destroy(transform.GetChild(i).gameObject);
            }
        }
        #endregion

        #region Rect Transform
        public static void SetPaddingLeft(this RectTransform rectTransform, float left)
        {
            rectTransform.offsetMin = new Vector2(left, rectTransform.offsetMin.y);
        }

        public static void SetPaddingRight(this RectTransform rectTransform, float right)
        {
            rectTransform.offsetMax = new Vector2(-right, rectTransform.offsetMax.y);
        }

        public static void SetPaddingTop(this RectTransform rectTransform, float top)
        {
            rectTransform.offsetMax = new Vector2(rectTransform.offsetMax.x, -top);
        }

        public static void SetPaddingBottom(this RectTransform rectTransform, float bottom)
        {
            rectTransform.offsetMin = new Vector2(rectTransform.offsetMin.x, bottom);
        }
        
        public static void SetPadding(this RectTransform rectTransform, float horizontal, float vertical) {
            rectTransform.offsetMax = new Vector2(-horizontal, -vertical);
            rectTransform.offsetMin = new Vector2(horizontal, vertical);
        }

        public static void SetPadding(this RectTransform rectTransform, float left, float top, float right, float bottom)
        {
            rectTransform.offsetMax = new Vector2(-right, -top);
            rectTransform.offsetMin = new Vector2(left, bottom);
        }
        #endregion
        
        #region List
        public static void Shift<T>(this List<T> list, int amount)
        {
            List<T> tempList = list.ToList();
            for (int i = 0; i < list.Count; i++)
            {
                int newValueIndex = (i + amount) % list.Count;
                list[i] = tempList[newValueIndex];
            }
        }
        #endregion
    }
}