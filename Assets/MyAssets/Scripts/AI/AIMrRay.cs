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
                // �ε�ε� ������ ���� �ذ�
                rotate = preferRotation - head.transform.rotation.eulerAngles.z;
            }
            else if(head.transform.rotation.eulerAngles.z > preferRotation)
            {
                // �⺻������ ���� rotation.eulerAngle.z�� preferRotation ���� ũ�� �ݽð�� �������� ������ �ѹ��� ���ƹ�������(head �������� Vector2.up������ ���� ��) preferRotation ���� 1->360 Ȥ�� �� �ݴ�� ���� �޺��Ѵ�.
                // �� �� head�� ������������ ���°� �ƴ϶� �չ������� �ѹ����� ��� ���� ������ �װ��� ��ġ�� ���� ���ǹ��� �߰� ���ش�.
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
