using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

[ExecuteInEditMode]
public class PatrolBlockNodeDrawer : MonoBehaviour
{
/*
    private void Update()
    {
        if (nodes.Length < 2)
        {
            nodes = new Vector2[2];
        }
        else if (nodes.Length > 10)
        {
            nodes = new Vector2[10];
        }
        for (int i = 0; i < nodes.Length - 2; i++)
        {
            if (i + 2 >= nodes.Length)
            {
                nodeTransforms[i].position = transform.parent.position;
            }

        }
        startPoint.position = transform.parent.position;
        nodes[0] = startPoint.position;
        for (int i = 0; i < nodes.Length - 2; i++)
        {
            nodes[i + 1] = nodeTransforms[i].position;
        }
        nodes[nodes.Length - 1] = endPoint.position;
        body.transform.position = startPoint.position;

        lineRenderer.positionCount = nodes.Length;
        for (int i = 0; i < nodes.Length; i++)
        {
            lineRenderer.SetPosition(i, nodes[i]);
        }
    }
*/
}
