using UnityEngine;
using UnityEngine.EventSystems;

public class DragAndDropScript : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public GameObjectScript gameObjectScript;
    private CanvasGroup canvasGroup;
    public ScreenBoundriesScript screenBoundriesScript;
    private FlyingObjectSpawnScript flyingSpawnScript;

    private Vector3 dragOffsetWorld;
    private Camera mainCamera;
    private Canvas canvas;

    void Awake()
    {
        flyingSpawnScript = Object.FindFirstObjectByType<FlyingObjectSpawnScript>();
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        if(gameObjectScript == null)
        {
            gameObjectScript = Object. FindFirstObjectByType<GameObjectScript>();
        }

        if(screenBoundriesScript == null)
        {
            screenBoundriesScript = Object.FindFirstObjectByType<ScreenBoundriesScript>();
        }

        canvas = GetComponentInParent<Canvas>();
        if (canvas != null)
        {
            mainCamera = canvas.worldCamera;
        } else
            Debug.LogWarning("Canvas not found in parent hierarchy of" + gameObject.name);


       
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("Clicked on " + gameObject.name);

        if(gameObjectScript.carSoundSource != null && gameObjectScript.sounds != null)
        {
            gameObjectScript.carSoundSource.PlayOneShot(gameObjectScript.sounds[0]);
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (GameManagerScript.isGameOver) return;

        Debug.Log("Start");
        GameObjectScript.isDragging = true;
        canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false;
        int lastIndex = transform.parent.childCount - 1;
        int positionIndex = Mathf.Max(0, lastIndex);
        transform.SetSiblingIndex(positionIndex);

            // Mākoņiem/lidmašīnām jāpaliek VIRS mašīnām (arī vilktajai)
            if (flyingSpawnScript != null && flyingSpawnScript.spawnPoint != null
                && flyingSpawnScript.spawnPoint.parent == transform.parent)
            {
                flyingSpawnScript.spawnPoint.SetAsLastSibling();
            }

        Vector3 cursorWorldPos;
        if (ScreenToWorldPoint(eventData.position, out cursorWorldPos))
        {
            dragOffsetWorld = transform.position - cursorWorldPos;
        }
        else
        {
            dragOffsetWorld = Vector3.zero;
        }
        GameObjectScript.lastDragged = eventData.pointerDrag;
        
    }

    private bool ScreenToWorldPoint(Vector2 screenPosition, out Vector3 worldPosition)
    {
        worldPosition = Vector3.zero;
        if(mainCamera == null)
        {
            Debug.LogWarning("Main camera is not assigned.");
            return false;
        }

        float z = Mathf.Abs(mainCamera.transform.position.z - transform.position.z);
        Vector3 screenPoint = new Vector3(screenPosition.x, screenPosition.y, z);
        worldPosition = mainCamera.ScreenToWorldPoint(screenPoint);
        return true;
    }

    public void OnDrag(PointerEventData eventData)
    {


        //if (GameManagerScript.isGameOver) return;

        Vector3 pointerWorld;
        if(!ScreenToWorldPoint(eventData.position, out pointerWorld))
        {
            return;
        }
        Vector3 desired = pointerWorld + dragOffsetWorld;
        desired.z = transform.position.z;

        screenBoundriesScript.RecalculateBounds();
        Vector2 clamped = screenBoundriesScript.GetClampedPosition(desired);

        transform.position = new Vector3(clamped.x, clamped.y, desired.z);


    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if(gameObjectScript.carSoundSource != null && gameObjectScript.sounds != null)
        {

            gameObjectScript.carSoundSource.PlayOneShot(gameObjectScript.sounds[0]);
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

   
}