using _Game.DesignPattern;
using _Game.GameGrid;
using _Game.Utilities.Grid;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[DefaultExecutionOrder(-100)]
public class InitManager : Singleton<InitManager>
{
    [SerializeField]
    bool isDebug = false;
    #region PROPERTYS
    [SerializeField]
    GameObject debugObject;
    #endregion
 
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        if (isDebug)
        {
            DontDestroyOnLoad(Instantiate(debugObject));
        }
    }
    private void Start()
    {
        //NOTE: ASync here later
        SceneManager.LoadScene(1);
    }

    
}
