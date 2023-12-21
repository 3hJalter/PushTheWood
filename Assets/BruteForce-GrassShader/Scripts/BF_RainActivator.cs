using UnityEngine;

public class BF_RainActivator : MonoBehaviour
{
    public Material skyboxDefault;
    public Material skyboxRain;
    public Color fogRain;
    public Color equatorColorRain;

    [Space] public bool isHDRP;

    public Camera overrideCam;
    public Camera toDisableCam;

    private Color equatorColorDefault;

    private Color fogDefault;

    private void Awake()
    {
        fogDefault = RenderSettings.fogColor;
        equatorColorDefault = RenderSettings.ambientEquatorColor;
    }

    private void OnEnable()
    {
        EnableRain();
    }

    private void OnDisable()
    {
        DisableRain();
    }

    private void OnDestroy()
    {
        DisableRain();
    }

    private void EnableRain()
    {
        if (!isHDRP)
        {
            RenderSettings.fogColor = fogRain;
            RenderSettings.skybox = skyboxRain;
            RenderSettings.ambientEquatorColor = equatorColorRain;
        }
        else
        {
            if (!overrideCam.gameObject.activeInHierarchy) overrideCam.gameObject.SetActive(true);
        }
    }

    private void DisableRain()
    {
        if (!isHDRP)
        {
            RenderSettings.fogColor = fogDefault;
            RenderSettings.skybox = skyboxDefault;
            RenderSettings.ambientEquatorColor = equatorColorDefault;
        }
        else
        {
            if (overrideCam.gameObject.activeInHierarchy) overrideCam.gameObject.SetActive(false);
        }
    }
}
