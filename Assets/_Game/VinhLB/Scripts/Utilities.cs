using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace VinhLB
{
    public static class Utilities
    {
        public static TextMeshPro CreateWorldText(string text, Transform parent = null, Vector3 localPosition = default, Vector2 size = default,
            float fontSize = 32f, Color color = default, TextAlignmentOptions alignmentOptions = TextAlignmentOptions.TopLeft, int sortingOrder = 1000)
        {
            GameObject gameObject = new("WorldText");
            gameObject.transform.SetParent(parent, false);
            gameObject.transform.localPosition = localPosition;
            TextMeshPro textMesh = gameObject.AddComponent<TextMeshPro>();
            textMesh.rectTransform.sizeDelta = size;
            textMesh.alignment = alignmentOptions;
            textMesh.text = text;
            textMesh.fontSize = fontSize;
            textMesh.color = color;
            textMesh.GetComponent<MeshRenderer>().sortingOrder = sortingOrder;

            return textMesh;
        }
    }
}