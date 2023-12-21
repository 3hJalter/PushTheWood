using UnityEngine;
using UnityEngine.UI;

public class BF_MowingUI : MonoBehaviour
{
    public float MowingProgress = 100f;

    public Text percentageText;
    public BF_MowingManager mowingManager;

    private void Update()
    {
        float marginValue = mowingManager.totalMarker * mowingManager.marginError;
        float normalizedValue =
            (mowingManager.markersPos.Count - marginValue) / (mowingManager.totalMarker - marginValue);
        normalizedValue =
            Mathf.Clamp01(normalizedValue - Mathf.Lerp(0, mowingManager.marginError, 1 - normalizedValue));
        percentageText.text = Mathf.RoundToInt(normalizedValue * 100f) + " %";
        MowingProgress = Mathf.RoundToInt(normalizedValue * 100f);
    }
}
