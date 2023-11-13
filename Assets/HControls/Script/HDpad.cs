using UnityEngine;

namespace HControls
{
    public class HDpad : HMonoBehaviour
    {
        [Tooltip("0 is Left, 1 is Right, 2 is Up, 3 is Down")]
        [SerializeField] private bool highlightButton = true;
        [SerializeField] private HDpadButton[] dpadButtons;
        
        public void OnButtonPointerDown(int index)
        {
            if (highlightButton) dpadButtons[index].PointerDownImg.SetActive(true);
            HInputManager.SetDirectionInput(dpadButtons[index].Direction);
        }

        public void OnButtonPointerUp(int index)
        {
            dpadButtons[index].PointerDownImg.SetActive(false);
            HInputManager.SetDirectionInput(Direction.None);
        }
    } 
}

