using System;
using System.Collections.Generic;
using _Game;
using Cinemachine;
using DG.Tweening;
using TMPro;
using UB.Simple2dWeatherEffects.Standard;
using UnityEngine;
using UnityEngine.UI;

namespace Daivq
{
    [DefaultExecutionOrder(-1)]
    public class OldUIManager : MonoBehaviour
    {
        private const int CAMERA_PRIORITY_ACTIVE = 99;
        private const int CAMERA_PRIORITY_INACTIVE = 1;
        
        public enum State
        {
            MAIN_MENU,
            IN_GAME,
            SETTING,
            WIN_POPUP
        }

        private const string KEY_PASS_TUT = "IsPassTut";

   

        [Header("General")] [SerializeField] private D2FogsPE _fogControl;

        [SerializeField] private SwipeDetector _swipeDetector;
        [SerializeField] private float _startFogDensity = 1f;
        [SerializeField] private float _durationShowCanvasGroup = 0.5f;
        [SerializeField] private float _durationHideCanvasGroup = 1f;

        [Header("Main Menu")] [SerializeField] private UISectionSetting _sectionMainMenu;

        [SerializeField] private UISectionSetting _sectionInGame;
        [SerializeField] private UISectionSetting _sectionWin;
        [SerializeField] private UISectionSetting _sectionSetting;
        [SerializeField] private UISectionSetting _sectionMap;


        [Header("Main Menu")] [SerializeField] private Button _btnPlay;

        [SerializeField] private float _durationTweenFogFromMainMenu = 1f;
        [SerializeField] private float _delayShowMainMenu = 1f;
        [SerializeField] private float _delayHideMainMenu = 1f;

        [Header("Tut")] [SerializeField] private CanvasGroup _canvasTut;

        [SerializeField] private Button _btnCloseTut;
        [SerializeField] private float _delayShowTuts = 1f;

        [Header("In Game")] [SerializeField] private Button _btnSetting;

        [SerializeField] private Button _btnMap;
        [SerializeField] private Button _btnUndo;
        [SerializeField] private Button _btnReset;
        [SerializeField] private float _fogTransitionEnterIngame = 0.5f;
        public TMP_Text _stepText;

        [Header("In Control")] [SerializeField]
        private Button _btnMoveUp;

        [SerializeField] private Button _btnMoveDown;
        [SerializeField] private Button _btnMoveLeft;
        [SerializeField] private Button _btnMoveRight;
        public Transform ControlBtns;

        [Header("Setting")] [SerializeField] private Button[] _btnsCloseSetting;

        [Header("Win")] [SerializeField] private Button _btnsNextLevel;

        [Header("WorldMap")] public List<Button> _teleportBtn;

        [SerializeField] private float _delayShowWorldMap = 2f;
        [SerializeField] private Button _backButton;

        private readonly float teleportFogDensity = 5f;
        private readonly float teleportTimeWait = 2f;
        private static bool IsPassTut => PlayerPrefs.GetInt(KEY_PASS_TUT, 0) == 1;
        public Vector2Int MoveDirectionFromButton { get; private set; }

        private void Start()
        {
            Application.targetFrameRate = 60;
            _swipeDetector.IsBlockPlayerInput.AddModifier(this);
            ShowMainMenu();
            _btnPlay.onClick.AddListener(StartPlayFromMainMenu);
            _btnReset.onClick.AddListener(Reset);
            _btnMap.onClick.AddListener(ShowWorldMap);
            _backButton.onClick.AddListener(WorldMapBack);
            _btnsNextLevel.onClick.AddListener(() => NextLevelButton(LevelManager.Ins.landIndex));
            SetTeleportPosition();

            void Reset()
            {
                LevelManager.Ins.Restart();
            }
        }

        private void LateUpdate()
        {
            MoveDirectionFromButton = Vector2Int.zero;
        }

        private static void SetPassTut()
        {
            PlayerPrefs.SetInt(KEY_PASS_TUT, 1);
        }

