using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using _Game.Utilities.UI;

public class InitCanvas : UICanvas
{
    public event Action<int, bool> _OnToggleValueChange;
    public event Action _OnStartGame;
    [SerializeField]
    UIToggle gridLogicDebugToggle;
    [SerializeField]
    UIToggle fpsDebugToggle;
    [SerializeField]
    UIToggle logToggle;
    [SerializeField]
    Button startButton;
    void Start()
    {
        gridLogicDebugToggle._OnValueChange = OnToggleValueChange;
        fpsDebugToggle._OnValueChange = OnToggleValueChange;
        logToggle._OnValueChange = OnToggleValueChange;
        startButton.onClick.AddListener(OnStartButtonClick);
    }

    public void SetData(bool value1, bool value2, bool value3)
    {
        gridLogicDebugToggle.SetValue(value1);
        fpsDebugToggle.SetValue(value2);
        logToggle.SetValue(value3);
    }

    private void OnToggleValueChange(int id, bool value)
    {
        _OnToggleValueChange?.Invoke(id, value);
    }

    private void OnStartButtonClick()
    {
        _OnStartGame?.Invoke();
    }

    private void OnDestroy()
    {
        startButton.onClick.RemoveListener(OnStartButtonClick);
    }
}