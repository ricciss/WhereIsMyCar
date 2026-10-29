using System;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BannerAd : MonoBehaviour
{
    [SerializeField] string _AndroidAdUnitId = "BP_Banner_Android";
    string _adUnitId;

    [SerializeField] Button _bannerButton;
    public bool isBannerVisible = false;

    [Header("Automātiska banera rādīšana")]
    [Tooltip("Ainas, kurās baneris parādās pats, bez pogas spiešanas. Citās ainās tas tiek paslēpts.")]
    [SerializeField] string[] autoShowScenes = { "MainMenu" };
    private bool isLoaded = false;

    // Citi skripti (piem. BannerUIShifter) var klausīties, kad baneris parādās / pazūd
    public static bool IsShown { get; private set; }
    public static event Action<bool> OnBannerVisibilityChanged;

    [SerializeField] BannerPosition bannerPosition = BannerPosition.BOTTOM_CENTER;
    void Awake()
    {
        _adUnitId = _AndroidAdUnitId;
        Advertisement.Banner.SetPosition(bannerPosition);
    }

    public void LoadBanner()
    {
        if (!Advertisement.isInitialized)
        {
            Debug.Log("Tried to load banner ad before Unity ads was initialized");
            return;
        }

        BannerLoadOptions bannerOptions = new BannerLoadOptions
        {
            loadCallback = OnBannerLoaded,
            errorCallback = OnBannerError
        };

        Advertisement.Banner.Load(_adUnitId, bannerOptions);
    }

    void OnBannerLoaded()
    {
        Debug.Log("Banner ad Loaded!");
        isLoaded = true;
        if (_bannerButton != null)
            _bannerButton.interactable = true;

        // Ja šajā ainā baneris jārāda automātiski - parāda uzreiz pēc ielādes
        ApplySceneRule(SceneManager.GetActiveScene().name);
    }

    // Parāda vai paslēpj banneri pēc tā, vai aina ir sarakstā autoShowScenes
    public void ApplySceneRule(string sceneName)
    {
        bool shouldShow = System.Array.IndexOf(autoShowScenes, sceneName) >= 0;

        if (shouldShow)
        {
            // Kamēr rāda pilnekrāna reklāmu, baneri nerāda - to atjaunos pēc reklāmas beigām
            if (InterstitialAd.IsShowing)
                return;

            if (isLoaded && !isBannerVisible)
                ShowBannerAd();
        }
        else if (isBannerVisible)
        {
            HideBannerAd();
        }
    }

    void OnBannerError(string message)
    {
        Debug.Log($"Banner error: {message}");
        LoadBanner();
    }

    public void ShowBannerAd()
    {
        if (isBannerVisible)
            HideBannerAd();

        else
        {
            BannerOptions bannerOptions = new BannerOptions
            {
                clickCallback = OnBannerClicked,
                hideCallback = OnBannerHidden,
                showCallback = OnBannerShown
            };
            Advertisement.Banner.Show(_adUnitId, bannerOptions);
        }
    }

    void OnBannerClicked()
    {
        Debug.Log("User clicked on banner ad: " + _adUnitId);
    }

    void OnBannerHidden()
    {
        Debug.Log("Banner ad is hidden! " + _adUnitId);
        isBannerVisible = false;
        IsShown = false;
        OnBannerVisibilityChanged?.Invoke(false);
    }

    void OnBannerShown()
    {
        Debug.Log("Banner ad is visible! " + _adUnitId);
        isBannerVisible = true;
        IsShown = true;
        OnBannerVisibilityChanged?.Invoke(true);
    }



    public void HideBannerAd()
    {
        Advertisement.Banner.Hide();

        // Karodziņu atjaunojam uzreiz, nevis gaidām hideCallback (tas var nenonākt)
        if (isBannerVisible)
        {
            isBannerVisible = false;
            IsShown = false;
            OnBannerVisibilityChanged?.Invoke(false);
        }
    }

    public void SetButton(Button button)
    {
        if (button == null)
            return;

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(ShowBannerAd);
        _bannerButton = button;
        _bannerButton.interactable = false;
    }
}