using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManagerScript : MonoBehaviour
{
    [Header("Spēles iestatījumi")]
    [Tooltip("Kopējais transportlīdzekļu skaits, kas jānovieto pareizajās vietās")]
    public int totalVehicles = 12;

    [Tooltip("Dzīvības - cik transportlīdzekļus drīkst uzspridzināt pirms Game Over")]
    public int startingLives = 3;

    [Header("Laika sliekšņi zvaigznēm (sekundes)")]
    [Tooltip("Ja spēle pabeigta līdz šim laikam - 3 zvaigznes (ja nav zaudētu mašīnu)")]
    public float threeStarTime = 60f;
    public float twoStarTime = 90f;
    public float oneStarTime = 120f;

    [Header("UI - Laiks un dzīvības spēles laikā")]
    public TMP_Text timerText;
    public TMP_Text livesText; // pēc izvēles - vari atstāt tukšu, ja izmanto sirsniņas

    [Tooltip("3 Image komponentes (sirsniņas), kas rāda atlikušās dzīvības - kreisā uz labo")]
    public Image[] livesImages;

    [Header("UI - Uzvaras panelis")]
    public GameObject victoryPanel;
    public TMP_Text victoryTimeText;
    [Tooltip("3 Image komponenti, kas attēlo zvaigznes")]
    public Image[] starImages;
    public Sprite starFilledSprite;
    public Sprite starEmptySprite;

    [Header("UI - Game Over panelis")]
    public GameObject gameOverPanel;

    [Header("Restarts")]
    [Tooltip("Atstāj tukšu, lai pārlādētu pašreizējo ainu")]
    public string sceneNameToReload = "";

    [Header("Atgriešanās uz sākuma ainu")]
    [Tooltip("Sākuma ainas nosaukums (jābūt Build Settings sarakstā)")]
    public string mainMenuSceneName = "MainMenu";

    // true, kad spēle beigusies (uzvara vai zaudējums) - citi skripti pēc tā bloķē vilkšanu
    public static bool isGameOver = false;

    private float elapsedTime = 0f;
    private bool timerRunning = true;
    private int correctlyPlaced = 0;
    private int vehiclesLost = 0;
    private int livesRemaining;

    // lai vienu un to pašu vietu nevarētu ieskaitīt divreiz
    private readonly HashSet<Transform> alreadyCountedPlaces = new HashSet<Transform>();

    void Awake()
    {
        // Sākam ar "atsaldētu" spēli (pēc restarta timeScale varētu būt palicis 0)
        isGameOver = false;
        Time.timeScale = 1f;

        livesRemaining = startingLives;

        if (victoryPanel != null) victoryPanel.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);

        UpdateLivesUI();
    }

    void Update()
    {
        // Esc atgriež uz sākuma ainu jebkurā brīdī (arī spēles laikā)
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ReturnToMainMenu();
            return;
        }

        if (!timerRunning) return;

        elapsedTime += Time.deltaTime;

        if (timerText != null)
        {
            timerText.text = FormatTime(elapsedTime);
        }
    }

    /// <summary>
    /// Sauc DropPlaceScript, kad transportlīdzeklis novietots PAREIZI.
    /// </summary>
    public void RegisterCorrectPlacement(Transform dropPlace)
    {
        if (!timerRunning) return;
        if (alreadyCountedPlaces.Contains(dropPlace)) return;

        alreadyCountedPlaces.Add(dropPlace);
        correctlyPlaced++;

        CheckForWin();
    }

    // Uzvara iestājas, kad visas VĒL SPĒLĒ ESOŠĀS mašīnas (t.i. tās, kas nav
    // uzspridzinātas) ir novietotas pareizi - uzspridzinātās mašīnas vairs
    // nekad netiks novietotas, tāpēc tās jāatņem no kopējā mērķa.
    void CheckForWin()
    {
        if (!timerRunning) return;

        if (correctlyPlaced + vehiclesLost >= totalVehicles)
        {
            Win();
        }
    }

    /// <summary>
    /// Sauc FlyingObjectControllerScript, kad velkamais transportlīdzeklis
    /// tiek iznīcināts sadursmē ar lidojošu objektu.
    /// </summary>
    public void RegisterVehicleLost()
    {
        if (!timerRunning) return; // spēle jau beigusies (uzvara/zaudējums)

        vehiclesLost++;
        livesRemaining = Mathf.Max(0, startingLives - vehiclesLost);
        UpdateLivesUI();

        if (livesRemaining <= 0)
        {
            GameOver();
        }
        else
        {
            // Ja tieši šī bija pēdējā mašīna, kas vēl bija palikusi spēlē,
            // spēle jāuzskata par uzvarētu (visas atlikušās jau novietotas).
            CheckForWin();
        }
    }

    void UpdateLivesUI()
    {
        if (livesText != null)
            livesText.text = "Dzīvības: " + livesRemaining + " / " + startingLives;

        if (livesImages != null)
        {
            for (int i = 0; i < livesImages.Length; i++)
            {
                if (livesImages[i] == null) continue;
                // Sirsniņa redzama, kamēr tās "numurs" ir mazāks par atlikušo dzīvību skaitu
                livesImages[i].enabled = (i < livesRemaining);
            }
        }
    }

    void Win()
    {
        timerRunning = false;

        int baseStars = ComputeStarsFromTime(elapsedTime);

        // Ja pazaudēta kaut viena mašīna, visas 3 zvaigznes vairs nav iespējamas.
        int maxStarsAllowed = Mathf.Clamp(startingLives - vehiclesLost, 0, startingLives);
        int finalStars = Mathf.Min(baseStars, maxStarsAllowed);

        if (victoryTimeText != null)
        {
            victoryTimeText.text = FormatTime(elapsedTime);
        }

        ShowStars(finalStars);

        if (victoryPanel != null) victoryPanel.SetActive(true);
        FreezeGame();
    }

    // Iesaldē visu spēli (lidojošie objekti, animācijas) un bloķē vilkšanu
    void FreezeGame()
    {
        isGameOver = true;
        Time.timeScale = 0f;
    }

    string FormatTime(float time)
    {
        int hours = Mathf.FloorToInt(time / 3600f);
        int minutes = Mathf.FloorToInt((time % 3600f) / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);
        return string.Format("{0:00}:{1:00}:{2:00}", hours, minutes, seconds);
    }

    int ComputeStarsFromTime(float time)
    {
        if (time <= threeStarTime) return 3;
        if (time <= twoStarTime) return 2;
        if (time <= oneStarTime) return 1;
        return 0;
    }

    void ShowStars(int count)
    {
        if (starImages == null) return;

        for (int i = 0; i < starImages.Length; i++)
        {
            if (starImages[i] == null) continue;
            starImages[i].sprite = (i < count) ? starFilledSprite : starEmptySprite;
        }
    }

    void GameOver()
    {
        timerRunning = false;
        if (gameOverPanel != null) gameOverPanel.SetActive(true);
        FreezeGame();
    }

    /// <summary>Piesaisti "Spēlēt vēlreiz" pogas OnClick() notikumam.</summary>
    public void RestartGame()
    {
        Time.timeScale = 1f;
        if (string.IsNullOrEmpty(sceneNameToReload))
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        else
            SceneManager.LoadScene(sceneNameToReload);
    }

    /// <summary>
    /// Piesaisti pogai (arī spēles laikā redzamajai) - izmanto mainMenuSceneName lauku.
    /// </summary>
    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }

    /// <summary>Piesaisti "Uz sākuma ainu" pogas OnClick() notikumam (ievadi ainas nosaukumu).</summary>
    public void GoToMainMenu(string mainMenuSceneName)
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }
}