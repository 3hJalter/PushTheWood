using System;
using UnityEngine;
using VinhLB;

namespace _Game._Scripts.UIs.Component
{
    [RequireComponent(typeof(HButton))]
    public class FeatureButton : MonoBehaviour, IClickable
    {
        [SerializeField] private HButton button;
        [SerializeField] private GameObject lockObj;
        [SerializeField] private GameObject unlockObj;
        private bool _isLock;
        
        public void AddListener(UnityEngine.Events.UnityAction action)
        {
            button.onClick.AddListener(action);
        }
        
        public void RemoveListener(UnityEngine.Events.UnityAction action)
        {
            button.onClick.RemoveListener(action);
        }
        
        public bool IsLock
        {
            get => _isLock;
            set
            {
                _isLock = value;
                if (_isLock)
                {
                    unlockObj.SetActive(true);
                    lockObj.SetActive(false);
                }
                else
                {
                    unlockObj.SetActive(false);
                    lockObj.SetActive(true);
                }
            }
        }

        public event Action OnClickedCallback;
    }
}
