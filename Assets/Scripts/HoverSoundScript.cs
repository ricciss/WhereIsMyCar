using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

// UI elementiem (Image zem Canvas) - izmanto EventSystem, nevis Collider2D.
public class HoverSoundScript : MonoBehaviour, IPointerEnterHandler
{
    [Header("Hover skaņa")]
    public AudioSource audioSource;   // ja tukšs, tiks izveidots automātiski
    public AudioClip hoverSound;
    [Tooltip("Cik sekundes atskaņot skaņu (piem. 1 vai 2), pat ja klips pats ir garāks")]
    public float hoverSoundDuration = 2f;

    [Header("Fona mūzikas klusināšana (ducking)")]
    [Tooltip("Ievelc objektu, kuram ir fona mūzikas AudioSource")]
    public AudioSource backgroundMusicSource;
    [Range(0f, 1f)]
    [Tooltip("Cik klusa kļūst fona mūzika, kamēr skan hover skaņa (0 = pilnībā izslēgta)")]
    public float duckedVolume = 0.15f;

    private float originalBgVolume;
    private Coroutine playCoroutine;

    void Awake()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        if (backgroundMusicSource != null)
            originalBgVolume = backgroundMusicSource.volume;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (audioSource != null && hoverSound != null && !audioSource.isPlaying)
        {
            if (playCoroutine != null)
                StopCoroutine(playCoroutine);

            playCoroutine = StartCoroutine(PlayForDuration());
        }
    }

    IEnumerator PlayForDuration()
    {
        if (backgroundMusicSource != null)
            backgroundMusicSource.volume = duckedVolume;

        audioSource.clip = hoverSound;
        audioSource.Play();

        yield return new WaitForSeconds(hoverSoundDuration);

        audioSource.Stop();

        if (backgroundMusicSource != null)
            backgroundMusicSource.volume = originalBgVolume;
    }
}