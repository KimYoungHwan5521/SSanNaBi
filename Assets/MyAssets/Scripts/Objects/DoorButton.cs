using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorButton : MonoBehaviour
{
    public Animator doors;
    public bool openDefault;
    public float offTime;
    float curOffTime;

    SpriteRenderer backSR;
    Transform frontT;
    bool isButtonOn;

    private void Start()
    {
        backSR = GetComponentsInChildren<SpriteRenderer>()[0];
        frontT = GetComponentsInChildren<Transform>()[1];
        if (openDefault) doors.SetBool("isDoorOpen", true);
    }

    private void Update()
    {
        backSR.material.color = Color.green;
        if (isButtonOn)
        {
            if(curOffTime < 0)
            {
                isButtonOn = false;
                doors.SetBool("isDoorOpen", isButtonOn ^ openDefault);
            }
            frontT.localScale = (offTime - curOffTime) / offTime * Vector3.one;
            curOffTime -= Time.deltaTime;
        }
        else
        {
            frontT.localScale = Vector3.zero;
            backSR.material.color = Color.red;
        }
    }

    public void ButtonOn()
    {
        isButtonOn= true;
        doors.SetBool("isDoorOpen", isButtonOn ^ openDefault);
        curOffTime = offTime;
    }

}
