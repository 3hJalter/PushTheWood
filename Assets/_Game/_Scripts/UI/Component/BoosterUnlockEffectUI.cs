﻿using System.Collections.Generic;
using _Game.Managers;
using _Game.Resource;
using _Game.UIs.Screen;
using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using VinhLB;

namespace _Game._Scripts.UIs.Component
{
    public class BoosterUnlockEffectUI : MonoBehaviour
    {
        [SerializeField] private OverlayScreen overlay;
        [SerializeField] private float jumpPower = 20;
        [SerializeField] private float jumpDuration = 1;
        [SerializeField] private Ease moveEase = Ease.OutQuad;
        [SerializeField] private Ease scaleEase = Ease.OutQuad;
        [SerializeField] private RectTransform icon;
        [SerializeField] private Image boosterImage;
        [SerializeField] private TextMeshProUGUI boosterAmount;
        private BoosterButton _boosterButton;
        private Vector3 _initIconScale;
        [SerializeField] private BoosterType test;

        [SerializeField] private List<Sprite> boosterIcon;
        
        
        private Vector3 _initPosition;
        private Transform tf;
        private Transform Tf => tf ??= GetComponent<Transform>();
        
        private void Awake()
        {
            // store Init Position
            _initIconScale = icon.localScale;
            _initPosition = Tf.position;
        }

        private void OnDisable()
        {
            Tf.position = _initPosition;
            icon.localScale = _initIconScale;
        }

        public void PlayUnlockEffect(BoosterType type)
        {
            _boosterButton = GetBoosterButton(type);
            if (_boosterButton is null) return;
            gameObject.SetActive(true);
            _boosterButton.gameObject.SetActive(false);
            ChangeIcon(type);
            // Get rect position of booster button
            Transform boosterButtonRect = _boosterButton.transform;
            // Get rect position of overlay screen
            Sequence s = DOTween.Sequence();
            
            s.Append(Tf.DOJump(boosterButtonRect.position, jumpPower, 1,  jumpDuration).SetEase(moveEase))
                .Join(icon.DOScaleX(1, jumpDuration).SetEase(scaleEase))
                .Join(icon.DOScaleY(1, jumpDuration).SetEase(scaleEase))
                .OnComplete(() =>
            {
                _boosterButton.gameObject.SetActive(true);
                gameObject.SetActive(false);
                overlay.Close();
            });
        }
        
        [Button]
        public void Test()
        {
            UIManager.Ins.OpenUI<OverlayScreen>();
            PlayUnlockEffect(test);
        }

        private void ChangeIcon(BoosterType type)
        {
            boosterImage.sprite = boosterIcon[(int) type];
            boosterAmount.text = type switch
            {
                BoosterType.Undo => 10.ToString(),
                BoosterType.PushHint => 3.ToString(),
                BoosterType.GrowTree => 5.ToString(),
                _ => 0.ToString()
            };
        }
        
        private static BoosterButton GetBoosterButton(BoosterType type)
        {
            InGameScreen ui = UIManager.Ins.GetUI<InGameScreen>();
            return type switch
            {
                BoosterType.Undo => ui.undoButton,
                BoosterType.PushHint => ui.pushHintButton,
                BoosterType.GrowTree => ui.growTreeButton,
                BoosterType.ResetIsland => ui.resetIslandButton,
                _ => null
            };
        }
        
    }
}
