using _Game._Scripts.InGame;
using _Game.DesignPattern;
using _Game.Managers;
using _Game.UIs.Screen;
using HControls;
using UnityEngine;

namespace _Game._Scripts.Managers
{
    public class ChooseLevelManager : Singleton<ChooseLevelManager>
    {
        [SerializeField] private int currentLevelIndex;
        private Level _currentLevel;
        private Level _nextLevel;
        private Level _previousLevel;
        

        [SerializeField] private int initializeX = 0;
        [SerializeField] private float moveSpeed = 20f;
        
        private Direction _previousDirection = Direction.None;
        private void Update()
        {
            Direction direction = HInputManager.GetDirectionInput();
            switch (direction)
            {
                case Direction.None:
                    _previousDirection = Direction.None;
                    return;
                case Direction.Left or Direction.Right:
                    return;
            }
            // else move the CameraTargetPosZ
            if (_previousDirection is Direction.None)
            {
                if (direction is Direction.Forward)
                {
                    SeePreviousLevel();
                }
                else if (direction is Direction.Back)
                {
                    SeeNextLevel();
                }
            }
            _previousDirection = direction;
        }

        private void OnEnable()
        {
            UIManager.Ins.CloseAll();
            UIManager.Ins.OpenUI<ChooseLevelScreen>();
            OnFirstSpawnLevel(currentLevelIndex);
        }

        [ContextMenu("See Next Level")]
        public void SeeNextLevel()
        {
            int nextLevelIndex = currentLevelIndex + 1;
            if (nextLevelIndex >= DataManager.Ins.CountLevel) return;
            currentLevelIndex += 1;
            // Despawn previous level
            if (_previousLevel is not null)
            {
                _previousLevel.OnDeSpawnLevel();
                _previousLevel = null;
            }

            // Previous level become current level
            _previousLevel = _currentLevel;
            // Current level become next level
            _currentLevel = _nextLevel;
            // Spawn next level
            Vector3 nextLevelPos = new(0, 0, _currentLevel.GetMaxZPos() + 5f);
            _nextLevel = nextLevelIndex + 1 < DataManager.Ins.CountLevel
                ? new Level(nextLevelIndex + 1)
                : null;
            // Change camera target position to center of first island of current level
            CameraManager.Ins.ChangeCameraTargetPosition(_currentLevel.GetIsland(0).GetCenterIslandPos() + Vector3.up * 5f);
        }

        [ContextMenu("See Previous Level")]
        public void SeePreviousLevel()
        {
            int previousLevelIndex = currentLevelIndex - 1;
            if (previousLevelIndex < 0) return;
            currentLevelIndex -= 1;
            // Despawn next level
            if (_nextLevel is not null)
            {
                _nextLevel.OnDeSpawnLevel();
                _nextLevel = null;
            }

            // Next level become current level
            _nextLevel = _currentLevel;
            // Current level become previous level
            _currentLevel = _previousLevel;
            // Spawn previous level
            Vector3 previousLevelPos = new(0, 0, _currentLevel.GetMinZPos() - 5f);
            _previousLevel = previousLevelIndex - 1 >= 0
                ? new Level(previousLevelIndex - 1)
                : null;
            CameraManager.Ins.ChangeCameraTargetPosition(_currentLevel.GetIsland(0).GetCenterIslandPos() + Vector3.up * 5f, 1.5f);
        }

        private void OnFirstSpawnLevel(int index)
        {
            _currentLevel = new Level(index);
            CameraManager.Ins.ChangeCameraTargetPosition(_currentLevel.GetIsland(0).GetCenterIslandPos() + Vector3.up * 5f, 1.5f);
            initializeX = _currentLevel.GridSizeX;
            
            if (index - 1 >= 0)
            {
                
                Vector3 previousLevelPos = new(0, 0, _currentLevel.GetMinZPos() - 5f);
                _previousLevel = new Level(index - 1);
            }

            if (index + 1 < DataManager.Ins.CountLevel)
            {
                Vector3 nextLevelPos = new(0, 0, _currentLevel.GetMaxZPos() + 5f);
                _nextLevel = new Level(index + 1);
            }
            
           
        }
    }
}
