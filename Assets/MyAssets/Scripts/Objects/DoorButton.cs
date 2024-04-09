using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorButton : MonoBehaviour
{
    public Animator doors;
    public float offTime;
    float curOffTime;

    SpriteRenderer backSR;
    Transform frontT;
    bool isButtonOn;

    private void Start()
    {
        backSR = GetComponentsInChildren<SpriteRenderer>()[0];
        frontT = GetComponentsInChildren<Transform>()[1];
    }

    private void Update()
    {
        backSR.material.color = Color.green;
        if (isButtonOn)
        {
            if(curOffTime < 0)
            {
                doors.SetTrigger("DoorOpen/Close");
                isButtonOn = false;
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
        doors.SetTrigger("DoorOpen/Close");
        curOffTime = offTime;
        isButtonOn= true;
    }

}
