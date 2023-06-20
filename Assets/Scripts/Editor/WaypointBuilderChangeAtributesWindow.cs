#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using UnityEditor;
using PixelH8.Data;

public class WaypointBuilderChangeAtributesWindow : EditorWindow
{
    public WaypointBuilder waypointBuilder;

    void OnGUI()
    {
        var currentWaypoint = waypointBuilder.SelectedWaypoint;
        var waypoint = currentWaypoint.GetComponent<Waypoint>();
        GUILayout.Label("Waypoint Name", EditorStyles.boldLabel);
        currentWaypoint.name = EditorGUILayout.TextField(currentWaypoint.name);
        GUILayout.Label("Waypoint Type", EditorStyles.boldLabel);
        waypoint.waypointType = (Waypoint.WaypointType)EditorGUILayout.EnumPopup(waypoint.waypointType);
        GUILayout.Label("Waypoint Complete Radius", EditorStyles.boldLabel);
        waypoint.radius = EditorGUILayout.FloatField(waypoint.radius);
        GUILayout.Label("Waypoint Wait Time", EditorStyles.boldLabel);
        waypoint.waitTime = EditorGUILayout.FloatField(waypoint.waitTime);

         if(GUILayout.Button("Done"))
            this.Close();
    }
}

#endif