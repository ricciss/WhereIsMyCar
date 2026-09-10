using UnityEngine;
using UnityEngine.EventSystems;

public class DragAndDropScript : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public GameObjectScript gameObjectScript;
    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;
    public ScreenBoundriesScript screenBoundriesScript;

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null )
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        rectTransform = GetComponent<RectTransform>();
        gameObjectScript = Object.FindFirstObjectByType<GameObjectScript>();
        screenBoundriesScript = Object.FindFirstObjectByType<ScreenBoundriesScript>();
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        if(Input.GetMouseButton(0) && !Input.GetMouseButton(1) && !Input.GetMouseButton(2))
        {
            Debug.Log("Left mouse button clicked on " + gameObject.name);
            gameObjectScript.carSoundSource.PlayOneShot(gameObjectScript.sounds[0]);
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
       if(Input.GetMouseButton(0) && !Input.GetMouseButton(1) && !Input.GetMouseButton(2))
        {
            GameObjectScript.isDragging = true;
            canvasGroup.alpha = 0.6f;
            canvasGroup.blocksRaycasts = false;
            int lastIndex = transform.parent.childCount - 1;
            int positionIndex = Mathf.Max(0, lastIndex);
            transform.SetSiblingIndex(positionIndex);

            Vector3 cursorWorldPos = Camera.main.ScreenToWorldPoint(new Vector3(
                Input.mousePosition.x, Input.mousePosition.y, screenBoundriesScript.screenPoint.z));
            rectTransform.position = cursorWorldPos;
            screenBoundriesScript.screenPoint = Camera.main.WorldToScreenPoint(rectTransform.localPosition);

            screenBoundriesScript.offset = rectTransform.localPosition - Camera.main.ScreenToWorldPoint(new Vector3(
                Input.mousePosition.x, Input.mousePosition.y,
                screenBoundriesScript.screenPoint.z));
            GameObjectScript.lastDragged = eventData.pointerDrag;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (Input.GetMouseButton(0) && !Input.GetMouseButton(1) && !Input.GetMouseButton(2))
        {
            Vector3 cursScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenBoundriesScript.screenPoint.z);
            Vector3 curPosition = Camera.main.ScreenToWorldPoint(cursScreenPoint) + screenBoundriesScript.offset;

            rectTransform.position = screenBoundriesScript.GetClampedPosition(curPosition);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    { 
        if (Input.GetMouseButtonUp(0))
        {
            GameObjectScript.isDragging = false;
            Debug.Log("OnEndDrag called for " + gameObject.name);
            canvasGroup.alpha = 1f;
            canvasGroup.blocksRaycasts = true;

            if (gameObjectScript.inRightPlace)
            {
                canvasGroup.blocksRaycasts = false;
                GameObjectScript.lastDragged = null;
            }

            gameObjectScript.inRightPlace = false;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