        private void SetTeleportPosition()
        {
            _teleportBtn[0].onClick.AddListener(() => TeleportMapTransition(0));
            _teleportBtn[1].onClick.AddListener(() => TeleportMapTransition(1));
            _teleportBtn[2].onClick.AddListener(() => TeleportMapTransition(2));
            _teleportBtn[3].onClick.AddListener(() => TeleportMapTransition(3));
            _teleportBtn[4].onClick.AddListener(() => TeleportMapTransition(4));
            _teleportBtn[5].onClick.AddListener(() => TeleportMapTransition(5));
            _teleportBtn[6].onClick.AddListener(() => TeleportMapTransition(6));
        }

        private void TeleportMapTransition(int index)
        {
            LevelManager.Ins.player.SetPosition(LevelManager.Ins.initPos[index]);
            WorldMapBack();
        }

        private void TeleportTransition(int index)
        {
            TweenFog(teleportFogDensity, _fogTransitionEnterIngame);
            TweenHideCanvasGroup(_sectionInGame.canvasGroup, _delayHideMainMenu);
            _sectionInGame.camera.Priority = CAMERA_PRIORITY_INACTIVE;
            _sectionMap.camera.Priority = CAMERA_PRIORITY_ACTIVE;
            _sectionInGame.canvasGroup.DOFade(0f, teleportTimeWait).SetEase(Ease.OutQuad).Play().OnComplete(() =>
            {
                TweenShowCanvasGroup(_sectionInGame.canvasGroup, _delayShowWorldMap);
                _sectionMap.camera.Priority = CAMERA_PRIORITY_INACTIVE;
                _sectionInGame.camera.Priority = CAMERA_PRIORITY_ACTIVE;
                LevelManager.Ins.player.SetPosition(LevelManager.Ins.initPos[index]);
            });
        }

        private void TweenFog(float fog, float duration = 1f)
        {
            if (!_fogControl) return;
            DOTween.To(GetDensity, SetDensity, fog, duration).SetEase(Ease.OutSine).Play();

            float GetDensity()
            {
                return _fogControl.Density;
            }

            void SetDensity(float value)
            {
                _fogControl.Density = value;
            }
        }

        private void TweenShowCanvasGroup(CanvasGroup canvasGroup, float delay = 0)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.DOFade(1f, _durationShowCanvasGroup).SetEase(Ease.OutQuad).SetDelay(delay).Play()
                .OnComplete(() => { canvasGroup.interactable = true; });
            canvasGroup.blocksRaycasts = true;
        }

        private void TweenHideCanvasGroup(CanvasGroup canvasGroup, float delay = 0f, TweenCallback onComplete = null)
        {
            canvasGroup.blocksRaycasts = false;
            canvasGroup.interactable = false;
            canvasGroup.DOFade(0f, _durationHideCanvasGroup).SetEase(Ease.OutQuad).SetDelay(delay)
                .OnComplete(onComplete).Play();
        }

        private void ShowMainMenu()
        {
            if (_fogControl) _fogControl.Density = _startFogDensity;
            TweenFog(_sectionMainMenu.fogDensity, _durationTweenFogFromMainMenu);
            TweenShowCanvasGroup(_sectionMainMenu.canvasGroup, _delayShowMainMenu);
            _sectionMainMenu.camera.Priority = CAMERA_PRIORITY_ACTIVE;
        }

        private void ShowWorldMap()
        {
            if (_fogControl) _fogControl.Density = _startFogDensity;
            TweenFog(_sectionMap.fogDensity, _fogTransitionEnterIngame);
            TweenHideCanvasGroup(_sectionInGame.canvasGroup, _delayHideMainMenu);
            TweenShowCanvasGroup(_sectionMap.canvasGroup, _delayShowWorldMap);
            _sectionInGame.camera.Priority = CAMERA_PRIORITY_INACTIVE;
            _sectionMap.camera.Priority = CAMERA_PRIORITY_ACTIVE;
        }

        private void WorldMapBack()
        {
            TweenFog(_sectionInGame.fogDensity, _fogTransitionEnterIngame);
            TweenHideCanvasGroup(_sectionMap.canvasGroup, _delayHideMainMenu);
            TweenShowCanvasGroup(_sectionInGame.canvasGroup, _delayShowWorldMap);
            _sectionMap.camera.Priority = CAMERA_PRIORITY_INACTIVE;
            _sectionInGame.camera.Priority = CAMERA_PRIORITY_ACTIVE;
        }

