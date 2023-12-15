using UnityEngine;
using UnityEngine.UI;

namespace VinhLB
{
    public class BuildingItem : HMonoBehaviour
    {
        [SerializeField]
        private Button _button;
        [SerializeField]
        private Image _mainImage;

        private int _buildingUnitDataId;
        
        public void OnInit(BuildingUnitData data)
        {
            _buildingUnitDataId = data.Id;
            
            _button.onClick.RemoveAllListeners();
            _button.onClick.AddListener(() =>
            {
                GridBuildingManager.Ins.ChangeCurrentObjectDataId(_buildingUnitDataId);
            });
            
            _mainImage.sprite = data.Sprite;
        }
    }
}