using _Game.Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.UIs.Screen
{
    public class DebugMainMenuScreen : UICanvas
    {
        [SerializeField]
        Button addGems;
        [SerializeField] 
        Button addGolds;
        [SerializeField]
        Button addSecretMapPieceBtn;
        [SerializeField]
        Button resetBtn;

        private void Awake()
        {
            addGems.onClick.AddListener(AddGems);
            addGolds.onClick.AddListener(AddGolds);
            addSecretMapPieceBtn.onClick.AddListener(AddSecretMapPiece);
            resetBtn.onClick.AddListener(ResetUserData);
        }
      
        private void AddGems()
        {
            GameManager.Ins.AddGem(10);
            UIManager.Ins.UpdateUIs();
        }
        private void AddGolds()
        {
            GameManager.Ins.AddGold(100);
            UIManager.Ins.UpdateUIs();
        }
        private void AddSecretMapPiece()
        {
            GameManager.Ins.AddSecretMapPiece(1);
            UIManager.Ins.UpdateUIs();
        }

        private void ResetUserData()
        {
            GameManager.Ins.ResetUserData();
            UIManager.Ins.UpdateUIs();
        }       
    }
}