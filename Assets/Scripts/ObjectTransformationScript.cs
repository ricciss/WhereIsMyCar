using UnityEngine;
using UnityEngine.EventSystems;

public class ObjectTransformationScript : MonoBehaviour
{
    public float rotationSpeed = 90f;
    public float scaleSpeed = 0.5f;
    public float minScale = 0.3f;
    public float maxScale = 0.9f;

    private bool rotateCW, rotateCCW, scaleUpY, scaleDownY, scaleUpX, scaleDownX;
    public static bool isTransforming = false;

    private void Update()
    {
        // Space darbojas uz datora (testēšanai), uz telefona izmanto FlipButton
        if (Input.GetKeyDown(KeyCode.Space))
        {
            FlipObject();
        }

        if (GameObjectScript.lastDragged == null)
        {
            isTransforming = false;
            return;
        }

        RectTransform rc = GameObjectScript.lastDragged.GetComponent<RectTransform>();
        Vector3 scale = rc.localScale;
        float signX = scale.x < 0 ? -1f : 1f;

        if (rotateCW)
        {
            rc.Rotate(0, 0, -rotationSpeed * Time.deltaTime);
        }

        if (rotateCCW)
        {
            rc.Rotate(0, 0, rotationSpeed * Time.deltaTime);
        }

        if (scaleUpY && scale.y < maxScale)
        {
            scale.y += scaleSpeed * Time.deltaTime;
        }

        if (scaleDownY && scale.y > minScale)
        {
            scale.y -= scaleSpeed * Time.deltaTime;
        }

        // X mērogs strādā ar absolūto vērtību, lai flip (negatīvs X) nesalauž ierobežojumus
        float absX = Mathf.Abs(scale.x);

        if (scaleUpX && absX < maxScale)
        {
            absX += scaleSpeed * Time.deltaTime;
        }

        if (scaleDownX && absX > minScale)
        {
            absX -= scaleSpeed * Time.deltaTime;
        }

        scale.x = absX * signX;
        rc.localScale = scale;

        isTransforming = rotateCW || rotateCCW || scaleUpY || scaleDownY || scaleUpX || scaleDownX;
    }

    // ---------- Flip (parasta Button, OnClick) ----------
    public void FlipObject()
    {
        if (GameObjectScript.lastDragged == null)
        {
            return;
        }

        RectTransform rc = GameObjectScript.lastDragged.GetComponent<RectTransform>();
        Vector3 scale = rc.localScale;
        scale.x *= -1f;
        rc.localScale = scale;
    }

    // ---------- Turēšanas pogas (Event Trigger: Pointer Down / Pointer Up) ----------
    public void StartRotateCW(BaseEventData eventData) { rotateCW = true; }
    public void StopRotateCW(BaseEventData eventData) { rotateCW = false; }

    public void StartRotateCCW(BaseEventData eventData) { rotateCCW = true; }
    public void StopRotateCCW(BaseEventData eventData) { rotateCCW = false; }

    public void StartScaleUpY(BaseEventData eventData) { scaleUpY = true; }
    public void StopScaleUpY(BaseEventData eventData) { scaleUpY = false; }

    public void StartScaleDownY(BaseEventData eventData) { scaleDownY = true; }
    public void StopScaleDownY(BaseEventData eventData) { scaleDownY = false; }

    public void StartScaleUpX(BaseEventData eventData) { scaleUpX = true; }
    public void StopScaleUpX(BaseEventData eventData) { scaleUpX = false; }

    public void StartScaleDownX(BaseEventData eventData) { scaleDownX = true; }
    public void StopScaleDownX(BaseEventData eventData) { scaleDownX = false; }
}