using UnityEngine;

public class GameObjectScript : MonoBehaviour
{
    public GameObject garbageTruck;
    public GameObject medicine;
    public GameObject schoolBuss;
    public GameObject b2;
    public GameObject cements;
    public GameObject e46;
    public GameObject e61;
    public GameObject eskavators;
    public GameObject police;
    public GameObject traktors1;
    public GameObject traktors5;
    public GameObject fireTruck;

    [HideInInspector]
    public Vector2 garbageTruckCoord;
    [HideInInspector]
    public Vector2 medicineCoord;
    [HideInInspector]
    public Vector2 schoolBussCoord;
    [HideInInspector]
    public Vector2 b2Coord;
    [HideInInspector]
    public Vector2 cementsCoord;
    [HideInInspector]
    public Vector2 e46Coord;
    [HideInInspector]
    public Vector2 e61Coord;
    [HideInInspector]
    public Vector2 eskavatorsCoord;
    [HideInInspector]
    public Vector2 policeCoord;
    [HideInInspector]
    public Vector2 traktors1Coord;
    [HideInInspector]
    public Vector2 traktors5Coord;
    [HideInInspector]
    public Vector2 fireTruckCoord;


    public Canvas canvas;
    public AudioSource carSoundSource;
    public AudioClip[] sounds;

    [HideInInspector]
    public bool inRightPlace = false;
    public static GameObject lastDragged = null;
    public static bool isDragging = false;

    void Awake()
    {
        garbageTruckCoord = garbageTruck.GetComponent<RectTransform>().localPosition;
        medicineCoord = medicine.GetComponent<RectTransform>().localPosition;
        schoolBussCoord = schoolBuss.GetComponent<RectTransform>().localPosition;
        b2Coord = b2.GetComponent<RectTransform>().localPosition;
        cementsCoord = cements.GetComponent<RectTransform>().localPosition;
        e46Coord = e46.GetComponent<RectTransform>().localPosition;
        e61Coord = e61.GetComponent<RectTransform>().localPosition;
        eskavatorsCoord = eskavators.GetComponent<RectTransform>().localPosition;
        policeCoord = police.GetComponent<RectTransform>().localPosition;
        traktors1Coord = traktors1.GetComponent<RectTransform>().localPosition;
        traktors5Coord = traktors5.GetComponent<RectTransform>().localPosition;
        fireTruckCoord = fireTruck.GetComponent<RectTransform>().localPosition;

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
