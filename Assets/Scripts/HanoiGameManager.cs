using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

/// <summary>
/// Hanojas torņa spēles vadība. Uzliec uz tukša GameObject ainā.
/// Visi diski un stieņi jābūt UZ VIENA UN TĀ PAŠA vecāka (piem., "GamePanel").
/// </summary>
public class HanoiGameManager : MonoBehaviour
{
    [Header("Stieņi (3 gab., no kreisās uz labo)")]
    public HanoiPeg[] pegs;

    [Header("Diski - sakārtoti NO MAZĀKĀ UZ LIELO (0block, 1block, 2block ...)")]
    public HanoiDisc[] discs;

    [Tooltip("Cik disku izmantot (3-6). Pārējie tiek paslēpti.")]
    [Range(3, 6)] public int discCount = 4;

    [Header("Izkārtojums")]
    [Tooltip("Apakšējā diska centra Y koordināta (localPosition) uz stieņa")]
    public float baseY = -150f;
    [Tooltip("Attālums starp diskiem pa Y (≈ diska augstums)")]
    public float discHeight = 55f;
    public bool autoSizeDiscs = true;
    public float minDiscWidth = 120f;
    public float discWidthStep = 40f;

    [Header("Laika sliekšņi zvaigznēm (sekundes)")]
    [Tooltip("Ja spēle pabeigta līdz šim laikam - 3 zvaigznes")]
    public float threeStarTime = 60f;
    [Tooltip("Ja spēle pabeigta līdz šim laikam - 2 zvaigznes, citādi 1 zvaigzne")]
    public float twoStarTime = 90f;

    [Header("UI - spēles laikā")]
    public TMP_Text timerText;
    [Tooltip("Objekti, kas uzvaras brīdī tiek paslēpti (spēles taimeris, Restart un Return pogas)")]
    public GameObject[] hideOnVictory;

    [Header("UI - Uzvaras panelis")]
    public GameObject victoryPanel;
    [Tooltip("Paša uzvaras paneļa Image komponente (tās attēls tiek nomainīts pēc zvaigžņu skaita)")]
    public Image victoryImage;
    public TMP_Text victoryTimeText;

    [Header("Rewarded reklāma")]
    [Tooltip("Cik sekundes tiek noņemtas no laika, kad rewarded reklāma noskatīta līdz galam")]
    public float rewardedTimeBonus = 30f;

    [Header("Uzvaras paneļa attēli")]
    public Sprite victory1Star;
    public Sprite victory2Stars;
    public Sprite victory3Stars;

    [Header("Skaņas (pēc izvēles)")]
    public AudioSource audioSource;
    public AudioClip pickupSound;
    public AudioClip placeSound;
    public AudioClip wrongSound;
    public AudioClip winSound;

    [Header("Ainas")]
    [Tooltip("Atstāj tukšu, lai pārlādētu pašreizējo ainu")]
    public string sceneNameToReload = "";
    public string mainMenuSceneName = "MainMenu";

    public bool IsFinished { get; private set; }

    private float elapsedTime = 0f;
    private bool timerRunning = false;

    void Awake()
    {
        Time.timeScale = 1f;
        IsFinished = false;

        if (victoryPanel != null) victoryPanel.SetActive(false);
        if (audioSource == null) audioSource = GetComponent<AudioSource>();

        foreach (HanoiPeg peg in pegs)
        {
            peg.manager = this;
            peg.stack.Clear();
        }

        SetupDiscs();
    }

    void SetupDiscs()
    {
        discCount = Mathf.Clamp(discCount, 1, discs.Length);

        for (int i = 0; i < discs.Length; i++)
        {
            HanoiDisc d = discs[i];
            bool used = i < discCount;
            d.gameObject.SetActive(used);
            if (!used) continue;

            d.rect = d.GetComponent<RectTransform>();
            d.manager = this;
            d.size = i;

            if (autoSizeDiscs)
                d.rect.sizeDelta = new Vector2(minDiscWidth + i * discWidthStep, discHeight);
        }

        // Visi diski sākumā uz 1. stieņa: lielākais apakšā, mazākais augšā
        for (int i = discCount - 1; i >= 0; i--)
        {
            pegs[0].stack.Add(discs[i]);
            discs[i].currentPeg = pegs[0];
            SnapToPeg(discs[i]);
        }
    }

    void OnEnable()
    {
        RewardedAds.OnRewardEarned += HandleRewardEarned;
    }

    void OnDisable()
    {
        RewardedAds.OnRewardEarned -= HandleRewardEarned;
    }

    // Rewarded reklāma noskatīta līdz galam - noņem sekundes no laika
    void HandleRewardEarned()
    {
        if (IsFinished) return;

        elapsedTime = Mathf.Max(0f, elapsedTime - rewardedTimeBonus);
        if (timerText != null) timerText.text = FormatTime(elapsedTime);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ReturnToMainMenu();
            return;
        }

