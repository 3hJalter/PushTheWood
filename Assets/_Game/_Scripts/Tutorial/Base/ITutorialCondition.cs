﻿using System.Collections.Generic;
using _Game.GameGrid;
using _Game.GameGrid.Unit;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Game._Scripts.Tutorial
{
    public interface ITutorialCondition
    {
        void HandleShowTutorial(GameGridCell cell, GridUnit triggerUnit);
        
        void HandleShowTutorial(GridUnit triggerUnit, GridUnit targetUnit);
        
        // void HandleShowTutorial();
        
        void ResetTutorial();
    }
    
    public abstract class BaseTutorialData : SerializedScriptableObject
    {
        // ReSharper disable once Unity.RedundantSerializeFieldAttribute
        [SerializeField] protected List<TutorialScreen> tutorialScreens = new();
        protected int currentScreenIndex;
        
        [ContextMenu("Reset Tutorial")]
        public void ResetTutorial()
        {
            currentScreenIndex = 0;
        }
    }
}