using UnityEngine;
using UnityEngine.EventSystems;

public class ObjectTransformationScript : MonoBehaviour
{
    public float rotationSpeed = 90f;
    public float scaleSpeed = 0.5f;
    private bool rotateCW, rotateCCW, scaleUpY, scaleDownY, scaleUpX, scaleDownX;
    public static bool isTransforming = false;
    

    private void Update()
    {
        if(GameObjectScript.lastDragged == null)
        {
            return;
        }

        RectTransform rc = GameObjectScript.lastDragged.GetComponent<RectTransform>();

        if(rotateCW )
        {
            rc.Rotate(0, 0, -rotationSpeed * Time.deltaTime);
        }

        if (rotateCCW)
        {
            rc.Rotate(0, 0, rotationSpeed *  Time.deltaTime);
        }

        if (scaleUpY && rc.localScale.y < 0.9f)
        {
            rc.localScale += new Vector3(0, scaleSpeed * Time.deltaTime, 0);
        }

        if(scaleDownY && rc.localScale.y > 0.3f)
        {
            rc.localScale -= new Vector3(0, scaleSpeed * Time.deltaTime, 0);
        }

        if (scaleUpX && rc.localScale.x < 0.9f)
        {
            rc.localScale += new Vector3(scaleSpeed * Time.deltaTime, 0, 0);
        }

        if (scaleDownX && rc.localScale.x > 0.3f)
        {
            rc.localScale -= new Vector3(scaleSpeed * Time.deltaTime, 0, 0);
        }

        isTransforming = rotateCW || rotateCCW || scaleUpY || scaleDownY || scaleUpX || scaleDownX;
    }

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



    /*void Update()
    {
        if(GameObjectScript.lastDragged != null)
        {
            if(Input.GetKey(KeyCode.Z)) 
            {
                GameObjectScript.lastDragged.GetComponent<RectTransform>().Rotate(0, 0, Time.deltaTime * 12);
            }

            if(Input.GetKey(KeyCode.X))
            {
                GameObjectScript.lastDragged.GetComponent<RectTransform>().Rotate(0, 0, -Time.deltaTime * 12);
            }

            if( Input.GetKey(KeyCode.UpArrow))
            {
                if(GameObjectScript.lastDragged.GetComponent<RectTransform>().localScale.y < 0.9f)
                {
                    GameObjectScript.lastDragged.GetComponent<RectTransform>().localScale =
                    new Vector3(GameObjectScript.lastDragged.GetComponent<RectTransform>().localScale.x,
                    GameObjectScript.lastDragged.GetComponent<RectTransform>().localScale.y + 0.001f, 1f);
                }
            }

            if (Input.GetKey(KeyCode.DownArrow))
            {
                if (GameObjectScript.lastDragged.GetComponent<RectTransform>().localScale.y > 0.3f)
                {
                    GameObjectScript.lastDragged.GetComponent<RectTransform>().localScale =
                    new Vector3(GameObjectScript.lastDragged.GetComponent<RectTransform>().localScale.x,
                    GameObjectScript.lastDragged.GetComponent<RectTransform>().localScale.y - 0.001f, 1f);
                }
            }

            if (Input.GetKey(KeyCode.LeftArrow))
            {
                if (GameObjectScript.lastDragged.GetComponent<RectTransform>().localScale.y > 0.3f)
                {
                    GameObjectScript.lastDragged.GetComponent<RectTransform>().localScale =
                    new Vector3(GameObjectScript.lastDragged.GetComponent<RectTransform>().localScale.x - 0.001f,
                    GameObjectScript.lastDragged.GetComponent<RectTransform>().localScale.y, 1f);
                }
            }

            if (Input.GetKey(KeyCode.RightArrow))
            {
                if (GameObjectScript.lastDragged.GetComponent<RectTransform>().localScale.y < 0.9f)
                {
                    GameObjectScript.lastDragged.GetComponent<RectTransform>().localScale =
                    new Vector3(GameObjectScript.lastDragged.GetComponent<RectTransform>().localScale.x + 0.001f,
                    GameObjectScript.lastDragged.GetComponent<RectTransform>().localScale.y, 1f);
                }
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                Vector3 scale = GameObjectScript.lastDragged.GetComponent<RectTransform>().localScale;
                scale.x *= -1;
                GameObjectScript.lastDragged.GetComponent<RectTransform>().localScale = scale;
            }
        }
    }*/
}
