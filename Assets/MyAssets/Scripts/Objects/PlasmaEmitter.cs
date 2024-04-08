using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class PlasmaEmitter : MonoBehaviour
{
    LineRenderer lineRenderer;
    EdgeCollider2D[] edgeColliders;
    Vector2 edgeStart;
    Vector2 edgeEnd;
    List<Vector2> edges;

    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        edgeColliders= GetComponentsInChildren<EdgeCollider2D>();

        lineRenderer.SetPosition(0, transform.position - GetComponent<Collider2D>().bounds.extents.y * 3 / 4 * Vector3.up);
        edgeStart = Vector2.zero;
    }

    void FixedUpdate()
    {
        RaycastHit2D[] hits = new RaycastHit2D[10];
        Physics2D.RaycastNonAlloc(transform.position, Vector2.down, hits, 30f);

        bool firstHit = false;
        for(int i=0; i<hits.Length; i++)
        {
            if (firstHit || hits[i].collider == null) break;
            if((hits[i].collider.CompareTag("Untagged") || hits[i].collider.CompareTag("Grabable")))
            {
                lineRenderer.SetPosition(1, hits[i].point);
                edgeEnd= Vector2.Distance(transform.position, hits[i].point) * Vector2.down;
                edges = new List<Vector2> { edgeStart, edgeEnd };
                edgeColliders[0].SetPoints(edges);
                edgeColliders[1].SetPoints(edges);
                firstHit = true;
            }
        }
    }
}
