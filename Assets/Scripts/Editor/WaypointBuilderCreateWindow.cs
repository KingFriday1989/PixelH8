using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using UnityEditor;
using PixelH8.Data;

public class WaypointBuilderCreateWindow : EditorWindow
{
    public WaypointBuilder waypointBuilder;
    public GameObject NewWaypoint;
    public Waypoint.WaypointType waypointType;
    public string NewName = "Waypoint";
    public float radius;
    public float waitTime;

    void OnGUI()
    {
        GUILayout.Label("Waypoint Name", EditorStyles.boldLabel);
        NewName = EditorGUILayout.TextField(NewName);
        GUILayout.Label("Waypoint Type", EditorStyles.boldLabel);
        waypointType = (Waypoint.WaypointType)EditorGUILayout.EnumPopup(waypointType);
        GUILayout.Label("Waypoint Complete Radius", EditorStyles.boldLabel);
        radius = EditorGUILayout.FloatField(radius);
        GUILayout.Label("Waypoint Wait Time", EditorStyles.boldLabel);
        waitTime = EditorGUILayout.FloatField(waitTime);

        if (GUILayout.Button("Done"))
        {
            NewWaypoint = Instantiate(ObjectsAndData.Instance.constants.waypoint, Vector3.zero, Quaternion.identity);
            NewWaypoint.transform.parent = waypointBuilder.RootWaypoint.transform;
            NewWaypoint.name = NewName;
            var WaypointScript = NewWaypoint.GetComponent<Waypoint>();
            WaypointScript.waypointType = waypointType;
            WaypointScript.radius = radius;
            WaypointScript.waitTime = waitTime;


            waypointBuilder.WorkingWaypoint.GetComponent<Waypoint>().ConnectedWaypoints.Add(this.NewWaypoint.GetComponent<Waypoint>());
            this.NewWaypoint.GetComponent<Waypoint>().ConnectedWaypoints.Add(waypointBuilder.WorkingWaypoint.GetComponent<Waypoint>());
            Selection.activeGameObject = NewWaypoint;
            this.Close();
        }
    }

}
