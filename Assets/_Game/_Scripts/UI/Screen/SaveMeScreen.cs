using _Game.DesignPattern;
using _Game.Managers;

namespace _Game.UIs.Screen
{
    public class SaveMeScreen : UICanvas
    {
        public override void Setup(object param = null)
        {
            base.Setup(param);
        }
        
        public override void Close()
        {
            base.Close();
        }

        private void OnClickMoreTime()
        {
            GameManager.Ins.PostEvent(EventID.MoreTimeGame);
            Close();
        }
    }
}
