using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.Rendering.Universal;

namespace VinhLB
{
    public class BuildingGhost : HMonoBehaviour
    {
        //private const string GHOST_RENDER_FEATURE = "Ghost";
        private const string GHOST_LAYER_NAME = "Ghost";

        [Header("References")]
        [SerializeField]
        private UniversalRendererData _rendererData;
        [SerializeField]
        private Material _validMaterial;
        [SerializeField]
        private Material _invalidMaterial;

        [Header("Settings")]
        [SerializeField]
        private float _heightOffset = 1f;
        [SerializeField]
        private float _animationSpeed = 15f;

        private RenderObjects _ghostRenderObjects;
        private Transform _visual;
        private bool _isValid;

        private void Start()
        {
            Utilities.TryGetRendererFeature<RenderObjects>(_rendererData, out _ghostRenderObjects);

            RefreshVisual();

            GridBuildingManager.Ins.OnSelectedChanged += GridBuildingSystem_OnSelectedChanged;
        }

        private void LateUpdate()
        {
            UpdateGhostPositionAndRotation(GridBuildingManager.Ins.Snapping);
        }

        private void UpdateGhostPositionAndRotation(bool instant)
        {
            if (!GridBuildingManager.Ins.IsOnBuildMode || GridBuildingManager.Ins.CurrentPlacedObjectData == null)
            {
                return;
            }

            if (_isValid != GridBuildingManager.Ins.CanBuild())
            {
                _isValid = GridBuildingManager.Ins.CanBuild();

                UpdateGhostMaterial();
            }

            Vector3 targetPosition = GridBuildingManager.Ins.GetMouseWorldSnappedPosition();
            targetPosition.y = _heightOffset;
            Quaternion targetRotation = GridBuildingManager.Ins.GetPlacedObjectRotation();

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

            PlacedObjectData placedObjectData = GridBuildingManager.Ins.CurrentPlacedObjectData;
            if (placedObjectData != null)
            {
                _visual = Instantiate(placedObjectData.Visual, transform);
                _visual.localPosition = Vector3.zero;
                _visual.localRotation = Quaternion.identity;
                _visual.SetLayer(LayerMask.NameToLayer(GHOST_LAYER_NAME), true);

                UpdateGhostPositionAndRotation(true);
            }
        }

        private void UpdateGhostMaterial()
        {
            if (_isValid)
            {
                _ghostRenderObjects.settings.overrideMaterial = _validMaterial;
            }
            else
            {
                _ghostRenderObjects.settings.overrideMaterial = _invalidMaterial;
            }

            _rendererData.SetDirty();
        }

        private void GridBuildingSystem_OnSelectedChanged()
        {
            RefreshVisual();
        }
    }
}
