using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Marker : MonoBehaviour
{
    GameObject owner;
    Transform arrow;

    RectTransform mainCanvas;

    float xPos;
    float yPos;
    Vector2 pos;

    public void Initialize(GameObject owner)
    {
        this.owner = owner;
        arrow = GetComponentsInChildren<Transform>()[1];
    }

    void Start()
    {
        mainCanvas = GameObject.FindGameObjectWithTag("MainCanvas").GetComponent<RectTransform>();
    }

    void Update()
    {
        if (owner != null)
        {
            xPos = Mathf.Clamp(Camera.main.WorldToScreenPoint(owner.transform.position + Vector3.up * 5).x, 30, mainCanvas.rect.width - 30);
            yPos = Mathf.Clamp(Camera.main.WorldToScreenPoint(owner.transform.position + Vector3.up * 5).y, 30, mainCanvas.rect.height - 30);
            pos = new Vector2(xPos, yPos);
            transform.position = pos;

            Vector2 lookVector = Camera.main.WorldToScreenPoint(owner.transform.position) - transform.position;
            arrow.eulerAngles = new Vector3(0, 0, Mathf.Atan2(lookVector.y, lookVector.x)*Mathf.Rad2Deg + 90);
        }
    }
}
