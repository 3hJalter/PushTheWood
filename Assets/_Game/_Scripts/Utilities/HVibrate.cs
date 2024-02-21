using _Game.Managers;
using MoreMountains.NiceVibrations;

namespace _Game._Scripts.Utilities
{
    public static class HVibrate
    {
        public static void Haptic(HapticTypes type)
        {
            if (!DataManager.Ins.GameData.setting.haptic)
                return;
            MMVibrationManager.Haptic(type, false, true);
        }
    }
}
