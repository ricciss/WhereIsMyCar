using UnityEngine;
using UnityEngine.EventSystems;

public class DropPlaceScript : MonoBehaviour, IDropHandler
{
    private float placeZRot, carZRot, diffZRot;
    private Vector3 placeSize, carSize;
    private float xSizeDiff, ySizeDiff;
    public GameObjectScript gameObjectScript;
    public GameManagerScript gameManagerScript;

    void Awake()
    {
        gameObjectScript = Object.FindFirstObjectByType<GameObjectScript>();
        gameManagerScript = Object.FindFirstObjectByType<GameManagerScript>();
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag == null)
            return;

        RectTransform carRect = eventData.pointerDrag.GetComponent<RectTransform>();
        RectTransform placeRect = GetComponent<RectTransform>();

        // 1) Vai šī ir pareizā vieta (tags sakrīt)?
        bool sameTag = eventData.pointerDrag.tag.Equals(tag);

        // 2) Vai pagrieziens un izmērs sakrīt? (pārbauda tikai, ja tags sakrīt)
        bool rotationAndSizeOk = false;

        if (sameTag)
        {
            placeZRot = carRect.transform.eulerAngles.z;
            carZRot = placeRect.transform.eulerAngles.z;
            diffZRot = Mathf.Abs(placeZRot - carZRot);
            Debug.Log("Diff Z Rot: " + diffZRot);

            placeSize = carRect.localScale;
            carSize = placeRect.localScale;
            xSizeDiff = Mathf.Abs(placeSize.x - carSize.x);
            ySizeDiff = Mathf.Abs(placeSize.y - carSize.y);
            Debug.Log("Diff X Size: " + xSizeDiff);
            Debug.Log("Diff Y Size: " + ySizeDiff);

            rotationAndSizeOk =
                (diffZRot <= 7 || (diffZRot >= 353 && diffZRot <= 360)) &&
                (xSizeDiff <= 0.1f && ySizeDiff <= 0.1f);
        }

        if (sameTag && rotationAndSizeOk)
        {
            // ---------- PAREIZI NOVIETOTS ----------
            Debug.Log("Car placed correctly!");
            gameObjectScript.inRightPlace = true;

            if (gameManagerScript != null)
                gameManagerScript.RegisterCorrectPlacement(placeRect);

            carRect.anchoredPosition = placeRect.anchoredPosition;
            carRect.localScale = placeRect.localScale;
            carRect.localRotation = placeRect.localRotation;

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
                case "b2":
                    gameObjectScript.carSoundSource.PlayOneShot(gameObjectScript.sounds[5]);
                    break;
                case "Cements":
                    gameObjectScript.carSoundSource.PlayOneShot(gameObjectScript.sounds[6]);
                    break;
                case "e46":
                    gameObjectScript.carSoundSource.PlayOneShot(gameObjectScript.sounds[7]);
                    break;
                case "e61":
                    gameObjectScript.carSoundSource.PlayOneShot(gameObjectScript.sounds[8]);
                    break;
                case "Eskavators":
                    gameObjectScript.carSoundSource.PlayOneShot(gameObjectScript.sounds[9]);
                    break;
                case "Police":
                    gameObjectScript.carSoundSource.PlayOneShot(gameObjectScript.sounds[10]);
                    break;
                case "Traktors1":
                    gameObjectScript.carSoundSource.PlayOneShot(gameObjectScript.sounds[11]);
                    break;
                case "Traktors5":
                    gameObjectScript.carSoundSource.PlayOneShot(gameObjectScript.sounds[12]);
                    break;
                case "Fire":
                    gameObjectScript.carSoundSource.PlayOneShot(gameObjectScript.sounds[13]);
                    break;
                default:
                    Debug.Log("No matching tag found for the dropped object.");
                    break;
            }
        }
        else if (sameTag)
        {
            // ---------- PAREIZĀ VIETA, bet nepareizs pagrieziens/izmērs ----------
            // Mašīna paliek turpat, kur nomesta (neatlec atpakaļ).
            gameObjectScript.inRightPlace = false;
        }
        else
        {
            // ---------- NEPAREIZA VIETA (cits tags) ----------
            // Mašīna atlec atpakaļ savā sākuma vietā.
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
                case "b2":
                    gameObjectScript.b2.GetComponent<RectTransform>().localPosition = gameObjectScript.b2Coord;
                    break;
                case "Cements":
                    gameObjectScript.cements.GetComponent<RectTransform>().localPosition = gameObjectScript.cementsCoord;
                    break;
                case "e46":
                    gameObjectScript.e46.GetComponent<RectTransform>().localPosition = gameObjectScript.e46Coord;
                    break;
                case "e61":
                    gameObjectScript.e61.GetComponent<RectTransform>().localPosition = gameObjectScript.e61Coord;
                    break;
                case "Eskavators":
                    gameObjectScript.eskavators.GetComponent<RectTransform>().localPosition = gameObjectScript.eskavatorsCoord;
                    break;
                case "Police":
                    gameObjectScript.police.GetComponent<RectTransform>().localPosition = gameObjectScript.policeCoord;
                    break;
                case "Traktors1":
                    gameObjectScript.traktors1.GetComponent<RectTransform>().localPosition = gameObjectScript.traktors1Coord;
                    break;
                case "Traktors5":
                    gameObjectScript.traktors5.GetComponent<RectTransform>().localPosition = gameObjectScript.traktors5Coord;
                    break;
                case "Fire":
                    gameObjectScript.fireTruck.GetComponent<RectTransform>().localPosition = gameObjectScript.fireTruckCoord;
                    break;
                default:
                    Debug.Log("No matching tag found for the dropped object.");
                    break;
            }
        }
    }
}