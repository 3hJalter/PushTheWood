using System;
using System.Collections;
using System.Collections.Generic;
using CnControls;
using DG.Tweening;
using MEC;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

[RequireComponent(typeof(EmptyGraphic))]
public class FloatingMoveInputButton : HMonoBehaviour, IPointerUpHandler, IPointerDownHandler
{
    public EmptyGraphic emptyGraphic;
    public float timeToHideButton = 1f;
    [SerializeField] private RectTransform btnContainer;
    
    private void Awake()
    {
        emptyGraphic = GetComponent<EmptyGraphic>();
        HideButton();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // throw new System.NotImplementedException();
        emptyGraphic.enabled = false;
        // Kill old hiddenTime coroutine
        DOTween.Kill(this);
        // Show btnContainer at the click position
        btnContainer.position = eventData.position;
        ShowButton();
    }
    
    public void OnPointerUp(PointerEventData eventData)
    {
        // Start the hiddenTime coroutine
        DOVirtual.DelayedCall(timeToHideButton, () =>
        {
            HideButton();
            emptyGraphic.enabled = true;
        }).SetId(this);
        // throw new System.NotImplementedException();
    }

    private IEnumerator<float> HideButtonAfterDelay()
    {
        yield return Timing.WaitForSeconds(timeToHideButton);
        HideButton();
        emptyGraphic.enabled = true;
    }

    private void ShowButton()
    {
        
        btnContainer.gameObject.SetActive(true);
    }

    public void HideButton()
    {
        btnContainer.gameObject.SetActive(false);
    }
}
