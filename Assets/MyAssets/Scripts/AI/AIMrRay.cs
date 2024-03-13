using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class AIMrRay : AIBase
{
    public GameObject head;
    public float headSpeed;
    protected override void FixedUpdate()
    {
        target = FindTarget();
        if(target != null)
        {
            Vector2 lookVector = new Vector2(Mathf.Sin(head.transform.rotation.z), Mathf.Cos(head.transform.rotation.z));
            float preferRotation = Vector2.Angle(target.transform.position - head.transform.position, Vector2.up);
            if ((target.transform.position - head.transform.position).x > 0) preferRotation = 360 - preferRotation;
            float rotate;
            if(head.transform.rotation.eulerAngles.z > preferRotation)
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
            Debug.Log($"{head.transform.rotation.eulerAngles.z}, {preferRotation}");
            head.transform.Rotate(0, 0, rotate);
        }
    }

}
