// Copyright (c) <2015> <Playdead>
// This file is subject to the MIT License as seen in the root of this folder structure (LICENSE.TXT)
// AUTHOR: Lasse Jon Fuglsang Pedersen <lasse@playdead.com>

using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Playdead/VelocityBufferTag")]
public class VelocityBufferTag : MonoBehaviour
{
    private const int framesNotRenderedSleepThreshold = 60;
#if UNITY_5_6_OR_NEWER
    private static readonly List<Vector3> temporaryVertexStorage = new(512);
#endif
    public static List<VelocityBufferTag> activeObjects = new(128);

    private Transform _transform;
    private int framesNotRendered = framesNotRenderedSleepThreshold;
    [NonSerialized] [HideInInspector] public Matrix4x4 localToWorldCurr;
    [NonSerialized] [HideInInspector] public Matrix4x4 localToWorldPrev;
    [NonSerialized] [HideInInspector] public Mesh mesh;

    [NonSerialized] [HideInInspector] public SkinnedMeshRenderer meshSmr;
    [NonSerialized] [HideInInspector] public bool meshSmrActive;
    public bool rendering => framesNotRendered < framesNotRenderedSleepThreshold;

    private void Awake()
    {
        Reset();
    }

    private void Reset()
    {
        _transform = transform;

        SkinnedMeshRenderer smr = GetComponent<SkinnedMeshRenderer>();
        if (smr != null)
        {
            if (mesh == null || meshSmrActive == false)
            {
                mesh = new Mesh();
                mesh.hideFlags = HideFlags.HideAndDontSave;
            }

            meshSmrActive = true;
            meshSmr = smr;
        }
        else
        {
            MeshFilter mf = GetComponent<MeshFilter>();
            if (mf != null)
                mesh = mf.sharedMesh;
            else
                mesh = null;

            meshSmrActive = false;
            meshSmr = null;
        }

        // force restart
        framesNotRendered = framesNotRenderedSleepThreshold;
    }

    private void LateUpdate()
    {
        if (framesNotRendered < framesNotRenderedSleepThreshold)
        {
            framesNotRendered++;
            TagUpdate(false);
        }
    }

    private void OnEnable()
    {
        activeObjects.Add(this);
    }

    private void OnDisable()
    {
        activeObjects.Remove(this);

        // force restart
        framesNotRendered = framesNotRenderedSleepThreshold;
    }

    private void OnWillRenderObject()
    {
        if (Camera.current != Camera.main)
            return; // ignore anything but main cam

        if (framesNotRendered >= framesNotRenderedSleepThreshold)
            TagUpdate(true);

        framesNotRendered = 0;
    }

    private void TagUpdate(bool restart)
    {
        if (meshSmrActive && meshSmr == null) Reset();

        if (meshSmrActive)
        {
            if (restart)
            {
                meshSmr.BakeMesh(mesh);
#if UNITY_5_6_OR_NEWER
                mesh.GetVertices(temporaryVertexStorage);
                mesh.SetNormals(temporaryVertexStorage);
#else
                mesh.normals = mesh.vertices;// garbage ahoy
#endif
            }
            else
            {
#if UNITY_5_6_OR_NEWER
                mesh.GetVertices(temporaryVertexStorage);
                meshSmr.BakeMesh(mesh);
                mesh.SetNormals(temporaryVertexStorage);
#else
                Vector3[] vs = mesh.vertices;// garbage ahoy
                meshSmr.BakeMesh(mesh);
                mesh.normals = vs;
#endif
            }
        }

        if (restart)
        {
            localToWorldCurr = _transform.localToWorldMatrix;
            localToWorldPrev = localToWorldCurr;
        }
        else
        {
            localToWorldPrev = localToWorldCurr;
            localToWorldCurr = _transform.localToWorldMatrix;
        }
    }
}
