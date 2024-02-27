using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

namespace VinhLB
{
    public class CollectionPage : TabPage
    {
        [SerializeField]
        private TabGroup _topNavigationTabGroup;

        public override void Open(object param = null)
        {
            base.Open(param);
            
            if (param is true)
            {
                _topNavigationTabGroup.ResetSelectedTab(false);
            }
            else
            {
                _topNavigationTabGroup.ResetSelectedTab(true);
            }
        }

        public override void Close()
        {
            base.Close();
            
            _topNavigationTabGroup.ClearSelectedTab();
        }
    }
}