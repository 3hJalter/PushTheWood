using _Game.DesignPattern;
using Sigtrap.Relays;
using UnityEngine;

namespace _Game._Scripts.Managers
{
    public class EventGlobalManager : Singleton<EventGlobalManager>
    {
        // Global events
        [HideInInspector] public Relay<int> OnMoneyChanged = new();
        [HideInInspector] public Relay OnStartLoadScene = new();
        [HideInInspector] public Relay OnFinishLoadScene = new();
        [HideInInspector] public Relay OnUpdateSetting = new();
        [HideInInspector] public Relay OnGameInit = new();
        [HideInInspector] public Relay OnPurchaseNoAds = new();
        
        // In-game events
        // Unit Button
        // ReSharper disable once Unity.RedundantHideInInspectorAttribute
        [HideInInspector] public readonly Relay<bool> OnButtonUnitEnter = new();
    }
}
