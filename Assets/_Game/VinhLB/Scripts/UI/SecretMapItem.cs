using UnityEngine;
using UnityEngine.UI;

namespace VinhLB
{
    public class SecretMapItem : HMonoBehaviour
    {
        [SerializeField]
        private Button _playButton;
        [SerializeField]
        private GameObject _statusGO;

        public void SetButtons(bool isUnlocked)
        {
            if (isUnlocked)
            {
                _playButton.gameObject.SetActive(true);
                _statusGO.SetActive(false);
            }
            else
            {
                _playButton.gameObject.SetActive(false);
                _statusGO.SetActive(true);
            }
        }
    }
}