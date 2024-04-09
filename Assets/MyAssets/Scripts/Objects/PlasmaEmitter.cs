using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class PlasmaEmitter : MonoBehaviour
{
    LineRenderer lineRenderer;
    EdgeCollider2D[] edgeColliders;

    Vector3 platPosition;
    Vector3 bodyPosition;
    Vector2 edgeStart;
    Vector2 edgeEnd;
    Vector2 lookDirection;
    List<Vector2> edges;

    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        edgeColliders= GetComponentsInChildren<EdgeCollider2D>();

        platPosition = GetComponentsInChildren<Transform>()[1].position;
        bodyPosition = GetComponentsInChildren<Transform>()[2].position;
        lineRenderer.SetPosition(0, bodyPosition);
        edgeStart = Vector2.zero;

        lookDirection = bodyPosition - platPosition;
        lookDirection.Normalize();
    }

    void FixedUpdate()
    {
        RaycastHit2D[] hits = new RaycastHit2D[10];
        Physics2D.RaycastNonAlloc(bodyPosition, lookDirection, hits, 200f);

        bool firstHit = false;
        for(int i=0; i<hits.Length; i++)
        {
            if (firstHit || hits[i].collider == null) break;
            if((hits[i].collider.CompareTag("Untagged") || hits[i].collider.CompareTag("Grabable")))
            {
                lineRenderer.SetPosition(1, hits[i].point);
                edgeEnd= Vector2.Distance(bodyPosition, hits[i].point) * Vector2.down;
                edges = new List<Vector2> { edgeStart, edgeEnd };
                edgeColliders[0].SetPoints(edges);
                edgeColliders[1].SetPoints(edges);
                firstHit = true;
            }
        }
    }
}
