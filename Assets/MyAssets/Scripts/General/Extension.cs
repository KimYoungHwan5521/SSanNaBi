using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using Unity.VisualScripting;
using UnityEngine;

public static class Extension
{
    public static Vector3 GetPredictPosition(this Vector3 origin, Vector3 velocity, Vector3 gravity, float time)
    {
        return origin + velocity * time + gravity * time * time / 2;
    }

    public static RaycastHit2D Parabolacast2D(this Vector3 origin, Vector3 velocity, Vector3 gravity, float wantTime, int split)
    {
        Vector3 formerPosition = origin;
        Ray ray = new Ray();
        ContactFilter2D filter = new ContactFilter2D();
        filter.useTriggers = false;
        filter.SetLayerMask(LayerMask.GetMask("Default"));
        RaycastHit2D[] hits = new RaycastHit2D[20];
        for(int i=0; i<split; i++)
        {
            float predictTime = wantTime / split * i;
            Vector3 predictPosition = origin.GetPredictPosition(velocity, gravity, predictTime);
            Vector3 direction = predictPosition - formerPosition;
            ray.origin = formerPosition;
            ray.direction = direction;
            Debug.DrawRay(ray.origin, ray.direction * direction.magnitude);
            int hitAmount = Physics2D.Raycast(ray.origin, ray.direction, filter, hits, direction.magnitude);
            if (hitAmount > 0) return hits[0];

            formerPosition = predictPosition;
        }
        return default;
    }
}
