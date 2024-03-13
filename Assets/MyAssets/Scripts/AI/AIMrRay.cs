using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class AIMrRay : AIBase
{
    public GameObject head;
    public SpriteRenderer laserSR;
    public float headSpeed;

    protected override void FixedUpdate()
    {
        target = FindTarget();
        if(target != null)
        {
            controlledCharacter.anim.speed = 1;
            controlledCharacter.anim.SetTrigger("doAttack");
            laserSR.enabled = true;
            float preferRotation = Vector2.Angle(target.transform.position - head.transform.position, Vector2.up);
            if ((target.transform.position - head.transform.position).x > 0) preferRotation = 360 - preferRotation;
            float rotate;
            if(Mathf.Abs(head.transform.rotation.eulerAngles.z - preferRotation) < headSpeed)
            {
                // 부들부들 떨리던 문제 해결
                rotate = preferRotation - head.transform.rotation.eulerAngles.z;
            }
            else if(head.transform.rotation.eulerAngles.z > preferRotation)
            {
                // 기본적으로 현재 rotation.eulerAngle.z가 preferRotation 보다 크면 반시계로 움직여야 하지만 한바퀴 돌아버렸을때(head 기준으로 Vector2.up방향을 지날 때) preferRotation 값이 1->360 혹은 그 반대로 값이 급변한다.
                // 이 때 head가 가까운방향으로 도는게 아니라 먼방향으로 한바퀴를 길게 돌기 때문에 그것을 고치기 위해 조건문을 추가 해준다.
                if (head.transform.rotation.eulerAngles.z - preferRotation < 180) rotate = -headSpeed;
                else rotate = headSpeed;
            }
            else
            {
                if (preferRotation - head.transform.rotation.eulerAngles.z < 180) rotate = headSpeed;
                else rotate = -headSpeed;
            }
            head.transform.Rotate(0, 0, rotate);
        }
        else
        {
            controlledCharacter.anim.ResetTrigger("doAttack");
            controlledCharacter.anim.speed = 100;
            laserSR.enabled= false;
        }
    }

    private void MrRayAttack()
    {
        laserSR.gameObject.GetComponent<BoxCollider2D>().enabled = true;
    }

    private void MrRayAttackEnd()
    {
        laserSR.gameObject.GetComponent<BoxCollider2D>().enabled = false;
    }

}
