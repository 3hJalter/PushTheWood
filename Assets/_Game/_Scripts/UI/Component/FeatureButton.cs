using System;
using TMPro;
using UnityEngine;
using VinhLB;

namespace _Game._Scripts.UIs.Component
{
    [RequireComponent(typeof(HButton))]
    public class FeatureButton : MonoBehaviour, IClickable
    {
        public event Action OnClicked;
        
        [SerializeField]
        private HButton button;
        [SerializeField]
        private GameObject lockObj;
        [SerializeField]
        private GameObject unlockObj;
        [SerializeField]
        private TMP_Text unlockLevelText;
        
        private bool _isUnlocked;
        private int _unlockedLevelIndex = -1;

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
        
        public void SetUnlockLevelIndex(int levelIndex)
        {
            if (levelIndex < 0 && levelIndex == _unlockedLevelIndex)
            {
                return;
            }
            
            _unlockedLevelIndex = levelIndex;
            
            unlockLevelText.text = $"Lv.{_unlockedLevelIndex + 1}";
        }
    }
}