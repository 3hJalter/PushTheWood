using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace VinhLB
{
    public class MaskScreen : UICanvas
    {
        [SerializeField]
        private UIMask _uiMaskPrefab;
        [SerializeField]
        private Transform _maskParentTF;
        [SerializeField]
        private Transform _simpleMaskableTF;
        [SerializeField]
        private MaskDatabase _maskDatabase;

        private List<UIMask> _createdUIMaskList = new List<UIMask>();
        private Stack<UIMask> _availableUIMaskList = new Stack<UIMask>();
        
        public override void Open(object param = null)
        {
            base.Open(param);

            SetupMask(param);
        }

        public override void Close()
        {
            base.Close();
            
            _availableUIMaskList.Clear();
            for (int i = 0; i < _createdUIMaskList.Count; i++)
            {
                _createdUIMaskList[i].Close();
                _availableUIMaskList.Push(_createdUIMaskList[i]);
            }
        }

        public void SetupMask(object param)
        {
            if (param == null)
            {
                _maskParentTF.gameObject.SetActive(false);
                _simpleMaskableTF.gameObject.SetActive(true);
                
                return;
            }
            
            _simpleMaskableTF.gameObject.SetActive(false);
            _maskParentTF.gameObject.SetActive(true);
            
            if (param is MaskData)
            {
                MaskData data = param as MaskData;
                
                InitializeUIMask(data);
            }
            else if (param is MaskData[])
            {
                MaskData[] data = param as MaskData[];

                for (int i = 0; i < data.Length; i++)
                {
                    InitializeUIMask(data[i]);
                }
            }
        }

        private void InitializeUIMask(MaskData data)
        {
            UIMask mask;
            if (_availableUIMaskList.Count > 0)
            {
                mask = _availableUIMaskList.Pop();
                mask.Open();
            }
            else
            {
                mask = Instantiate(_uiMaskPrefab, _maskParentTF);
                _createdUIMaskList.Add(mask);
            }
            
            mask.Initialize(data.Position, data.Size, _maskDatabase.MaskSpriteDict[data.MaskType], data.Button, Close);
        }
    }

    public class MaskData
    {
        public Vector3 Position;
        public Vector2 Size;
        public MaskType MaskType;
        public Button Button;
    }

    public enum MaskType
    {
        None = -1,
        Rectangle = 0,
        Eclipse = 1
    }
}