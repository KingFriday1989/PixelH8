#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using UnityEditor;
using PixelH8.Data;

public class WaypointBuilder : EditorWindow
{
    [SerializeField] private int m_SelectedIndex = -1;
    private VisualElement m_RightPane;
    Transform LevelTransform;
    public GameObject RootWaypoint;
    public GameObject WorkingWaypoint;
    public GameObject SelectedWaypoint;
    SerializedObject serializedObject;
    SerializedProperty serializedProperty;

    string WaypointName = "Waypoint";
    [SerializeField] List<GameObject> ConnectedWaypoints = new List<GameObject>();


    [MenuItem("PixelH8/Waypoint Builder")]

    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(WaypointBuilder));
    }

    void OnEnable()
    {
        if (RootWaypoint == null)
        {
            RootWaypoint = GameObject.Find("Level").transform.Find("Waypoints").Find("Root").gameObject;

            if (RootWaypoint == null)
                return;
            RefreshConnectedWaypoints();
        }
    }

    void OnGUI()
    {
        if (RootWaypoint == null)
        {
            RootWaypoint = (GameObject)EditorGUILayout.ObjectField((Object)RootWaypoint, typeof(GameObject), true);
            if (GUILayout.Button("Create New Root?"))
            {
                RootWaypoint = Instantiate(ObjectsAndData.Instance.constants.waypoint, Vector3.zero, Quaternion.identity);
                RootWaypoint.transform.parent = GameObject.Find("Level").transform.Find("Waypoints");
                RootWaypoint.name = "Root";
                WaypointName = "Root";
                RootWaypoint.GetComponent<Waypoint>().waypointType = Waypoint.WaypointType.NotAccessAble;
                WorkingWaypoint = RootWaypoint;
            }
        }
        else
        {
            GUILayout.Label("Current Waypoint Name", EditorStyles.boldLabel);
            //Rename
            var text = EditorGUILayout.TextField("Current Waypoint Name", WaypointName);
            if (WaypointName != text)
            {
                WaypointName = text;
                WorkingWaypoint.GetComponent<Waypoint>().name = text;
            }
            //Select Base
            if(SelectedWaypoint != WorkingWaypoint && GUILayout.Button("Select Current Waypoint"))
            {
                SelectedWaypoint = WorkingWaypoint;
                Selection.activeGameObject = WorkingWaypoint;
            }
            //Create
            if (GUILayout.Button("Create New Waypoint"))
            {
                var newWindow = GetWindow<WaypointBuilderCreateWindow>();
                newWindow.waypointBuilder = this;
            }
            //Add
            if (SelectedWaypoint != WorkingWaypoint && GUILayout.Button("Add Connection"))
            {
                var newWindow = GetWindow<WaypointBuilderAddWindow>();
                newWindow.waypointBuilder = this;
                newWindow.Waypoint = SelectedWaypoint;
            }

            if (SelectedWaypoint != null)
            {
                GUILayout.Label("Edit Selected Waypoint attributes", EditorStyles.boldLabel);
                //ChangeAtributes
                if (GUILayout.Button("Change Selected Attributes"))
                {
                    var newWindow = GetWindow<WaypointBuilderChangeAtributesWindow>();
                    newWindow.waypointBuilder = this;
                }
                //Delete Connection
                if (SelectedWaypoint != WorkingWaypoint && GUILayout.Button("Delete Connection"))
                {
                    WorkingWaypoint.GetComponent<Waypoint>().ConnectedWaypoints.Remove(SelectedWaypoint.GetComponent<Waypoint>());
                    SelectedWaypoint.GetComponent<Waypoint>().ConnectedWaypoints.Remove(WorkingWaypoint.GetComponent<Waypoint>());
                    SelectedWaypoint = null;
                }
                if (SelectedWaypoint != WorkingWaypoint && GUILayout.Button("Delete Waypoint"))
                {
                    WorkingWaypoint.GetComponent<Waypoint>().ConnectedWaypoints.Remove(SelectedWaypoint.GetComponent<Waypoint>());
                    Destroy(SelectedWaypoint.gameObject); 
                    SelectedWaypoint = null;
                }
                //GoTo
                if (SelectedWaypoint != WorkingWaypoint && GUILayout.Button("Goto Connection"))
                {
                    WorkingWaypoint = SelectedWaypoint;
                    SelectedWaypoint = null;
                    WaypointName = WorkingWaypoint.name;
                    RefreshConnectedWaypoints();
                }
            }

            RefreshConnectedWaypoints();
            if (ConnectedWaypoints.Count != 0)
            {
                GUILayout.Label("Connected Waypoints", EditorStyles.boldLabel);
                ScriptableObject target = this;
                SerializedObject so = new SerializedObject(target);
                SerializedProperty stringsProperty = so.FindProperty("ConnectedWaypoints");

                EditorGUILayout.PropertyField(stringsProperty, true);
                so.ApplyModifiedProperties();
            }
        }
    }

    void OnSelectionChange()
    {
        if (Selection.activeGameObject.GetComponent<Waypoint>())
            SelectedWaypoint = Selection.activeGameObject;
    }

    void RefreshConnectedWaypoints()
    {
        ConnectedWaypoints.Clear();

        var list = WorkingWaypoint.GetComponent<Waypoint>().ConnectedWaypoints;
        foreach (var item in list)
        {
            ConnectedWaypoints.Add(item.gameObject);
        }
    }
}

#endif