        private void StartPlayFromMainMenu()
        {
            TweenHideCanvasGroup(_sectionMainMenu.canvasGroup, _delayHideMainMenu, StartPlay);

            void StartPlay()
            {
                TweenFog(_sectionInGame.fogDensity, _fogTransitionEnterIngame);
                TweenShowCanvasGroup(_sectionInGame.canvasGroup);
                _sectionMainMenu.camera.Priority = CAMERA_PRIORITY_INACTIVE;
                _sectionInGame.camera.Priority = CAMERA_PRIORITY_ACTIVE;

                _sectionInGame.canvasGroup.interactable = false;

                StartGame();
                Tutorial.Ins.tutorialObj.SetActive(true);
                Tutorial.Ins.tapContinueButton.gameObject.SetActive(true);
            }
        }

        private void ShowTut()
        {
            TweenShowCanvasGroup(_canvasTut, _delayShowTuts);
            _btnCloseTut.onClick.AddListener(CloseTut);

            void CloseTut()
            {
                TweenHideCanvasGroup(_canvasTut);
                StartGame();
            }
        }

        private void StartGame()
        {
            _sectionInGame.canvasGroup.interactable = true;
            _swipeDetector.IsBlockPlayerInput.RemoveModifier(this);

            _btnSetting.onClick.AddListener(OpenSetting);

            void OpenSetting()
            {
                _swipeDetector.IsBlockPlayerInput.AddModifier(this);
                TweenShowCanvasGroup(_sectionSetting.canvasGroup);
                TweenFog(_sectionSetting.fogDensity);
                foreach (Button btn in _btnsCloseSetting) btn.onClick.AddListener(CloseSetting);
            }

            void CloseSetting()
            {
                _swipeDetector.IsBlockPlayerInput.RemoveModifier(this);
                TweenFog(_sectionInGame.fogDensity);
                TweenHideCanvasGroup(_sectionSetting.canvasGroup);
            }

            //_btnReset.onClick.AddListener(OnReset);

            _btnsNextLevel.onClick.AddListener(CloseWinPopup);
            _btnMoveUp.onClick.AddListener(Moveup);
            _btnMoveDown.onClick.AddListener(MoveDown);
            _btnMoveLeft.onClick.AddListener(MoveLeft);
            _btnMoveRight.onClick.AddListener(MoveRight);

            void Moveup()
            {
                MoveDirectionFromButton = Vector2Int.up;
            }

            void MoveDown()
            {
                MoveDirectionFromButton = Vector2Int.down;
            }

            void MoveLeft()
            {
                MoveDirectionFromButton = Vector2Int.left;
            }

            void MoveRight()
            {
                MoveDirectionFromButton = Vector2Int.right;
            }
        }

        public void ShowWin()
        {
            _swipeDetector.IsBlockPlayerInput.AddModifier(this);
            TweenShowCanvasGroup(_sectionWin.canvasGroup);
            TweenFog(_sectionWin.fogDensity);
        }

        private void NextLevelButton(int index)
        {
            LevelManager.Ins.steps = 0;
            if (LevelManager.Ins.winBySkip) return;
            TeleportTransition(index);
        }

        private void CloseWinPopup()
        {
            _swipeDetector.IsBlockPlayerInput.RemoveModifier(this);
            TweenFog(_sectionInGame.fogDensity);
            TweenHideCanvasGroup(_sectionWin.canvasGroup);
        }

        //public void SetUpCamera(Transform player)
        //{
        //    _sectionMainMenu.camera.Follow = player;
        //    _sectionMainMenu.camera.LookAt = player;
        //    _sectionInGame.camera.Follow = player;
        //    _sectionInGame.camera.LookAt = player;
        //}

        [Serializable]
        public class UISectionSetting
        {
            public CanvasGroup canvasGroup;
            public CinemachineVirtualCameraBase camera;
            public float fogDensity = 1f;
        }
    }
}
