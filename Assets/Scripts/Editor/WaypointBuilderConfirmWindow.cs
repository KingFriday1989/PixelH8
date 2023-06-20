using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class WaypointBuilderConfermWindow : EditorWindow
{
    public WaypointBuilder waypointBuilder;
    public GameObject Waypoint;
    void OnGUI()
    {
        var newName = EditorGUILayout.TextField("New Name",Waypoint.name);
        if(Waypoint.name != newName)
            Waypoint.name = newName;

        if(GUILayout.Button("Done"))
            this.Close();
    }
}