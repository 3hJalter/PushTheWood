using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class BF_MowingManager : MonoBehaviour
{
    public int precisionValue = 10;

    [Range(0f, 1f)] public float marginError = 0.2f;

    [HideInInspector] public List<Vector3> markersPos = new();
    [HideInInspector] public int totalMarker;
    private bool debugShown;
    private Bounds grassBounds;
    private GameObject grassGO;
    private Vector4 noGrassCoordOffset = Vector4.zero;
    private Texture2D noGrassTex;
    private float useVP;

    private void OnDrawGizmos()
    {
        if (debugShown)
            foreach (Vector3 pos in markersPos)
            {
                Gizmos.color = new Color(1, 1, 0, 0.75F);
                Gizmos.DrawWireSphere(pos, 0.3f);
            }
    }

    public void CreateMowingMarker(bool isDebugShown, int precisionValue)
    {
        ClearMarkers();

        grassGO = gameObject;
        noGrassTex = (Texture2D)GetComponent<MeshRenderer>().sharedMaterial.GetTexture("_NoGrassTex");
        Texture2D newTex = null;
        if (noGrassTex != null)
        {
            newTex = duplicateTexture(noGrassTex);
            noGrassCoordOffset = GetComponent<MeshRenderer>().sharedMaterial.GetVector("_MainTex_ST");
        }

        useVP = GetComponent<MeshRenderer>().sharedMaterial.GetFloat("_UseVP");
        grassBounds = grassGO.GetComponent<MeshRenderer>().bounds;

        for (int i = 0; i < precisionValue; i++)
        for (int j = 0; j < precisionValue; j++)
        {
            float lerpXValue = i / (float)precisionValue;
            float lerpZValue = j / (float)precisionValue;
            if (isDebugShown) debugShown = true;

            Vector3 markerPosition = new(Mathf.Lerp(grassBounds.max.x, grassBounds.min.x, lerpZValue), 5,
                Mathf.Lerp(grassBounds.max.z, grassBounds.min.z, lerpXValue));
            int layerMask = 1 << 0;
            RaycastHit hit;
            if (Physics.Raycast(markerPosition, Vector3.down, out hit, 50, layerMask))
                if (hit.transform == transform)
                {
                    if (useVP == 0)
                    {
                        if (newTex != null)
                        {
                            if (newTex.GetPixel(
                                    Mathf.RoundToInt((hit.textureCoord.x + noGrassCoordOffset.z) * 2048f *
                                                     noGrassCoordOffset.x),
                                    Mathf.RoundToInt((hit.textureCoord.y + noGrassCoordOffset.w) * 2048f *
                                                     noGrassCoordOffset.y)).r >= 0.2f) markersPos.Add(hit.point);
                        }
                        else
                        {
                            markersPos.Add(hit.point);
                        }
                    }
                    else
                    {
                        Mesh grassMesh = GetComponent<MeshFilter>().sharedMesh;

                        if (grassMesh.colors.Length == 0)
                        {
                            markersPos.Add(hit.point);
                        }
                        else
                        {
                            int triIndex = hit.triangleIndex;
                            int vertIndex1 = grassMesh.triangles[triIndex * 3 + 0];
                            if (vertIndex1 < grassMesh.colors.Length)
                                if (grassMesh.colors[vertIndex1].g >= 0.2f)
                                    markersPos.Add(hit.point);
                        }
                    }
                }
        }

        totalMarker = markersPos.Count;
    }

    public void changeDebugState(bool isShown)
    {
        debugShown = isShown;
    }

    private void ClearMarkers()
    {
        markersPos.Clear();
        markersPos.TrimExcess();
    }


    private Texture2D duplicateTexture(Texture2D source)
    {
        RenderTexture renderTex = RenderTexture.GetTemporary(
            source.width,
            source.height,
            0,
            RenderTextureFormat.Default,
            RenderTextureReadWrite.Linear);

        Graphics.Blit(source, renderTex);
        RenderTexture previous = RenderTexture.active;
        RenderTexture.active = renderTex;
        Texture2D readableText = new(source.width, source.height);
        readableText.ReadPixels(new Rect(0, 0, renderTex.width, renderTex.height), 0, 0);
        readableText.Apply();
        RenderTexture.active = previous;
        RenderTexture.ReleaseTemporary(renderTex);
        return readableText;
    }
}
