using System.Collections.Generic;
using _Game.Utilities.Timer;
using HControls;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace _Game._Scripts.Tutorial
{
    public class TutorialScreenPage : TutorialScreen
    {
        [SerializeField] private HButton nextButton;
        [SerializeField] private HButton prevButton;
        [SerializeField] private HButton closeButton;
        
        [SerializeField] private GameObject pageContainer;
        [SerializeField] private List<GameObject> pages;
        [SerializeField] private Image panel;

        private int _currentPageIndex;

        public override void Setup(object param = null)
        {
            base.Setup(param);
            panel.color = new Color(0,0,0,0);
            _currentPageIndex = 0;
            HInputManager.LockInput();
            pages.ForEach(page => page.SetActive(false));
            nextButton.gameObject.SetActive(false);
            prevButton.gameObject.SetActive(false);
            // Add event for buttons
            nextButton.onClick.AddListener(NextPage);
            prevButton.onClick.AddListener(PrevPage);
            closeButton.onClick.AddListener(() => CloseDirectly());
            // Wait to zoom in camera
            TimerManager.Ins.WaitForTime(1.5f, () =>
            {
                panel.color = new Color(0,0,0,0.65f);
                pages[_currentPageIndex].SetActive(true);
                // Set active for next button if there are more than 1 page
                nextButton.gameObject.SetActive(pages.Count > 1);
            });
        }

        public override void CloseDirectly(object param = null)
        {
            // Remove event for buttons
            nextButton.onClick.RemoveListener(NextPage);
            prevButton.onClick.RemoveListener(PrevPage);
            closeButton.onClick.RemoveListener(() => CloseDirectly());
            HInputManager.LockInput(false);
            base.CloseDirectly(param);
        }

        private void NextPage()
        {
            if (_currentPageIndex < pages.Count - 1)
            {
                pages[_currentPageIndex].SetActive(false);
                _currentPageIndex++;
                pages[_currentPageIndex].SetActive(true);
            }
            // hide next button if it's the last page
            nextButton.gameObject.SetActive(_currentPageIndex != pages.Count - 1);
            // show prev button if it's not the first page
            prevButton.gameObject.SetActive(_currentPageIndex != 0);
        }
        
        private void PrevPage()
        {
            if (_currentPageIndex > 0)
            {
                pages[_currentPageIndex].SetActive(false);
                _currentPageIndex--;
                pages[_currentPageIndex].SetActive(true);
            }
            // hide prev button if it's the first page
            prevButton.gameObject.SetActive(_currentPageIndex != 0);
            // show next button if it's not the last page
            nextButton.gameObject.SetActive(_currentPageIndex != pages.Count - 1);
        }
    }
}
