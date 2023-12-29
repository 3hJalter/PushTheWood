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

        private Level _middleLevel;
        private Level _headerLevel;
        private Level _footerLevel;

        [SerializeField] private float offset = 8f;
        
        private void Update()
        {
            // Only Debug
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                Tf.position += Vector3.forward * 10f;
            }
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                Tf.position += Vector3.back * 10f;
            }

            if (Tf.position.z > _middleLevel.GetMaxZPos() - offset) // offset
            {
                PreloadNextLevel();
            }
            else if (Tf.position.z < _footerLevel.GetMaxZPos() - offset)
            {
                PreLoadPreviousLevel();
            }
        }
        
        private void PreloadNextLevel()
        {
            int currentLevelIndex = LevelManager.Ins.CurrentLevel.Index;
            int nextPreloadIndex = _headerLevel.Index + 1;
            if (nextPreloadIndex >= DataManager.Ins.CountLevel) return;
            if (_footerLevel is not null && _footerLevel.Index != currentLevelIndex)
            {
                // Despawn Level
                _footerLevel.OnDeSpawnLevel();
            }
            _footerLevel = _middleLevel;
            _middleLevel = _headerLevel;
            _headerLevel = nextPreloadIndex == currentLevelIndex ? LevelManager.Ins.CurrentLevel : new Level(nextPreloadIndex);
            nearestLevelIndex = _footerLevel.Index;
            furthestLevelIndex = _headerLevel.Index;
        }
        
        private void PreLoadPreviousLevel()
        {
            int currentLevelIndex = LevelManager.Ins.CurrentLevel.Index;
            int previousPreloadIndex = _footerLevel.Index - 1;
            if (previousPreloadIndex < 0) return;
            if (_headerLevel is not null && _headerLevel.Index != currentLevelIndex)
            {
                // Despawn Level
                _headerLevel.OnDeSpawnLevel();
            }
            _headerLevel = _middleLevel;
            _middleLevel = _footerLevel;
            _footerLevel = previousPreloadIndex == currentLevelIndex ? LevelManager.Ins.CurrentLevel : new Level(previousPreloadIndex);
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

        private void OnDisable()
        {
            _middleLevel = LevelManager.Ins.CurrentLevel;
        }

        bool _isInit = false;

        private void OnActive()
        {
            _middleLevel = LevelManager.Ins.CurrentLevel;
            if (_middleLevel.Index > 0)
            {
                _footerLevel = new Level(_middleLevel.Index - 1);
            }
            if (_middleLevel.Index < DataManager.Ins.CountLevel - 2)
            {
                _headerLevel = new Level(_middleLevel.Index + 1);
            }
        }
        
        private void Start()
        {
            OnActive();
            _isInit = true;
        }

        IEnumerator<float> WaitToActive()
        {
            yield return Timing.WaitForOneFrame;
            OnActive();
            _isInit = true;
        }

        private void OnEnable()
        {
            if (!_isInit) return;
            OnActive();
        }
    }
}
