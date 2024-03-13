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
        private bool _isUnlocked;
        
        public void AddListener(UnityEngine.Events.UnityAction action)
        {
            button.onClick.AddListener(action);
        }
        
        public void RemoveListener(UnityEngine.Events.UnityAction action)
        {
            button.onClick.RemoveListener(action);
        }
        
        public bool IsUnlocked
        {
            get => _isUnlocked;
            set
            {
                _isUnlocked = value;
                if (_isUnlocked)
                {
                    button.interactable = true;
                    unlockObj.SetActive(true);
                    lockObj.SetActive(false);
                }
                else
                {
                    button.interactable = false;
                    unlockObj.SetActive(false);
                    lockObj.SetActive(true);
                }
            }
        }

        public event Action OnClickedCallback;
    }
}
