using UnityEngine;

public class DirectionIcon : HMonoBehaviour
{
    [SerializeField] private Transform icon;

    public void OnChangeIcon(Direction direction)
    {
        icon.gameObject.SetActive(direction is not Direction.None);
        if (!icon.gameObject.activeSelf) return;
        icon.localRotation = direction switch
        {
            Direction.Forward => Quaternion.Euler(40, 0, 90),
            Direction.Back => Quaternion.Euler(40, 0, -90),
            Direction.Left => Quaternion.Euler(40, 0, 180),
            Direction.Right => Quaternion.Euler(40, 0, 0),
            _ => icon.localRotation
        };
    }
}
    