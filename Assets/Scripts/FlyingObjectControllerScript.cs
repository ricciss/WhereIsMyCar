using System.Collections;
using UnityEngine;
using UnityEngine.UI;


public class FlyingObjectControllerScript : MonoBehaviour
{
    [HideInInspector]
    public float speed = 1f;
    public float fadeDuration = 1.5f;
    public float waveAmplitude = 25f;
    public float waveFrequency = 1f;
    private GameObjectScript gameObjectScript;
    private ScreenBoundriesScript screenBoundriesScript;
    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;
    private bool isFadingOut = false;
    private bool IsExploading = false;
    private Image image;
    private Color originalColor;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();

        rectTransform = GetComponent<RectTransform>();

        image = GetComponent<Image>();
        if (image != null)
            originalColor = image.color;

        gameObjectScript = Object.FindFirstObjectByType<GameObjectScript>();
        screenBoundriesScript = Object.FindFirstObjectByType<ScreenBoundriesScript>();
        StartCoroutine(FadeIn());

        IEnumerator FadeIn()
        {
            float time = 0f;
            while (time < fadeDuration)
            {
                time += Time.deltaTime;
                canvasGroup.alpha = Mathf.Lerp(0f, 1f, time / fadeDuration);
                yield return null;
            }
            canvasGroup.alpha = 1f;
        }

    }

    // Update is called once per frame
    private void Update()
    {
        float waveOffset = Mathf.Sin(Time.time * waveFrequency) * waveAmplitude;
        rectTransform.anchoredPosition +=
            new Vector2(-speed * Time.deltaTime, waveOffset * Time.deltaTime);

        if (speed > 0 && transform.position.x < (screenBoundriesScript.minX + 80) && !isFadingOut)
        {
            StartCoroutine(FadeOutAndDestroy());
            isFadingOut = true;
        }

        if (speed < 0 && transform.position.x > (screenBoundriesScript.maxX - 80) && !isFadingOut)
        {
            StartCoroutine(FadeOutAndDestroy());
            isFadingOut = true;
        }

        if(CompareTag("Bomb") && !IsExploading && 
            RectTransformUtility.RectangleContainsScreenPoint(rectTransform, Input.mousePosition, Camera.main))
        {
            Debug.Log("THe cursor collided with a bomb!");
            TriggerExplosion();
        }

        if(GameObjectScript.isDragging && !isFadingOut && RectTransformUtility.RectangleContainsScreenPoint(
            rectTransform, Input.mousePosition, Camera.main))
        {
            Debug.Log("The cursor collided with a flying object!");
            if(GameObjectScript.lastDragged != null)
            {
                StartCoroutine(ShrinkAndDestroy(GameObjectScript.lastDragged, 0.5f));
                GameObjectScript.lastDragged = null;
                GameObjectScript.isDragging = false;
            }

            if (CompareTag("Bomb"))

                StartAndDestroy(Color.red);

            else
                StartAndDestroy(Color.cyan);
        }
    }

    IEnumerator ShrinkAndDestroy(GameObject obj, float duration)
    {
        Vector3 originalScale = obj.transform.localScale;
        Quaternion orginamRotation = obj.transform.rotation;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            obj.transform.localScale = Vector3.Lerp(originalScale, Vector3.zero, time / duration);
            float angle = Mathf.Lerp(0f, 360f, time / duration);
            obj.transform.rotation = Quaternion.Euler(0f, 0f, angle);

            yield return null;
        }
        Destroy(obj);
    }

    public void TriggerExplosion()
    {
        IsExploading = true;

        if(gameObjectScript.carSoundSource != null && gameObjectScript.sounds != null)
        {
            gameObjectScript.carSoundSource.PlayOneShot(gameObjectScript.sounds[15], 5f);
        }

        if(TryGetComponent<Animator>(out Animator animator))
        {
            animator.SetBool("explode", true);
        }

        if(image != null)
        {
            image.color = Color.red;
            StartCoroutine(RecoverColor(0.4f));
        }

        StartCoroutine(Vibrate());
        StartCoroutine(WaitBeforeExplode());
    }

    IEnumerator RecoverColor(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        if(image != null)
        {
            image.color = originalColor;
        }
    }

    IEnumerator Vibrate()
    {
        Vector2 originalPos = rectTransform.anchoredPosition;
        float duration = 0.4f;
        float elapsed = 0f;
        float intensity = 5f;

        while (elapsed < duration)
        {
            rectTransform.anchoredPosition = originalPos + Random.insideUnitCircle * intensity;
            elapsed += Time.deltaTime;
            yield return null;
        }
        rectTransform.anchoredPosition = originalPos;
    }

    IEnumerator WaitBeforeExplode()
    {
        float radius = 0f;
        if(TryGetComponent<CircleCollider2D>(out CircleCollider2D circle))
        {
            radius = circle.radius * transform.localScale.x;
        }

        ExplodeAndDestroyNearbyObjects(radius);

        yield return new WaitForSeconds(1f);
        ExplodeAndDestroyNearbyObjects(radius);
        Destroy(gameObject);
    }

    void ExplodeAndDestroyNearbyObjects(float radius)
    {
        Collider2D[] hit = Physics2D.OverlapCircleAll(transform.position, radius);
        foreach (Collider2D item in hit) 
        {
            if(item != null && item.gameObject != gameObject)
            {
                FlyingObjectControllerScript flyingObject = item.GetComponent<FlyingObjectControllerScript>();
                if(flyingObject != null && !flyingObject.IsExploading)
                {
                    flyingObject.StartAndDestroy(Color.cyan);
                }
            }
        }
    }

    public void StartAndDestroy(Color color)
    {
        if (!isFadingOut)
        {
            StartCoroutine(FadeOutAndDestroy());
            isFadingOut = true;

            if(image != null)
            {
                image.color = color;
                StartCoroutine(RecoverColor(0.5f));
            }

            StartCoroutine(Vibrate());
            if(gameObjectScript.carSoundSource != null && gameObjectScript.sounds != null)
            {
                gameObjectScript.carSoundSource.PlayOneShot(gameObjectScript.sounds[14]);
            }
        }
    }

    IEnumerator FadeOutAndDestroy()
    {
        float time = 0f;
        float startAlpha = canvasGroup.alpha;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, time / fadeDuration);
            yield return null;
        }
        canvasGroup.alpha = 0f;
        Destroy(gameObject);
    }
}
