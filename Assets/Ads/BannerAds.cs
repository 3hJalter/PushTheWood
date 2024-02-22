using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BannerAds : MonoBehaviour
{
    string bannerAdUnitId = "6966fa233ec0364f"; // Retrieve the ID from your account

    // Start is called before the first frame update
    void Start()
    {
        MaxSdkCallbacks.OnSdkInitializedEvent += (MaxSdkBase.SdkConfiguration sdkConfiguration) =>
        {
            
            // AppLovin SDK is initialized, start loading ads
            // Banners are automatically sized to 320×50 on phones and 728×90 on tablets
            // You may call the utility method MaxSdkUtils.isTablet() to help with view sizing adjustments
            MaxSdk.CreateBanner(bannerAdUnitId, MaxSdkBase.BannerPosition.BottomCenter);

            // Set background or background color for banners to be fully functional
            MaxSdk.SetBannerBackgroundColor(bannerAdUnitId, Color.black);
        };

        MaxSdk.SetSdkKey("ZoNyqu_piUmpl33-qkoIfRp6MTZGW9M5xk1mb1ZIWK6FN9EBu0TXSHeprC3LMPQI7S3kTc1-x7DJGSV8S-gvFJ");
        MaxSdk.SetUserId("USER_ID");
        MaxSdk.InitializeSdk();
    }

    public void Show()
    {
        MaxSdk.ShowBanner(bannerAdUnitId);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
