using System;
using UnityEngine;

#if GOOGLE_MOBILE_ADS
using GoogleMobileAds.Api;
#endif

public class AdsManager : MonoBehaviour
{
    public static AdsManager I;

#if UNITY_ANDROID
    const string APP_ID = "ca-app-pub-3940256099942544~3347511713";
    const string INTERSTITIAL_ID = "ca-app-pub-3940256099942544/1033173712";
    const string REWARDED_ID = "ca-app-pub-3940256099942544/5224354917";
    const string BANNER_ID = "ca-app-pub-3940256099942544/6300978111";
#elif UNITY_IOS
    const string APP_ID = "ca-app-pub-3940256099942544~1458002511";
    const string INTERSTITIAL_ID = "ca-app-pub-3940256099942544/4411468910";
    const string REWARDED_ID = "ca-app-pub-3940256099942544/1712485313";
    const string BANNER_ID = "ca-app-pub-3940256099942544/2934735716";
#else
    const string APP_ID = "";
    const string INTERSTITIAL_ID = "";
    const string REWARDED_ID = "";
    const string BANNER_ID = "";
#endif

#if GOOGLE_MOBILE_ADS
    InterstitialAd _interstitial;
    RewardedAd _rewarded;
    BannerView _banner;
#endif

    Action _onRewardGranted;
    Action _onInterstitialClosed;

    void Awake()
    {
        if (I != null)
        {
            Destroy(gameObject);
            return;
        }

        I = this;
        DontDestroyOnLoad(gameObject);

#if GOOGLE_MOBILE_ADS
        MobileAds.Initialize(_ =>
        {
            Debug.Log("AdMob initialised");
            LoadInterstitial();
            LoadRewarded();
        });
#else
        Debug.Log("Google Mobile Ads SDK not installed. Ads are disabled in this build.");
#endif
    }

#if GOOGLE_MOBILE_ADS
    void LoadInterstitial()
    {
        InterstitialAd.Load(INTERSTITIAL_ID, new AdRequest(), (ad, error) =>
        {
            if (error != null)
            {
                Debug.LogWarning($"Interstitial load failed: {error}");
                return;
            }

            _interstitial = ad;
            _interstitial.OnAdFullScreenContentClosed += () =>
            {
                _onInterstitialClosed?.Invoke();
                LoadInterstitial();
            };
        });
    }
#endif

    public void ShowInterstitial(Action onClosed = null)
    {
        _onInterstitialClosed = onClosed;

#if GOOGLE_MOBILE_ADS
        if (_interstitial != null && _interstitial.CanShowAd())
        {
            _interstitial.Show();
            return;
        }
#endif

        onClosed?.Invoke();
    }

#if GOOGLE_MOBILE_ADS
    void LoadRewarded()
    {
        RewardedAd.Load(REWARDED_ID, new AdRequest(), (ad, error) =>
        {
            if (error != null)
            {
                Debug.LogWarning($"Rewarded load failed: {error}");
                return;
            }

            _rewarded = ad;
            _rewarded.OnAdFullScreenContentClosed += () => LoadRewarded();
        });
    }
#endif

    public void ShowRewarded(Action onRewarded, Action onFailed = null)
    {
#if GOOGLE_MOBILE_ADS
        if (_rewarded != null && _rewarded.CanShowAd())
        {
            _onRewardGranted = onRewarded;
            _rewarded.Show(_ => _onRewardGranted?.Invoke());
            return;
        }
#endif

        onFailed?.Invoke();
    }

    public void ShowBanner()
    {
#if GOOGLE_MOBILE_ADS
        if (_banner != null) return;

        _banner = new BannerView(BANNER_ID, AdSize.Banner, AdPosition.Bottom);
        _banner.LoadAd(new AdRequest());
#endif
    }

    public void HideBanner()
    {
#if GOOGLE_MOBILE_ADS
        _banner?.Hide();
        _banner?.Destroy();
        _banner = null;
#endif
    }
}
