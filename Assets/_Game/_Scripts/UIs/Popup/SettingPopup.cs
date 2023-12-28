using _Game.GameGrid;
using _Game.Managers;
using _Game.UIs.Screen;
using UnityEngine;

namespace _Game.UIs.Popup
{
    public class SettingPopup : UICanvas
    {
        public void OnClickChangeBgmStatusButton()
        {
            Debug.Log("Click change bgm button");
        }

        public void OnClickChangeSfxStatusButton()
        {
            Debug.Log("Click change sfx button");
        }

        public void OnClickLikeButton()
        {
            Debug.Log("Click like button");
        }

        public void OnClickMoveOptionPopup()
        {
            UIManager.Ins.OpenUI<MoveOptionPopup>();
        }

        public void OnClickSelectLevelButton()
        {
            Close();
            UIManager.Ins.OpenUI<ChooseLevelScreen>();
        }
        
        public void OnClickGoMenuButton()
        {
            LevelManager.Ins.OnRestart();
            UIManager.Ins.CloseAll();
            UIManager.Ins.OpenUI<MainMenuScreen>();
        }

        public void OnClickHowToPlayButton()
        {
            Debug.Log("Click how to play button");
        }
    }
}
