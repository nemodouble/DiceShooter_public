using System;
using Unity.Services.Core;
using Unity.Services.Core.Environments;
using UnityEngine;

namespace Script.Ad
{
    public class AdController : MonoBehaviour
    {
        #region singleton

        public static AdController Instance
        {
            get
            {
                if (instance != null)
                    return instance;

                instance = FindObjectOfType<AdController>();

                if (instance != null)
                    return instance;

                Create ();

                return instance;
            }
        }
        public static AdController instance;

        public static AdController Create ()
        {
            GameObject sceneControllerGameObject = new GameObject("AdController");
            instance = sceneControllerGameObject.AddComponent<AdController>();

            return instance;
        }

        #endregion
        
        
        [SerializeField] string _androidGameId;
        [SerializeField] string _iOSGameId;
        [SerializeField] bool _testMode = true;
        
        private AdsInitializer m_AdsInitializer;
        private InterstitialAd m_InterstitialAd;
        private BannerAd m_BannerAd;
        private RewardedAdsButton m_RewardedAdsButton;

        private bool isPurchased = false;
        
        private void Awake()
        {
            if (Instance != this)
            {
                Destroy(gameObject);
            }
            DontDestroyOnLoad(this);

            isPurchased = PlayerPrefs.GetInt("IsPurchased", 0) == 0;
            
            
            
            m_AdsInitializer = gameObject.AddComponent<AdsInitializer>();
            m_AdsInitializer._androidGameId = _androidGameId;
            m_AdsInitializer._iOSGameId = _iOSGameId;
            m_AdsInitializer._testMode = _testMode;
            m_AdsInitializer.adController = this;
            m_AdsInitializer.InitializeAds();
        }

        public void LoadAllAd()
        {
            m_BannerAd = gameObject.AddComponent<BannerAd>();
            m_BannerAd.Start();
            m_BannerAd.LoadBanner();
            m_InterstitialAd = gameObject.AddComponent<InterstitialAd>();
            m_InterstitialAd.LoadAd();
            m_RewardedAdsButton = gameObject.AddComponent<RewardedAdsButton>();
            m_RewardedAdsButton.LoadAd();
        }
        
        public void ShowInterstitialAd()
        {
            if(!isPurchased)
                m_InterstitialAd.ShowAd();
        }

        public void ShowRewardAd()
        {
            m_RewardedAdsButton.ShowAd();
        }

        public void ShowBannerAd()
        {
            if(!isPurchased)
                m_BannerAd.ShowBannerAd();
        }
        
        public void HideBannerAd()
        {
            if(!isPurchased)
                m_BannerAd.HideBannerAd();
        }

        public void RemoveAd()
        {
            Debug.Log("Remove Ad Success");
            PlayerPrefs.SetInt("IsPurchased", 1);
            isPurchased = true;
        }
    }
}