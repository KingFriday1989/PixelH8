using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using UnityEditor;
using PixelH8.Data;

public class WaypointBuilderAddWindow : EditorWindow
{
    public WaypointBuilder waypointBuilder;
    public GameObject Waypoint;
    public GameObject NewWaypoint;
    void OnGUI()
    {
        GUILayout.Label("Selected Waypoint", EditorStyles.boldLabel);
        NewWaypoint = (GameObject)EditorGUILayout.ObjectField((Object)NewWaypoint, typeof(GameObject), true);

        if (GUILayout.Button("Done"))
        {
            waypointBuilder.WorkingWaypoint.GetComponent<Waypoint>().ConnectedWaypoints.Add(NewWaypoint.GetComponent<Waypoint>());
            NewWaypoint.GetComponent<Waypoint>().ConnectedWaypoints.Add(waypointBuilder.WorkingWaypoint.GetComponent<Waypoint>());

            this.Close();
        }
    }

    void OnSelectionChange()
    {
        if (Selection.activeGameObject.GetComponent<Waypoint>())
            NewWaypoint = Selection.activeGameObject;
    }
}
