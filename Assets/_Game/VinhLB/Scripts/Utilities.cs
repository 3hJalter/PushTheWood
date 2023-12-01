using _Game.GameGrid;
using _Game.Utilities.Grid;
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

        public static bool TryGetMouseWorldPosition(out Vector3 mousePosition, 
            Camera camera = null, float distance = Mathf.Infinity, LayerMask layerMask = default)
        {
            mousePosition = Vector3.zero;
            camera = camera != null ? camera : Camera.main;
            layerMask = layerMask != default ? layerMask : Physics.DefaultRaycastLayers;
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, distance, layerMask))
            {
                mousePosition = hit.point;

                return true;
            }

            return false;
        }
    }
}