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
    [SerializeField] string[] autoShowScenes = { "MainMenu", "HanoiScene" };

    [System.Serializable]
    public class ScenePosition
    {
        public string sceneName;
        public BannerPosition position;
    }

    [Header("Banera pozīcija pa ainām")]
    [Tooltip("Ainām, kas nav sarakstā, tiek izmantota pozīcija 'Banner Position' zemāk")]
    [SerializeField] ScenePosition[] scenePositions =
    {
        new ScenePosition { sceneName = "MainMenu",   position = BannerPosition.BOTTOM_CENTER },
        new ScenePosition { sceneName = "HanoiScene", position = BannerPosition.TOP_CENTER }
    };
    [SerializeField] BannerPosition bannerPosition = BannerPosition.BOTTOM_CENTER;

    // Citi skripti (piem. BannerUIShifter) var klausīties, kad baneris parādās / pazūd
    public static bool IsShown { get; private set; }
    public static event Action<bool> OnBannerVisibilityChanged;

    private bool isLoaded = false;
    private bool isLoading = false;
    public bool IsLoaded { get { return isLoaded; } }

    void Awake()
    {
        _adUnitId = _AndroidAdUnitId;
        Advertisement.Banner.SetPosition(bannerPosition);
    }

    // ---------------- Pozīcija ----------------

    BannerPosition GetPositionForActiveScene()
    {
        string sceneName = SceneManager.GetActiveScene().name;

        if (scenePositions != null)
        {
            foreach (ScenePosition sp in scenePositions)
            {
                if (sp.sceneName == sceneName)
                    return sp.position;
            }
        }
        return bannerPosition;
    }

    // ---------------- Ielāde ----------------

    public void LoadBanner()
    {
        if (!Advertisement.isInitialized)
        {
            Debug.Log("Tried to load banner ad before Unity ads was initialized");
            return;
        }

        if (isLoading) return;
        isLoading = true;

        // Pozīcija tiek uzstādīta PIRMS ielādes
        Advertisement.Banner.SetPosition(GetPositionForActiveScene());

        BannerLoadOptions bannerOptions = new BannerLoadOptions
        {
            loadCallback = OnBannerLoaded,
            errorCallback = OnBannerError
        };

        Advertisement.Banner.Load(_adUnitId, bannerOptions);
    }

    // Katrā jaunā ainā: iznīcina veco banneri un ielādē jaunu ar šīs ainas pozīciju
    public void ReloadForScene()
    {
        CancelInvoke(nameof(LoadBanner));
        Advertisement.Banner.Hide(true);   // true = pilnībā iznīcina veco banneri
        SetVisible(false);

        isLoaded = false;
        isLoading = false;
        if (_bannerButton != null)
            _bannerButton.interactable = false;

        LoadBanner();
    }

    void OnBannerLoaded()
    {
        Debug.Log("Banner ad Loaded!");
        isLoading = false;
        isLoaded = true;

        if (_bannerButton != null)
            _bannerButton.interactable = true;

        // Ja šajā ainā baneris jārāda automātiski - parāda uzreiz pēc ielādes
        ApplySceneRule(SceneManager.GetActiveScene().name);
    }

    void OnBannerError(string message)
    {
        Debug.Log($"Banner error: {message}");
        isLoading = false;
        CancelInvoke(nameof(LoadBanner));
        Invoke(nameof(LoadBanner), 5f);   // mēģina vēlreiz pēc 5 sekundēm
    }

    // ---------------- Rādīšana / slēpšana ----------------

    // Parāda vai paslēpj banneri pēc tā, vai aina ir sarakstā autoShowScenes
    public void ApplySceneRule(string sceneName)
    {
        bool shouldShow = Array.IndexOf(autoShowScenes, sceneName) >= 0;

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

    // Stāvokli maina TIKAI mūsu pašu izsaukumi, nevis vēlu atnākošie Unity callbacki
    void SetVisible(bool visible)
    {
        if (isBannerVisible == visible) return;

        isBannerVisible = visible;
        IsShown = visible;
        OnBannerVisibilityChanged?.Invoke(visible);
    }

    public void ShowBannerAd()
    {
        if (isBannerVisible)
        {
            HideBannerAd();
            return;
        }

        if (!isLoaded)
        {
            Debug.Log("Banner not loaded yet, cannot show.");
            return;
        }

        Advertisement.Banner.SetPosition(GetPositionForActiveScene());

        BannerOptions bannerOptions = new BannerOptions
        {
            clickCallback = OnBannerClicked,
            hideCallback = OnBannerHidden,
            showCallback = OnBannerShown
        };
        Advertisement.Banner.Show(_adUnitId, bannerOptions);
        SetVisible(true);
    }

    public void HideBannerAd()
    {
        Advertisement.Banner.Hide();
        SetVisible(false);
    }

    void OnBannerClicked()
    {
        Debug.Log("User clicked on banner ad: " + _adUnitId);
    }

    void OnBannerHidden()
    {
        Debug.Log("Banner ad is hidden! " + _adUnitId);
    }

    void OnBannerShown()
    {
        Debug.Log("Banner ad is visible! " + _adUnitId);
    }

    // ---------------- Poga (CityScene) ----------------

    public void SetButton(Button button)
    {
        if (button == null)
            return;

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(ShowBannerAd);
        _bannerButton = button;
        _bannerButton.interactable = isLoaded;
    }
}