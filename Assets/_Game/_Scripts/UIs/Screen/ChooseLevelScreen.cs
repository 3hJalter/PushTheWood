using _Game._Scripts.Managers;
using _Game.Camera;
using _Game.GameGrid;
using _Game.Managers;
using _Game.Utilities;
using HControls;
using UnityEngine;

namespace _Game.UIs.Screen
{
    public class ChooseLevelScreen : UICanvas
    {
        private Direction _previousDirection;
        
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
        
        public override void Setup()
        {
            base.Setup();
            GameManager.Ins.ChangeState(GameState.WorldMap);
            _previousDirection = Direction.None;
            // CurveWorld
            FxManager.Ins.ChangePlanetCurvatureSize();
            CameraManager.Ins.ChangeWorldTargetPosition();
            CameraManager.Ins.ChangeCamera(ECameraType.WorldMapCamera);
        }

        public override void Close()
        {
            FxManager.Ins.ChangePlanetCurvatureSize(0f);
            CameraManager.Ins.ChangeCamera(ECameraType.InGameCamera);
            LevelManager.Ins.SetCameraToPlayerIsland();
            UIManager.Ins.OpenUI<InGameScreen>();
            base.Close();
        }

        public void SeeNextLevel()
        {
            DevLog.Log(DevId.Hoang, "SeeNextLevel");
            // ChooseLevelManager.Ins.SeeNextLevel();
        }
        
        public void SeePreviousLevel()
        {
            DevLog.Log(DevId.Hoang, "SeePreviousLevel");
            // ChooseLevelManager.Ins.SeePreviousLevel();
        }
    }
}
