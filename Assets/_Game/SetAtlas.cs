using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class SetAtlas : MonoBehaviour
{
    // TODO: Add it to the Image GameObject
    [SerializeField] private Image image;
    [SerializeField] private SpriteAtlas spriteAtlas;
    
    [SerializeField] private string spriteName;
    
    void Start()
    {
        image.sprite = spriteAtlas.GetSprite(spriteName);
    }
}
