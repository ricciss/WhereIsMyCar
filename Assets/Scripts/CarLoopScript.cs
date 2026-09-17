using UnityEngine;

public class CarLoopScript : MonoBehaviour
{
    [Tooltip("Ātrums, ar kādu mašīna brauc pa labi (vienības/sekundē)")]
    public float speed = 2f;

    [Tooltip("X pozīcija, kur mašīna atgriežas (ārpus ekrāna pa kreisi)")]
    public float leftX = -12f;

    [Tooltip("X pozīcija, pēc kuras mašīna 'pazūd' un atgriežas pa kreisi (ārpus ekrāna pa labi)")]
    public float rightX = 12f;

    void Update()
    {
        transform.Translate(Vector3.right * speed * Time.deltaTime);

        if (transform.position.x > rightX)
        {
            transform.position = new Vector3(leftX, transform.position.y, transform.position.z);
        }
    }
}