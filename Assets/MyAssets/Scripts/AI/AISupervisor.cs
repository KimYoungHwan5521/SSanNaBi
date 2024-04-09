using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class AISupervisor : MonoBehaviour
{
    public SpriteRenderer eye;
    public TextMeshProUGUI deathTimerText;

    Transform playerT;
    Vector3 preferPosition;
    Vector3 preferDirection;
    RectTransform canvas;
    float xPos;
    float yPos;
    Vector2 pos;

    public float deathTime;
    float curDeathTime;

    public float moveSpeed;
    float moveSpeedCorrection;
    bool isWatching;

    private void Start()
    {
        playerT = GameObject.FindGameObjectWithTag("Player").transform;
        canvas = GetComponentInChildren<Canvas>().GetComponent<RectTransform>();
        curDeathTime = deathTime;
        preferPosition = transform.position;
    }

    private void Update()
    {
        if(isWatching)
        {
            deathTimerText.enabled = true;
            deathTimerText.text = $"{(int)Mathf.Clamp(curDeathTime, 0, deathTime)}";
            xPos = Mathf.Clamp(Camera.main.WorldToScreenPoint(transform.position).x, 0, canvas.rect.width - 10 - deathTimerText.rectTransform.rect.width);
            yPos = Mathf.Clamp(Camera.main.WorldToScreenPoint(transform.position).y, deathTimerText.rectTransform.rect.height, canvas.rect.height - 10 - deathTimerText.rectTransform.rect.height);
            pos = new Vector2(xPos, yPos);
            deathTimerText.transform.position = pos;
            curDeathTime -= Time.deltaTime;
        }
        else
        {
            deathTimerText.enabled = false;
            curDeathTime = deathTime;
        }
    }

    void FixedUpdate()
    {
        if(FindPlayer())
        {
            isWatching= true;
            preferPosition = playerT.position;
            eye.material.color = new Color(1, Mathf.Clamp(eye.material.color.g - 3 * Time.fixedDeltaTime, 0, 1), 0);
        }
        else
        {
            isWatching= false;
            eye.material.color = new Color(1, Mathf.Clamp(eye.material.color.g + Time.fixedDeltaTime, 0, 1), 0);

        }
        preferDirection = new Vector3(preferPosition.x - transform.position.x, preferPosition.y - transform.position.y, 0);
        if (Vector2.Distance(transform.position, preferPosition) > 20) moveSpeedCorrection = 3;
        else moveSpeedCorrection = 1;
        transform.position += moveSpeed * moveSpeedCorrection * Time.fixedDeltaTime * preferDirection;

    }

    bool FindPlayer()
    {
        RaycastHit2D[] hits = new RaycastHit2D[10];
        Physics2D.RaycastNonAlloc(playerT.position, transform.forward, hits, 2f, LayerMask.GetMask("Default"));
        for(int i=0; i<hits.Length; i++)
        {
            if (hits[i].collider == null) return true;
            if (hits[i].collider.CompareTag("BackWall"))
            {
                return false;
            }
        }
        return true;
    }
}
