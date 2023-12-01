using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VinhLB
{
    public class BuildingGhost : HMonoBehaviour
    {
        [SerializeField]
        private float _offset = 1f;
        [SerializeField]
        private float _animationSpeed = 15f;

        private Transform _visual;

        private void Start()
        {
            RefreshVisual();

            GridBuildingSystem.Instance.OnSelectedChanged += GridBuildingSystem_OnSelectedChanged;
        }

        private void LateUpdate()
        {
            UpdateGhostPositionAndRotation(GridBuildingSystem.Instance.Snapping);
        }

        private void UpdateGhostPositionAndRotation(bool instant)
        {
            if (!GridBuildingSystem.Instance.IsOnBuildMode)
            {
                return;
            }

            Vector3 targetPosition = GridBuildingSystem.Instance.GetMouseWorldSnappedPosition();
            targetPosition.y = _offset;
            Quaternion targetRotation = GridBuildingSystem.Instance.GetPlacedObjectRotation();

            if (instant)
            {
                transform.position = targetPosition;
                transform.rotation = targetRotation;
            }
            else
            {
                transform.position = Vector3.Lerp(transform.position, targetPosition, _animationSpeed * Time.deltaTime);
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, _animationSpeed * Time.deltaTime);
            }
        }

        private void RefreshVisual()
        {
            if (_visual != null)
            {
                Destroy(_visual.gameObject);
                _visual = null;
            }

            PlacedObjectData placedObjectData = GridBuildingSystem.Instance.CurrentPlacedObjectData;
            if (placedObjectData != null)
            {
                _visual = Instantiate(placedObjectData.Visual, transform);
                _visual.localPosition = Vector3.zero;
                _visual.localRotation = Quaternion.identity;
                //_visual.SetLayer(11, true);

                UpdateGhostPositionAndRotation(true);
            }
        }

        private void GridBuildingSystem_OnSelectedChanged()
        {
            RefreshVisual();
        }
    }
}
