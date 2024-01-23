using UnityEngine;
using UnityEngine.EventSystems;

namespace HControls
{
    [RequireComponent(typeof(GameObject))]
    public class HDpadButton : HMonoBehaviour
    {
        [SerializeField] private Direction direction;
        [SerializeField] private GameObject pointerDownImg;
        [SerializeField] private EventTrigger eventTrigger;
        public Direction Direction => direction;

        public GameObject PointerDownImg => pointerDownImg;

        private void Awake()
        {
            pointerDownImg.SetActive(false);
        }
        
        public void ManualPointerDown()
        {
            eventTrigger.OnPointerDown(new PointerEventData(EventSystem.current));
        }
        
        public void ManualPointerUp()
        {
            eventTrigger.OnPointerUp(new PointerEventData(EventSystem.current));
        }
    }
}
