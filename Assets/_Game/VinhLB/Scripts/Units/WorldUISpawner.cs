using _Game.DesignPattern;
using _Game.Managers;
using UnityEngine;

namespace VinhLB
{
    public class WorldUISpawner : HMonoBehaviour
    {
        [SerializeField]
        private Canvas _worldCanvas;
        [SerializeField]
        private Camera _worldCamera;
        [SerializeField]
        private Camera _uiCamera;

        public void SpawnFloatingUI(Transform objectTF)
        {
            FloatingUI floatingUI = SimplePool.Spawn<FloatingUI>(DataManager.Ins.GetWorldUI(PoolType.FloatingUI));

            SetupWorldUI(floatingUI.Tf, objectTF);
                
            floatingUI.Initialize(null, "x1");
        }

        private void SetupWorldUI(Transform worldUITF, Transform objectTF)
        {
            // worldUITF.SetParent(_worldCanvas.transform, false);
            //
            // Vector3 screenPosition = _worldCamera.WorldToScreenPoint(objectTF.position);
            // screenPosition.z = (_worldCanvas.transform.position - _uiCamera.transform.position).magnitude;
            // worldUITF.position = _uiCamera.ScreenToWorldPoint(screenPosition);

            worldUITF.position = objectTF.position;
        }
    }
}