using UnityEngine;
using System.Collections.Generic;

[DefaultExecutionOrder(-100)] // Izpildās PIRMS GameObjectScript.Awake()
public class RandomPlacementScript : MonoBehaviour
{
    [Header("Mašīnas (12 esoši objekti scēnē)")]
    public RectTransform[] cars;            // 12 mašīnas
    public RectTransform[] carSpawnPoints;  // 17 iespējamie punkti

    [Header("Nomešanas vietas (12 Drop Places)")]
    public RectTransform[] dropPlaces;          // 12 objekti ar DropPlaceScript
    public RectTransform[] dropSpawnPoints;     // 17 iespējamie punkti

    void Awake()
    {
        RandomizePositions(cars, carSpawnPoints);
        RandomizePositions(dropPlaces, dropSpawnPoints);
    }

    void RandomizePositions(RectTransform[] objects, RectTransform[] points)
    {
        if (objects.Length == 0 || points.Length == 0) return;

        if (objects.Length > points.Length)
        {
            Debug.LogWarning($"Punktu ir mazāk ({points.Length}) nekā objektu ({objects.Length})! Nepietiek vietu.");
            return;
        }

        List<RectTransform> shuffledPoints = new List<RectTransform>(points);

        // Fisher-Yates shuffle - sajauc visus 17 punktus
        for (int i = shuffledPoints.Count - 1; i > 0; i--)
        {
            int randIndex = Random.Range(0, i + 1);
            (shuffledPoints[i], shuffledPoints[randIndex]) = (shuffledPoints[randIndex], shuffledPoints[i]);
        }

        // Paņem tikai tik daudz punktu, cik ir objektu (piem. 12 no 17)
        for (int i = 0; i < objects.Length; i++)
        {
            objects[i].localPosition = shuffledPoints[i].localPosition;
        }
    }
}
