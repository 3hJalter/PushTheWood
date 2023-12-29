using System;
using System.Collections.Generic;
using _Game.GameGrid;
using _Game.Managers;
using MEC;
using UnityEngine;

namespace _Game._Scripts.InGame
{
    public class WorldMapTarget : HMonoBehaviour
    {
        [SerializeField] private Transform header;
        [SerializeField] private Transform footer;

        [SerializeField] private int nearestLevelIndex;
        [SerializeField] private int furthestLevelIndex;

        [SerializeField] private float offset = 8f;
        private Level _footerLevel;
        private Level _headerLevel;

        private bool _isInit;

        private Level _middleLevel;

        private void Start()
        {
            OnActive();
            _isInit = true;
        }

        private void Update()
        {
            // Only Debug
            if (Input.GetKeyDown(KeyCode.UpArrow)) Tf.position += Vector3.forward * 10f;
            if (Input.GetKeyDown(KeyCode.DownArrow)) Tf.position += Vector3.back * 10f;
            //
            
            
            if (Tf.position.z > _middleLevel.GetMaxZPos() - offset) // offset
                PreloadNextLevel();
            else if (Tf.position.z < _footerLevel.GetMaxZPos() - offset) PreLoadPreviousLevel();
        }

        private void OnEnable()
        {
            if (!_isInit) return;
            OnActive();
        }

        private void OnDisable()
        {
            if (CanDespawn(_headerLevel)) _headerLevel.OnDeSpawnLevel();
            if (CanDespawn(_footerLevel)) _footerLevel.OnDeSpawnLevel();
            if (CanDespawn(_middleLevel)) _middleLevel.OnDeSpawnLevel();
            ResetData();
        }

        private bool CanDespawn(Level level)
        {
            if (level is null) return false;
            int currentLevelIndex = LevelManager.Ins.CurrentLevel.Index;
            if (level.Index == currentLevelIndex) return false;
            if (level.Index == currentLevelIndex + 1) return false;
            if (level.Index == currentLevelIndex - 1) return false;
            return true;
        }
        
        private void PreloadNextLevel()
        {
            int currentLevelIndex = LevelManager.Ins.CurrentLevel.Index;
            int nextPreloadIndex = _headerLevel.Index + 1;
            if (nextPreloadIndex >= DataManager.Ins.CountLevel) return;
            if (CanDespawn(_footerLevel))
            {
                _footerLevel.OnDeSpawnLevel();
            }
            _footerLevel = _middleLevel;
            _middleLevel = _headerLevel;
            
            if (LevelManager.Ins.PreLoadLevels.ContainsKey(nextPreloadIndex))
                _headerLevel = LevelManager.Ins.PreLoadLevels[nextPreloadIndex];
            else if (nextPreloadIndex == currentLevelIndex)
                _headerLevel = LevelManager.Ins.CurrentLevel;
            else
                _headerLevel = new Level(nextPreloadIndex);
            nearestLevelIndex = _footerLevel.Index;
            furthestLevelIndex = _headerLevel.Index;
        }

        private void PreLoadPreviousLevel()
        {
            int currentLevelIndex = LevelManager.Ins.CurrentLevel.Index;
            int previousPreloadIndex = _footerLevel.Index - 1;
            if (previousPreloadIndex < 0) return;
            if (CanDespawn(_headerLevel))
            {
                _headerLevel.OnDeSpawnLevel();
            }
            _headerLevel = _middleLevel;
            _middleLevel = _footerLevel;
            if (LevelManager.Ins.PreLoadLevels.ContainsKey(previousPreloadIndex))
                _footerLevel = LevelManager.Ins.PreLoadLevels[previousPreloadIndex];
            else if (previousPreloadIndex == currentLevelIndex)
                _footerLevel = LevelManager.Ins.CurrentLevel;
            else
                _footerLevel = new Level(previousPreloadIndex);
            nearestLevelIndex = _footerLevel.Index;
            furthestLevelIndex = _headerLevel.Index;
        }

        public float GetHeaderZPos()
        {
            return header.position.z;
        }

        public float GetFooterZPos()
        {
            return footer.position.z;
        }
        
        private void ResetData()
        {
            _middleLevel = null;
            _headerLevel = null;
            _footerLevel = null;
        }
        
        private void OnActive()
        {
            _middleLevel = LevelManager.Ins.CurrentLevel;
            // if Preload contains current level + 1, _headerLevel = Preload[current level + 1]
            if (LevelManager.Ins.PreLoadLevels.ContainsKey(_middleLevel.Index + 1))
                _headerLevel = LevelManager.Ins.PreLoadLevels[_middleLevel.Index + 1];
            // if Preload contains current level - 1, _footerLevel = Preload[current level - 1]
            if (LevelManager.Ins.PreLoadLevels.ContainsKey(_middleLevel.Index - 1))
                _footerLevel = LevelManager.Ins.PreLoadLevels[_middleLevel.Index - 1];
        }
    }
}
