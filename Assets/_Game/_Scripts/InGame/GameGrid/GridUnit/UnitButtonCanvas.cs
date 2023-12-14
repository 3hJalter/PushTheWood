using _Game.Camera;
using UnityEngine;

namespace _Game.GameGrid.Unit
{
    public class UnitButtonCanvas : HMonoBehaviour
    {
        [SerializeField] private Vector3 offset;
        [SerializeField] private HButton button;
        
        public void OnShow(GridUnit gridUnit)
        {
            gameObject.SetActive(true);
            // Convert unit position to screen position
            Vector3 screenPos = CameraFollow.Ins.MainCamera.WorldToScreenPoint(gridUnit.transform.position + offset);
            // Set the button position to screen position
            button.transform.position = screenPos;
        }
        
        public void OnHide()
        {
            gameObject.SetActive(false);
        }
    }
}
