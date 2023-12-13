using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
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
        [SerializeField]
        private Transform _visualHolderTransform;
        [SerializeField]
        private Transform _cursorIndicatorTransform;

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

            _isValid = true;

            Vector3 startPosition = Tf.position;
            startPosition.y = _heightOffset;
            Tf.position = startPosition;
            _visualHolderTransform.DOMoveY(_heightOffset + 0.25f, 0.5f).SetLoops(-1, LoopType.Yoyo);

            GridBuildingManager.Ins.OnBuildingModeChanged += GridBuildingManager_OnOnBuildingModeChanged;
            GridBuildingManager.Ins.OnSelectedChanged += GridBuildingManager_OnSelectedChanged;

            GridBuildingManager_OnOnBuildingModeChanged();
            GridBuildingManager_OnSelectedChanged();
        }

        private void GridBuildingManager_OnOnBuildingModeChanged()
        {
            _cursorIndicatorTransform.gameObject.SetActive(GridBuildingManager.Ins.IsOnBuildingMode);
        }

        private void LateUpdate()
        {
            UpdateGhostPositionAndRotation(GridBuildingManager.Ins.Snapping);
        }

        private void UpdateGhostPositionAndRotation(bool instant)
        {
            if (!GridBuildingManager.Ins.IsOnBuildingMode)
            {
                return;
            }

            if (GridBuildingManager.Ins.CurrentBuildingUnitData != null && 
                _isValid != GridBuildingManager.Ins.CanBuild())
            {
                _isValid = GridBuildingManager.Ins.CanBuild();

                UpdateGhostMaterial();
            }

            Vector3 targetPosition = GridBuildingManager.Ins.GetMouseWorldSnappedPosition();
            targetPosition += new Vector3(Constants.CELL_SIZE * 0.5f, 0f, Constants.CELL_SIZE * 0.5f);
            targetPosition.y = Tf.position.y;
            Quaternion targetRotation = Quaternion.identity;
            Vector3 visualTargetPosition = Vector3.zero;
            if (GridBuildingManager.Ins.CurrentBuildingUnitData != null)
            {
                targetRotation = GridBuildingManager.Ins.GetPlacedObjectRotation();
                
                visualTargetPosition = Quaternion.Inverse(targetRotation) * GridBuildingManager.Ins.GetRotationOffset();
                visualTargetPosition.y = _visualHolderTransform.localPosition.y;
            }

            if (instant)
            {
                Tf.position = targetPosition;
                Tf.rotation = targetRotation;
                _visualHolderTransform.localPosition = visualTargetPosition;
            }
            else
            {
                Tf.position = Vector3.Lerp(Tf.position, targetPosition, _animationSpeed * Time.deltaTime);
                Tf.rotation = Quaternion.Lerp(Tf.rotation, targetRotation, _animationSpeed * Time.deltaTime);
                _visualHolderTransform.localPosition = Vector3.Lerp(
                    _visualHolderTransform.localPosition, visualTargetPosition, _animationSpeed * Time.deltaTime);
            }
        }

        private void RefreshVisual()
        {
            if (_visual != null)
            {
                Destroy(_visual.gameObject);
                _visual = null;
            }

            BuildingUnitData buildingUnitData = GridBuildingManager.Ins.CurrentBuildingUnitData;
            if (buildingUnitData != null)
            {
                _visual = Instantiate(buildingUnitData.Visual, _visualHolderTransform);
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

        private void GridBuildingManager_OnSelectedChanged()
        {
            RefreshVisual();
        }
    }
}
