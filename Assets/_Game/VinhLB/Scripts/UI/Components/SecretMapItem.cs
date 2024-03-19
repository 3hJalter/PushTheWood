using System;
using UnityEngine;
using UnityEngine.UI;

namespace VinhLB
{
    public class SecretMapItem : HMonoBehaviour
    {
        public event Action<int> OnPlayClick;
        
        [SerializeField]
        private int index;
        [SerializeField]
        private GameObject _statusGO;
        [SerializeField]
        private GameObject _lockGO;
        [SerializeField]
        private GameObject _unlockProgressGO;
        [SerializeField]
        private Button _playButton;
        [SerializeField]
        private GameObject _completeGO;

        private void Awake()
        {
            _playButton.onClick.AddListener(OnPlayButtonClick);
        }
        
        public void SetButtons(bool isUnlocked)
        {
            if (!isUnlocked)
            {
                _statusGO.SetActive(true);
                _lockGO.SetActive(true);
                _unlockProgressGO.SetActive(false);
                _playButton.gameObject.SetActive(false);
            }
            else
            {
                _statusGO.SetActive(false);
                _lockGO.SetActive(false);
                _unlockProgressGO.SetActive(true);
                _playButton.gameObject.SetActive(true);
            }
        }

        private void OnPlayButtonClick()
        {
            OnPlayClick?.Invoke(index);
        }

        private void OnDestroy()
        {
            _playButton.onClick.RemoveAllListeners();
        }
    }
}