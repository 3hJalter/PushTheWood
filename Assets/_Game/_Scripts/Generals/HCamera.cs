using UnityEngine;
using UnityEngine.Serialization;

public class HCamera : HMonoBehaviour
{
    [SerializeField] private Camera initCamera;

    public Camera InitCamera => initCamera;

    public Vector3 Position => initCamera.transform.position;
    public Quaternion Rotation => initCamera.transform.rotation;
}
