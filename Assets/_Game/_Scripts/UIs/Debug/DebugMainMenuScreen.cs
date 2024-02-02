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
        private Button addGoldBtn;
        [SerializeField]
        private Button addGemsBtn;
        [SerializeField]
        private Button addSecretMapPieceBtn;
        [SerializeField]
        private Button resetBtn;

        private void Awake()
        {
            addGemsBtn.onClick.AddListener(AddGems);
            addGoldBtn.onClick.AddListener(AddGold);
            addSecretMapPieceBtn.onClick.AddListener(AddSecretMapPiece);
            resetBtn.onClick.AddListener(ResetUserData);
        }
      
        private void AddGems()
        {
            GameManager.Ins.GainGems(10, addGemsBtn.transform.position);
            // UIManager.Ins.UpdateUIs();
        }
        private void AddGold()
        {
            GameManager.Ins.GainGold(100, addGoldBtn.transform.position);
            // UIManager.Ins.UpdateUIs();
        }
        private void AddSecretMapPiece()
        {
            GameManager.Ins.GainSecretMapPiece(1);
            // UIManager.Ins.UpdateUIs();
        }

        private void ResetUserData()
        {
            GameManager.Ins.ResetUserData();
            // UIManager.Ins.UpdateUIs();
        }       
    }
}