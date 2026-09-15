using UnityEngine;

public class ObjectTransformationScript : MonoBehaviour
{
    public GameObjectScript gameObjectScript;

    private void Awake()
    {
        gameObjectScript = FindFirstObjectByType<GameObjectScript>();
    }


    void Update()
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
        }
    }
}
