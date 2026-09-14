using NUnit.Framework.Constraints;
using UnityEngine;
using UnityEngine.EventSystems;

public class DropPlaceScript : MonoBehaviour, IDropHandler
{
    private float placeZRot, carZRot, diffZRot;
    private Vector3 placeSize, carSize;
    private float xSizeDiff, ySizeDiff;
    public GameObjectScript gameObjectScript;

    void Awake()
    {
        gameObjectScript = Object.FindFirstObjectByType<GameObjectScript>();
    }

    public void OnDrop(PointerEventData eventData)
    {
        if((eventData.pointerDrag != null) && Input.GetMouseButtonUp(0) && (!Input.GetMouseButton(2))) 
        {
            if (eventData.pointerDrag.tag.Equals(tag))
            {
                placeZRot = eventData.pointerDrag.GetComponent<RectTransform>().transform.eulerAngles.z;
                carZRot = GetComponent<RectTransform>().transform.eulerAngles.z;
                diffZRot = Mathf.Abs(placeZRot - carZRot);
                Debug.Log("Diff Z Rot: " + diffZRot);

                placeSize = eventData.pointerDrag.GetComponent<RectTransform>().localScale;
                carSize = GetComponent<RectTransform>().localScale;
                xSizeDiff = Mathf.Abs(placeSize.x - carSize.x);
                ySizeDiff = Mathf.Abs(placeSize.y - carSize.y);
                Debug.Log("Diff X Size: " + xSizeDiff);
                Debug.Log("Diff Y Size: " + ySizeDiff);

                if((diffZRot <= 7 || (diffZRot >=353 && diffZRot <=360)) && (xSizeDiff <= 0.08f && ySizeDiff <= 0.08f))
                {
                    Debug.Log("Car placed correctly!");
                    gameObjectScript.inRightPlace = true;
                    eventData.pointerDrag.GetComponent<RectTransform>().anchoredPosition = GetComponent<RectTransform>().anchoredPosition;
                    eventData.pointerDrag.GetComponent<RectTransform>().localScale = GetComponent<RectTransform>().localScale;
                    eventData.pointerDrag.GetComponent<RectTransform>().localRotation = GetComponent<RectTransform>().localRotation;

                    switch (eventData.pointerDrag.tag)
                    {
                        case "Garbage":
                            gameObjectScript.carSoundSource.PlayOneShot(gameObjectScript.sounds[1]);
                            break;

                        case "Ambulance":
                            gameObjectScript.carSoundSource.PlayOneShot(gameObjectScript.sounds[2]);
                            break;

                        case "School":
                            gameObjectScript.carSoundSource.PlayOneShot(gameObjectScript.sounds[3]);
                            break;

                        default:
                            Debug.Log("No matching tag found for the dropped object.");
                            break;

                    }

                }

            } else
            {
                gameObjectScript.inRightPlace = false;
                gameObjectScript.carSoundSource.PlayOneShot(gameObjectScript.sounds[4]);

                switch (eventData.pointerDrag.tag)
                {
                    case "Garbage":
                        gameObjectScript.garbageTruck.GetComponent<RectTransform>().localPosition = gameObjectScript.garbageTruckCoord;
                        break;

                    case "Ambulance":
                        gameObjectScript.medicine.GetComponent<RectTransform>().localPosition = gameObjectScript.medicineCoord;
                        break;

                    case "School":
                        gameObjectScript.schoolBuss.GetComponent<RectTransform>().localPosition = gameObjectScript.schoolBussCoord;
                        break;

                    default:
                        Debug.Log("No matching tag found for the dropped object.");
                        break;

                }
            }
        }
    }
    
}
