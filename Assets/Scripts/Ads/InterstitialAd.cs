using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InterstitialAd : MonoBehaviour, IUnityAdsLoadListener, IUnityAdsShowListener
{
    [SerializeField] string _androidAdUnitId = "BP_Interstitial_Android";
    string _adUnitId;
    public event Action OnInterstitialAdReady;
    public bool isReady = false;

    // Patiesi, kamēr pilnekrāna reklāma tiek rādīta - baneri šajā laikā nerāda
    public static bool IsShowing { get; private set; }
    [SerializeField] Button _interstitialAdButton;

    [Tooltip("Tikai šajā ainā pēc reklāmas ieslēdzas lēnais laiks (30 s). Citās ainās laiks paliek normāls.")]
    [SerializeField] string slowMotionSceneName = "CityScene";

    void Awake()
    {
        _adUnitId = _androidAdUnitId;
    }


    private void Update()
    {
        // Pogas var nebūt (piem., MainMenu vai Hanoi ainā) - tad nekas nav jādara
        if (_interstitialAdButton != null && AdManager.Instance != null && AdManager.Instance.interstitialAd != null)
        {
            _interstitialAdButton.interactable = AdManager.Instance.interstitialAd.isReady;
        }
    }

    public void OnInterstitialAdButtonClicked()
    {
        Debug.Log("Interstitial Ad Button Clicked");
        ShowInterstitial();
    }

    public void ShowInterstitial()
    {
        if (AdManager.Instance.interstitialAd != null && AdManager.Instance.interstitialAd.isReady)
        {
            Debug.Log("Showing interstitial ad manually!");
            ShowAd();
        }
        else
        {
            Debug.LogWarning("Interstitial ad is not ready yet, loading again!");
            LoadAd();
        }
    }

    public void ShowAd()
    {
        if (isReady)
        {
            // Banneri paslēpjam VIENMĒR (nepaļaujoties uz karodziņu), lai tas nekad nebūtu virs pilnekrāna reklāmas
            if (AdManager.Instance != null && AdManager.Instance.bannerAd != null)
                AdManager.Instance.bannerAd.HideBannerAd();
            else
                Advertisement.Banner.Hide();

            IsShowing = true;
            Advertisement.Show(_adUnitId, this);
            isReady = false;
        }
        else
        {
            Debug.LogWarning("Interstitial ad is not ready");
        }
    }

    public void LoadAd()
    {
        if (!Advertisement.isInitialized)
        {
            Debug.LogWarning("Tried to load interstitial ad before Unity Ads was initialized!");
            return;
        }

        Debug.Log("Loading interstitial ad...");
        Advertisement.Load(_adUnitId, this);
    }

    public void OnUnityAdsAdLoaded(string placementId)
    {
        Debug.Log("Interstitial ad loaded succesfully!");
        isReady = true;   // vispirms atzīmējam, ka reklāma gatava (neatkarīgi no pogas)
        if (_interstitialAdButton != null)
            _interstitialAdButton.interactable = true;
        OnInterstitialAdReady?.Invoke();
    }

    public void OnUnityAdsFailedToLoad(string placementId, UnityAdsLoadError error, string message)
    {
        Debug.Log($"Failed to load interstitial ad: {error.ToString()} -  {message}");
        LoadAd();
    }

    public void OnUnityAdsShowClick(string placementId)
    {
        Debug.Log("Interstitial ad clicked!");
    }

    // Pēc reklāmas atjauno banneri tajās ainās, kur tam jābūt redzamam (piem., MainMenu)
    private void RestoreBanner()
    {
        IsShowing = false;

        if (AdManager.Instance != null && AdManager.Instance.bannerAd != null)
            AdManager.Instance.bannerAd.ApplySceneRule(SceneManager.GetActiveScene().name);
    }

    public void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState showCompletionState)
    {
        RestoreBanner();
        if (showCompletionState == UnityAdsShowCompletionState.COMPLETED)
        {
            Debug.Log("Interstitial ad completed sucesfully!");
            if (SceneManager.GetActiveScene().name == slowMotionSceneName)
                StartCoroutine(SlowDownTimeTemporarily(30f));
            else
                Time.timeScale = 1f;
            LoadAd();
        }
        else
        {
            Debug.LogWarning($"Interstitial ad was not completed: {showCompletionState.ToString()}");
            Time.timeScale = 1f;
            LoadAd();
        }
    }

    private IEnumerator SlowDownTimeTemporarily(float duration)
    {
        Time.timeScale = 0.3f;
        Debug.Log("Time slowed down for " + duration + " seconds.");
        yield return new WaitForSecondsRealtime(duration);
        Time.timeScale = 1f;
        Debug.Log("Time restored to normal.");
    }

    public void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message)
    {
        Debug.LogError($"Failed to show interstitial ad: {error.ToString()} - {message}");
        RestoreBanner();
        LoadAd();
    }

    public void OnUnityAdsShowStart(string placementId)
    {
        Debug.Log("Interstitial ad started showing!");
        Time.timeScale = 0f;
    }

    public void SetButton(Button button)
    {
        if (button == null)
            return;

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(OnInterstitialAdButtonClicked);
        _interstitialAdButton = button;
        _interstitialAdButton.interactable = false;
    }




}