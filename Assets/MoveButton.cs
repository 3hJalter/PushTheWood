using System.Collections;
using System.Collections.Generic;
using _Game.GameGrid;
using DG.Tweening;
using HControls;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class MoveButton : HMonoBehaviour
{
    [SerializeField] private Direction direction;
    [SerializeField] private DpadFloating dpadFloating;
    [SerializeField] private GameObject pointerDownImg;
    public void OnButtonPointerDown()
    {
        pointerDownImg.SetActive(true);
        // LevelManager.Ins.SetPlayerDirection(direction);
        HInputManager.SetDirectionInput(direction);
        Debug.Log("On Move");
        if (dpadFloating is null) return;
        DOTween.Kill(dpadFloating);
    }

    public void OnButtonPointerUp()
    {
        pointerDownImg.SetActive(false);
        // LevelManager.Ins.SetPlayerDirection(Direction.None);
        HInputManager.SetDirectionInput(Direction.None);
        Debug.Log("Release Move");
        if (dpadFloating is null) return;
        DOVirtual.DelayedCall(dpadFloating.timeToHideButton, () =>
        {
            dpadFloating.HideButton();
            dpadFloating.emptyGraphic.enabled = true;
        }).SetId(dpadFloating);
    }
}
