using Script.Ad;
using UnityEngine;
using UnityEngine.Advertisements;
 
public class AdsInitializer : MonoBehaviour, IUnityAdsInitializationListener
{
    public AdController adController;
    
    public string _androidGameId;
    public string _iOSGameId;
    public bool _testMode = true;
    private string _gameId;
 
    public void InitializeAds()
    {
        _gameId = (Application.platform == RuntimePlatform.IPhonePlayer)
            ? _iOSGameId
            : _androidGameId;
        Advertisement.Initialize(_gameId, _testMode, this);
    }
 
    public void OnInitializationComplete()
    {
        Debug.Log("Unity Ads initialization complete.");
        adController.LoadAllAd();
    }
 
    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
    {
        Debug.Log($"Unity Ads Initialization Failed: {error.ToString()} - {message}");
    }
}