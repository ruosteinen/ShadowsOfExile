using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Waypoint : MonoBehaviour
{
    public Waypoint[] edges;

    private void OnDrawGizmos()
    {
        if(edges != null)
        {
            Gizmos.color = Color.green;
            foreach(Waypoint edge in edges)
            {
                Gizmos.DrawLine(transform.position, edge.transform.position);
            }
        }
    }
}
