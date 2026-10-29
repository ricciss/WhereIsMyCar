using System.Collections;
using UnityEngine;

/// <summary>
/// Uzliec uz jebkura objekta Hanoi ainā. Ainas laikā ekrāns ir tikai horizontāls
/// (automātiski pagriežas, ja telefons turēts vertikāli). Iziejot no ainas
/// atkal tiek atļauta jebkura orientācija.
/// </summary>
public class LandscapeOnlyScript : MonoBehaviour
{
    [Tooltip("Iziejot no ainas atļaut atpakaļ arī vertikālo orientāciju")]
    public bool restoreOnExit = true;

    void Awake()
    {
        // Ainas laikā vertikālā orientācija ir aizliegta
        Screen.autorotateToPortrait = false;
        Screen.autorotateToPortraitUpsideDown = false;
        Screen.autorotateToLandscapeLeft = true;
        Screen.autorotateToLandscapeRight = true;

        // Piespiež pagriezties uz horizontāli, ja ierīce šobrīd ir vertikāli
        Screen.orientation = ScreenOrientation.LandscapeLeft;

        StartCoroutine(EnableLandscapeAutoRotate());
    }

    // Pēc pagriešanās atļauj abas horizontālās puses (LandscapeLeft un LandscapeRight)
    IEnumerator EnableLandscapeAutoRotate()
    {
        yield return new WaitForSeconds(0.5f);
        Screen.orientation = ScreenOrientation.AutoRotation;
    }

    void OnDestroy()
    {
        if (!restoreOnExit) return;

        Screen.autorotateToPortrait = true;
        Screen.autorotateToPortraitUpsideDown = true;
        Screen.autorotateToLandscapeLeft = true;
        Screen.autorotateToLandscapeRight = true;
        Screen.orientation = ScreenOrientation.AutoRotation;
    }
}