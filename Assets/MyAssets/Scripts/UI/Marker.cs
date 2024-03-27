using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Marker : MonoBehaviour
{
    GameObject owner;
    Transform arrow;
    Image[] images;

    RectTransform mainCanvas;
    Transform player;

    float xPos;
    float yPos;
    Vector2 pos;

    public bool isExecutor;

    public void Initialize(GameObject owner)
    {
        this.owner = owner;
        arrow = GetComponentsInChildren<Transform>()[1];
        images = GetComponentsInChildren<Image>();
    }

    void Start()
    {
        mainCanvas = GameObject.FindGameObjectWithTag("MainCanvas").GetComponent<RectTransform>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
    }

    void Update()
    {
        if (owner != null)
        {
            if (Vector2.Distance(owner.transform.position, player.position) > 100)
            {
                HideMarker();
            }
            else
            {
                if(!isExecutor) ExposeMarker();
                xPos = Mathf.Clamp(Camera.main.WorldToScreenPoint(owner.transform.position + Vector3.up * 5).x, 30, mainCanvas.rect.width - 30);
                yPos = Mathf.Clamp(Camera.main.WorldToScreenPoint(owner.transform.position + Vector3.up * 5).y, 30, mainCanvas.rect.height - 30);
                pos = new Vector2(xPos, yPos);
                transform.position = pos;

                Vector2 lookVector = Camera.main.WorldToScreenPoint(owner.transform.position) - transform.position;
                arrow.eulerAngles = new Vector3(0, 0, Mathf.Atan2(lookVector.y, lookVector.x)*Mathf.Rad2Deg + 90);
            }
        }
    }

    public void HideMarker()
    {
        Debug.Log("hide");
        images[0].color = new Color(images[0].color.r, images[0].color.g, images[0].color.b, 0);
        images[1].color = new Color(images[1].color.r, images[1].color.g, images[1].color.b, 0);

    }

    public void ExposeMarker()
    {
        images[0].color = new Color(images[0].color.r, images[0].color.g, images[0].color.b, 1f);
        images[1].color = new Color(images[1].color.r, images[1].color.g, images[1].color.b, 1f);

    }
}
