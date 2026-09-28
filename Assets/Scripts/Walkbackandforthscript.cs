using UnityEngine;

// Uzliec šo skriptu uz cilvēciņa (UI objekta ar RectTransform).
// Viņš noiet noteiktu attālumu vienā virzienā, apgriežas un iet atpakaļ - bezgalīgi.
[RequireComponent(typeof(RectTransform))]
public class WalkBackAndForthScript : MonoBehaviour
{
    [Header("Kustība")]
    [Tooltip("Cik tālu cilvēciņš noiet vienā virzienā (Canvas vienībās/pikseļos)")]
    public float walkDistance = 150f;

    [Tooltip("Ātrums (vienības sekundē)")]
    public float speed = 40f;

    [Tooltip("(1,0) = pa labi/pa kreisi, (0,1) = augšup/lejup")]
    public Vector2 direction = Vector2.right;

    [Header("Apstāšanās galos")]
    [Tooltip("Cik sekundes pastāv galā, pirms apgriežas (0 = uzreiz)")]
    public float pauseAtEnds = 0.5f;

    [Header("Apgriešanās")]
    [Tooltip("Apgriež attēlu (spoguļo pa X), kad iet otrādi")]
    public bool flipSpriteWhenTurning = true;

    private RectTransform rectTransform;
    private Vector2 startPos;
    private Vector2 endPos;
    private Vector2 target;
    private float pauseTimer = 0f;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        startPos = rectTransform.anchoredPosition;
        endPos = startPos + direction.normalized * walkDistance;
        target = endPos;
    }

    void Update()
    {
        // Time.deltaTime = 0, kad spēle iesaldēta (uzvara/zaudējums), tāpēc viņi arī apstājas
        if (pauseTimer > 0f)
        {
            pauseTimer -= Time.deltaTime;
            return;
        }

        rectTransform.anchoredPosition = Vector2.MoveTowards(
            rectTransform.anchoredPosition, target, speed * Time.deltaTime);

        if (Vector2.Distance(rectTransform.anchoredPosition, target) < 0.01f)
        {
            // Sasniedza galu - pārslēdz mērķi uz otru pusi un apgriežas
            target = (target == endPos) ? startPos : endPos;
            pauseTimer = pauseAtEnds;

            if (flipSpriteWhenTurning)
            {
                Vector3 scale = rectTransform.localScale;
                scale.x *= -1f;
                rectTransform.localScale = scale;
            }
        }
    }
}