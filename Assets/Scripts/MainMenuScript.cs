using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{
    [Header("Aina")]
    [Tooltip("Precīzs spēles ainas nosaukums (jāsakrīt ar Build Settings sarakstā)")]
    public string gameSceneName = "CityScene";

    [Tooltip("Bonus spēles aina (Hanojas tornis) - jāsakrīt ar Build Settings sarakstā")]
    public string bonusSceneName = "HanoiScene";

    private string sceneToLoad;

    [Header("Skaņa")]
    public AudioSource audioSource;   // ja tukšs, meklēs uz šī paša objekta
    public AudioClip clickSound;
    public float delayBeforeAction = 0.2f; // laiks, lai paspētu nospēlēties skaņai

    void Awake()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }

    // Piesaisti šo metodi "START" pogas OnClick() notikumam
    public void OnStartClicked()
    {
        sceneToLoad = gameSceneName;
        PlayClickSound();
        Invoke(nameof(LoadGameScene), delayBeforeAction);
    }

    // Piesaisti šo metodi "BONUS GAME" pogas OnClick() notikumam
    public void OnBonusClicked()
    {
        sceneToLoad = bonusSceneName;
        PlayClickSound();
        Invoke(nameof(LoadGameScene), delayBeforeAction);
    }

    // Piesaisti šo metodi "EXIT" pogas OnClick() notikumam
    public void OnExitClicked()
    {
        PlayClickSound();
        Invoke(nameof(QuitGame), delayBeforeAction);
    }

    void PlayClickSound()
    {
        if (audioSource != null && clickSound != null)
            audioSource.PlayOneShot(clickSound);
    }

    void LoadGameScene()
    {
        SceneManager.LoadScene(sceneToLoad);
    }

    void QuitGame()
    {
        Debug.Log("Iziet no spēles");
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}