        if (!timerRunning) return;

        elapsedTime += Time.deltaTime;
        if (timerText != null) timerText.text = FormatTime(elapsedTime);
    }

    // ---------------- Noteikumi ----------------

    /// <summary>Vilkt drīkst tikai augšējo disku uz stieņa.</summary>
    public bool CanDrag(HanoiDisc disc)
    {
        if (IsFinished || disc.currentPeg == null) return false;
        return disc.currentPeg.Top == disc;
    }

    /// <summary>Mēģina pārvietot disku uz stieni. Lielāku disku uz mazāka likt nedrīkst.</summary>
    public void TryMove(HanoiDisc disc, HanoiPeg target)
    {
        if (IsFinished || disc == null || target == null) return;

        HanoiPeg source = disc.currentPeg;

        // Nomests atpakaļ uz tā paša stieņa - nav gājiens
        if (source == target) return;

        HanoiDisc topOfTarget = target.Top;
        bool legal = (topOfTarget == null) || (disc.size < topOfTarget.size);

        if (!legal)
        {
            Play(wrongSound);   // disks atlecīs atpakaļ OnEndDrag laikā
            return;
        }

        source.stack.Remove(disc);
        target.stack.Add(disc);
        disc.currentPeg = target;
        SnapToPeg(disc);

        if (!timerRunning) timerRunning = true;   // taimeris sākas ar pirmo gājienu
        Play(placeSound);

        CheckForWin();
    }

    void CheckForWin()
    {
        // Uzvara: visi diski uz pēdējā stieņa
        HanoiPeg last = pegs[pegs.Length - 1];
        if (last.stack.Count == discCount)
            Win();
    }

    // ---------------- Pozicionēšana ----------------

    /// <summary>Novieto disku pareizā vietā uz sava stieņa.</summary>
    public void SnapToPeg(HanoiDisc disc)
    {
        if (disc == null || disc.currentPeg == null) return;

        int index = disc.currentPeg.stack.IndexOf(disc);
        Vector3 pegPos = disc.currentPeg.transform.localPosition;

        disc.rect.localPosition = new Vector3(pegPos.x, baseY + index * discHeight, disc.rect.localPosition.z);
    }

    // ---------------- Uzvara ----------------

    void Win()
    {
        timerRunning = false;
        IsFinished = true;

        int stars = ComputeStarsFromTime(elapsedTime);

        if (victoryTimeText != null) victoryTimeText.text = FormatTime(elapsedTime);

        // Paslēpj spēles laika taimeri un pogas, uzvaras panelī paliek tikai paneļa paša elementi
        foreach (GameObject obj in hideOnVictory)
        {
            if (obj != null) obj.SetActive(false);
        }

        ShowVictorySprite(stars);
        ShowPanelOnTop(victoryPanel);
        Play(winSound);
    }

    int ComputeStarsFromTime(float time)
    {
        if (time <= threeStarTime) return 3;
        if (time <= twoStarTime) return 2;
        return 1;   // pabeigts - vienmēr vismaz 1 zvaigzne
    }

    // Nomaina visa uzvaras paneļa attēlu uz tādu, kur ir 1, 2 vai 3 zvaigznes
    void ShowVictorySprite(int stars)
    {
        Sprite chosen = victory1Star;
        if (stars >= 3) chosen = victory3Stars;
        else if (stars == 2) chosen = victory2Stars;

        if (victoryImage != null && chosen != null)
            victoryImage.sprite = chosen;
    }

    void ShowPanelOnTop(GameObject panel)
    {
        if (panel == null) return;

        panel.SetActive(true);
        panel.transform.SetAsLastSibling();

        Canvas canvas = panel.GetComponent<Canvas>();
        if (canvas == null) canvas = panel.AddComponent<Canvas>();
        canvas.overrideSorting = true;
        canvas.sortingOrder = 100;

        if (panel.GetComponent<GraphicRaycaster>() == null)
            panel.AddComponent<GraphicRaycaster>();
    }

    // ---------------- UI palīgfunkcijas ----------------

    string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void PlayPickup() { Play(pickupSound); }

    void Play(AudioClip clip)
    {
        if (audioSource != null && clip != null)
            audioSource.PlayOneShot(clip);
    }

    // ---------------- Pogām (OnClick) ----------------

    public void RestartGame()
    {
        Time.timeScale = 1f;

        // Restarts PĒC uzvaras - reklāmu nerāda. Restarts spēles laikā - reklāma parādās kā parasti.
        if (IsFinished && AdManager.Instance != null)
            AdManager.Instance.SkipNextSceneInterstitial();

        if (string.IsNullOrEmpty(sceneNameToReload))
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        else
            SceneManager.LoadScene(sceneNameToReload);
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }
}