using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JusticeCore : MonoBehaviour
{
    public Transform[] cores;
    float rotationSpeed = 100f;

    private void FixedUpdate()
    {
        // 1. �θ� ��ü�� ȸ����
        transform.Rotate(Vector3.forward * rotationSpeed * Time.fixedDeltaTime);

        float angle = transform.rotation.eulerAngles.z * Mathf.Deg2Rad;
        float radius = 6f; // ������ ����

        for(int i=0;i<cores.Length;i++)
        {
            // 2. �θ��� ȸ������ ���������� �ڽĵ��� ��� ��ġ
            cores[i].position = new Vector3(transform.position.x + Mathf.Cos(angle + 2 * Mathf.PI * i / cores.Length) * radius,
                                           transform.position.y + Mathf.Sin(angle + 2 * Mathf.PI * i / cores.Length) * radius,
                                           cores[i].position.z);
            // 3. �߾�(�θ���ġ)�� �ٶ󺸰�
            cores[i].rotation = Quaternion.Euler(0, 0, Mathf.Atan2((cores[i].position - transform.position).y, (cores[i].position - transform.position).x) * Mathf.Rad2Deg - 90);
        }

    }
}