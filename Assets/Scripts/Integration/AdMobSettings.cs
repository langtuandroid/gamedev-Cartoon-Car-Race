using UnityEngine;

namespace Integration
{
    [CreateAssetMenu(fileName = "Settings", menuName = "AdMob/Settings", order = 1)]
    public class AdMobSettings : ScriptableObject
    {
#if UNITY_ANDROID
        [Header("App ID")]
        [SerializeField] private string _appAndroidId;
        [Header("Ads ID")]
        [SerializeField] private string _bannerId;
        [SerializeField] private string _interstitialId;
        [SerializeField] private string _rewardedId;
        
        [Header("Test Ads ID")]
        [SerializeField] private string _bannerTestId;
        [SerializeField] private string _interstitialTestId;
        [SerializeField] private string _rewardedTestId;

#elif UNITY_IOS
        [Header("App ID")]
        [SerializeField] private string _appIOSId;
        [Header("Ads ID")]
        [SerializeField] private string _bannerId;
        [SerializeField] private string _interstitialId;
        [SerializeField] private string _rewardedId;

        [Header("Test Ads ID")]
        [SerializeField] private string _bannerTestId;
        [SerializeField] private string _interstitialTestId;
        [SerializeField] private string _rewardedTestId;

#endif
        
        public string BannerID => _bannerId;
        public string InterstitialID => _interstitialId;
        public string RewardedID => _rewardedId;


        public string BannerTestID => _bannerTestId;
        public string InterstitialTestID => _interstitialTestId;
        public string RewardedTestID => _rewardedTestId;

    }
}
