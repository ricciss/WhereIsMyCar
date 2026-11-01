using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AdManager : MonoBehaviour
{
    public AdsInitializer adsInitializer;
    public InterstitialAd interstitialAd;
    [SerializeField] bool turnOffInterstitialAds = false;
    private bool firstAdShown = false;

    [Header("Interstitial reklāma pie ainu maiņas")]
    [SerializeField] bool showInterstitialOnSceneChange = true;
    [Tooltip("Minimālais laiks (sekundēs) starp divām interstitial reklāmām. 0 = katrā ainu maiņā")]
    [SerializeField] float minSecondsBetweenInterstitials = 0f;
    private float lastInterstitialTime = -9999f;

    public RewardedAds rewardedAds;
    [SerializeField] bool turnOffRewardedAds = false;

    public BannerAd bannerAd;
    [SerializeField] bool turnOffBannerAds = false;

    public static AdManager Instance { get; private set; }

    private bool firstSceneLoaded = false;
    private bool adsInitialized = false;
    private bool interstitialSubscribed = false;
    private bool skipNextSceneInterstitial = false;

    // Izsauc pirms ainas ielādes, ja nākamajā ainas maiņā reklāmu rādīt NEVAJAG (piem., restarts pēc uzvaras)
    public void SkipNextSceneInterstitial()
    {
        skipNextSceneInterstitial = true;
    }

    private void Awake()
    {
        if(adsInitializer == null)
        {
            adsInitializer = FindFirstObjectByType<AdsInitializer>();
        }

        if(Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        adsInitializer.OnAdsInitialized += HandleAdsInitialized;
    }

    private void HandleAdsInitialized()
    {
        adsInitialized = true;

        if (!turnOffInterstitialAds)
        {
            if (!interstitialSubscribed)
            {
                interstitialAd.OnInterstitialAdReady += HandleInterstitialReady;
                interstitialSubscribed = true;
            }
            interstitialAd.LoadAd();
        }

        if (!turnOffRewardedAds)
        {
            rewardedAds.LoadAd();
        }

        if (!turnOffBannerAds)
        {
            bannerAd.LoadBanner();
        }
    }

    private void HandleInterstitialReady()
    {
        if (!firstAdShown)
        {
            Debug.Log("Showing interstitial ad for the first time!");
            lastInterstitialTime = Time.realtimeSinceStartup;
            interstitialAd.ShowAd();
            firstAdShown = true;
        }
        else
        {
            Debug.Log("Interstitial ad is ready to be shown again!");
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // Atrod pogu pēc tag. Ja ainā tādas nav (MainMenu, Hanoi) - atgriež null, nevis met kļūdu
    private Button FindButtonWithTag(string tag)
    {
        GameObject obj = GameObject.FindGameObjectWithTag(tag);
        return obj != null ? obj.GetComponent<Button>() : null;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (interstitialAd == null)
            interstitialAd = FindFirstObjectByType<InterstitialAd>();
        if (rewardedAds == null)
            rewardedAds = FindFirstObjectByType<RewardedAds>();

        // Reklāmu objekti dzīvo līdzi visām ainām, bet FlyingObjectManager ir katrā ainā savs
        if (rewardedAds != null)
            rewardedAds.flyingObjectManager = FindFirstObjectByType<FlyingObjectManager>();

        Button interstitialButton = FindButtonWithTag("InterstitialAdButton");
        if (interstitialAd != null && interstitialButton != null)
            interstitialAd.SetButton(interstitialButton);

        Button rewardedAdButton = FindButtonWithTag("RewardedAdButton");
        if (rewardedAds != null && rewardedAdButton != null)
            rewardedAds.SetButton(rewardedAdButton);

        Button bannerAdButton = FindButtonWithTag("BannerAdButton");
        if (bannerAd != null && bannerAdButton != null)
            bannerAd.SetButton(bannerAdButton);

        if (!firstSceneLoaded)
        {
            firstSceneLoaded = true;
            Debug.Log("First scene Loaded!");
            return;
        }

        Debug.Log("Scene changed: " + scene.name);

        // Vienreizējs izlaidums: nākamajā ainas ielādē reklāmu nerāda
        bool skipInterstitialNow = skipNextSceneInterstitial;
        skipNextSceneInterstitial = false;

        if (!adsInitialized)
            return;   // reklāmas vēl nav inicializētas - pirmo reklāmu parādīs HandleInterstitialReady

        // 1) Interstitial reklāma pie ainas maiņas (tikai ja jau ir ielādēta)
        bool shownNow = false;
        if (!turnOffInterstitialAds && showInterstitialOnSceneChange && !skipInterstitialNow)
            shownNow = TryShowInterstitial();
        else if (skipInterstitialNow)
            Debug.Log("Interstitial skipped for this scene change (restart after victory).");

        // Baneris: katrā jaunā ainā tiek iznīcināts un ielādēts no jauna ar šīs ainas pozīciju.
        // Pēc ielādes tas parādās pats (MainMenu, Hanoi) vai gaida pogu (CityScene).
        if (!turnOffBannerAds && bannerAd != null)
            bannerAd.ReloadForScene();

        // 2) Ielādē reklāmas nākamajai reizei / atjauno pogas jaunajā ainā
        if (!turnOffInterstitialAds && !shownNow && interstitialAd != null && !interstitialAd.isReady)
            interstitialAd.LoadAd();

        if (!turnOffRewardedAds && rewardedAds != null)
            rewardedAds.LoadAd();

    }

    private bool TryShowInterstitial()
    {
        if (interstitialAd == null || !interstitialAd.isReady)
        {
            Debug.Log("Interstitial not ready on scene change, skipping.");
            return false;
        }

        if (Time.realtimeSinceStartup - lastInterstitialTime < minSecondsBetweenInterstitials)
        {
            Debug.Log("Interstitial skipped (too soon after the previous one).");
            return false;
        }

        lastInterstitialTime = Time.realtimeSinceStartup;
        firstAdShown = true;
        interstitialAd.ShowAd();
        return true;
    }
}