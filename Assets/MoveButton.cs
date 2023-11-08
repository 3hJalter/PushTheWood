using System.Collections;
using System.Collections.Generic;
using _Game.GameGrid;
using DG.Tweening;
using UnityEngine;

public class MoveButton : HMonoBehaviour
{
    [SerializeField] private Direction direction;
    [SerializeField] private FloatingMoveInputButton floatingMoveInputButton;
    public void OnButtonPointerDown()
    {
        LevelManager.Ins.SetPlayerDirection(direction);
        Debug.Log("On Move");
        DOTween.Kill(floatingMoveInputButton);
    }

    public void OnButtonPointerUp()
    {
        LevelManager.Ins.SetPlayerDirection(Direction.None);
        Debug.Log("Release Move");
        DOVirtual.DelayedCall(floatingMoveInputButton.timeToHideButton, () =>
        {
            floatingMoveInputButton.HideButton();
            floatingMoveInputButton.emptyGraphic.enabled = true;
        }).SetId(floatingMoveInputButton);
    }
}
