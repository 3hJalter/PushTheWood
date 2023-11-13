using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageDirectionChange : HMonoBehaviour
{
    [SerializeField] private Image image;

    [SerializeField] private readonly Dictionary<Direction, Sprite> spriteDic = new();

    public void ChangeSprite(Direction direction)
    {
        image.sprite = direction is Direction.None ? null : spriteDic[direction];
    }
}
