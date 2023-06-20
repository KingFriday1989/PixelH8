using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoint : MonoBehaviour
{
    public enum WaypointType
    {
        Default,
        MustMove,
        Patrol,
        Hold,
        AreaTransition,
        Action,
        Spawn,
        NotAccessAble,
    }
    public WaypointType waypointType;
    public List<Waypoint> ConnectedWaypoints;
    public float radius;
    public float waitTime;

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        
        if (ConnectedWaypoints.Count != 0)
            foreach (var waypoint in ConnectedWaypoints)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawLine(transform.position,waypoint.transform.position);
            }
    }
#endif
}
