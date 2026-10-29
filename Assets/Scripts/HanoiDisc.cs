using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Viens disks (0block, 1block ...). Uzliec uz katra diska UI Image objekta.
/// Vilkt var tikai augšējo disku uz stieņa.
/// </summary>
[RequireComponent(typeof(RectTransform))]
public class HanoiDisc : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    [HideInInspector] public int size;               // 0 = mazākais disks
    [HideInInspector] public HanoiPeg currentPeg;
    [HideInInspector] public HanoiGameManager manager;
    [HideInInspector] public RectTransform rect;

    private CanvasGroup canvasGroup;
    private RectTransform parentRect;
    private Camera uiCamera;
    private Vector2 grabOffset;
    private bool dragging;

    void Awake()
    {
        rect = GetComponent<RectTransform>();

        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();

        parentRect = transform.parent as RectTransform;

        Canvas canvas = GetComponentInParent<Canvas>();
        if (canvas != null)
        {
            canvas = canvas.rootCanvas;
            // Overlay režīmā kamera nav vajadzīga (null), citādi ņemam canvas kameru
            uiCamera = canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera;
        }
    }

    private bool PointerToLocal(Vector2 screenPos, out Vector2 local)
    {
        return RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRect, screenPos, uiCamera, out local);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        dragging = false;
        if (manager == null || !manager.CanDrag(this)) return;

        dragging = true;
        canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false;   // lai OnDrop trāpītu stieni/disku zem kursora
        transform.SetAsLastSibling();         // vilktais disks zīmējas virs visa

        Vector2 local;
        if (PointerToLocal(eventData.position, out local))
            grabOffset = (Vector2)rect.localPosition - local;
        else
            grabOffset = Vector2.zero;

        manager.PlayPickup();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!dragging || manager == null || manager.IsFinished) return;

        Vector2 local;
        if (!PointerToLocal(eventData.position, out local)) return;

        rect.localPosition = new Vector3(local.x + grabOffset.x, local.y + grabOffset.y, rect.localPosition.z);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!dragging) return;
        dragging = false;

        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

        // OnDrop (ja bija) jau nostrādājis un currentPeg ir atjaunināts.
        // Ja nomests nekurienē - disks atlec atpakaļ uz savu stieni.
        if (manager != null)
            manager.SnapToPeg(this);
    }

    // Ja disku nomet tieši uz cita diska - to uzskata par nomešanu uz tā diska stieņa
    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag == null || manager == null || currentPeg == null) return;

        HanoiDisc dragged = eventData.pointerDrag.GetComponent<HanoiDisc>();
        if (dragged != null && dragged != this)
            manager.TryMove(dragged, currentPeg);
    }
}