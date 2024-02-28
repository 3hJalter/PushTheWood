using System;
using _Game.Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VinhLB;
using _Game.GameGrid;
using TMPro;

namespace _Game.UIs.Screen
{
    public class DebugMainMenuScreen : UICanvas
    {
        [Header("General")]
        [SerializeField]
        private Toggle _debugMenusToggle;
        [SerializeField]
        private GameObject _menusGO;
        [SerializeField]
        private Button _backButton;
        
        [Header("Debug Menu")]
        [SerializeField]
        private GameObject _debugMenuGO;
        [SerializeField] 
        private Button _resourceButton;
        [SerializeField]
        private Button _dailyRewardButton;

        [Header("Resource Menu")]
        [SerializeField]
        private GameObject _resourceMenuGO;
        [SerializeField] 
        private Button _addGoldButton;
        [SerializeField]
        private Button _addAdTicketsButton;
        [SerializeField]
        private Button _addSecretMapPieceButton;
        [SerializeField]
        private Button _resetResourceButton;
        [SerializeField]
        private Button _changePlayerSkinButton;
        [SerializeField]
        private TMP_InputField _playerSkinInputField;
        
        [Header("Daily Reward Menu")]
        [SerializeField]
        private GameObject _dailyRewardMenuGO;
        [SerializeField] 
        private Button _setCanCollectTodayButton;
        [SerializeField]
        private Button _increase1DailyDay;
        [SerializeField]
        private Button _decrease1DailyDayButton;
        [SerializeField]
        private Button _resetDailyRewardButton;

        private void Awake()
        {
            _debugMenusToggle.onValueChanged.AddListener(ToggleDebugMenu);
            _backButton.onClick.AddListener(BackToDebugMenu);
            
            _resourceButton.onClick.AddListener(ResourceButton);
            _dailyRewardButton.onClick.AddListener(DailyRewardButton);
            
            _addAdTicketsButton.onClick.AddListener(AddAdTickets);
            _addGoldButton.onClick.AddListener(AddGold);
            _addSecretMapPieceButton.onClick.AddListener(AddSecretMapPiece);
            _resetResourceButton.onClick.AddListener(ResetUserData);
            _changePlayerSkinButton.onClick.AddListener(ChangePlayerSkin);
            
            _setCanCollectTodayButton.onClick.AddListener(DailyRewardManager.SetCanCollectToday);
            _increase1DailyDay.onClick.AddListener(DailyRewardManager.Increase1DailyDay);
            _decrease1DailyDayButton.onClick.AddListener(DailyRewardManager.Decrease1DailyDay);
            _resetDailyRewardButton.onClick.AddListener(DailyRewardManager.ResetAll);

            _playerSkinInputField.text = DataManager.Ins.GameData.user.currentPlayerSkinIndex.ToString();
        }

        private void OnEnable()
        {
            BackToDebugMenu();
        }
        
        private void ToggleDebugMenu(bool value)
        {
            _menusGO.SetActive(value);
        }
        
        private void BackToDebugMenu()
        {
            _backButton.gameObject.SetActive(false);
            _debugMenuGO.SetActive(true);
            _resourceMenuGO.SetActive(false);
            _dailyRewardMenuGO.SetActive(false);
        }

        private void ResourceButton()
        {
            _backButton.gameObject.SetActive(true);
            _debugMenuGO.SetActive(false);
            _resourceMenuGO.SetActive(true);
        }

        private void DailyRewardButton()
        {
            _backButton.gameObject.SetActive(true);
            _debugMenuGO.SetActive(false);
            _dailyRewardMenuGO.SetActive(true);
        }

        private void AddAdTickets()
        {
            GameManager.Ins.GainAdTickets(1, _addAdTicketsButton.transform.position);
            //UIManager.Ins.UpdateUIs();
        }
        
        private void AddGold()
        {
            GameManager.Ins.GainGold(100, _addGoldButton.transform.position);
            //UIManager.Ins.UpdateUIs();
        }
        
        private void ChangePlayerSkin()
        {
            int index = int.Parse(_playerSkinInputField.text);
            DataManager.Ins.GameData.user.currentPlayerSkinIndex = index;
            LevelManager.Ins.player?.ChangeSkin(index);
        }
        private void AddSecretMapPiece()
        {
            GameManager.Ins.GainSecretMapPiece(1);
        }

        private void ResetUserData()
        {
            GameManager.Ins.ResetUserData();
            //UIManager.Ins.UpdateUIs();
        }       
    }
}