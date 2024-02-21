using UnityEngine;
using UnityEngine.UI;

namespace VinhLB
{
    public class SecretMapItem : HMonoBehaviour
    {
        [SerializeField]
        private GameObject _statusGO;
        [SerializeField]
        private GameObject _lockGO;
        [SerializeField]
        private GameObject _unlockProgressGO;
        [SerializeField]
        private Button _playButton;

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
    }
}