using System.Collections.Generic;
using UnityEngine;

namespace UB.Simple2dWeatherEffects.Standard
{
    public class EffectBase : MonoBehaviour
    {
        public static Dictionary<string, RenderTexture> AlreadyRendered = new();

        public static bool InsideRendering { get; set; } = false;
    }
}
