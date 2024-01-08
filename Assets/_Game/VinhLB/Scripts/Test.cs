using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    [SerializeField]
    private MeshRenderer _meshRenderer;
    [SerializeField]
    private float alpha;

    private void OnValidate()
    {
        _meshRenderer.sharedMaterial.SetColor("_BaseColor", new Color(1, 1, 1, alpha));
    }
}
