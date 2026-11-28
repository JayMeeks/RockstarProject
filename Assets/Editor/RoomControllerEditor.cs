using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using JetBrains.Annotations;

[CustomEditor(typeof(RoomController))]
public class RoomControllerEditor : Editor
{
    public VisualTreeAsset visualTree;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        // Equivalent to Header
        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("Spawn Locations", EditorStyles.boldLabel);

        RoomController room = (RoomController)target;

        // Adds a button to RoomController scripts that creates a new SpawnLocation marker in editor
        if (GUILayout.Button("Add Spawn Location"))
        {
            GameObject marker = new GameObject("Spawn Location");

            Undo.RegisterCreatedObjectUndo(marker, "Add Spawn Location");

            Undo.SetTransformParent(marker.transform, room.transform, "Parent Spawn Location");

            marker.transform.localPosition = Vector3.zero;
            marker.transform.localRotation = Quaternion.identity;

            Undo.AddComponent<LocationMarker>(marker);

            MeshFilter meshFilter = Undo.AddComponent<MeshFilter>(marker);
            MeshRenderer meshRenderer = Undo.AddComponent<MeshRenderer>(marker);

            meshFilter.sharedMesh = Resources.GetBuiltinResource<Mesh>("Cylinder.fbx");

            marker.transform.localScale = new Vector3(1f, 0.1f, 1f);

            Material material = AssetDatabase.LoadAssetAtPath<Material>("Assets/Models/Texture/New Material.mat");

            meshRenderer.sharedMaterial = material;

            Selection.activeGameObject = marker;
        }

        // Displays all current LocationMarkers attached to this room and their locations
        LocationMarker[] markers = room.GetComponentsInChildren<LocationMarker>();

        for (int i = 0; i < markers.Length; i++)
        {
            EditorGUILayout.BeginHorizontal();

            // When clicking on the marker, it takes you to that marker in editor so you can move it
            if (GUILayout.Button(markers[i].LocationName, EditorStyles.linkLabel))
            {
                Selection.activeGameObject = markers[i].gameObject;
                EditorGUIUtility.PingObject(markers[i].gameObject);
            }

            EditorGUILayout.LabelField(markers[i].transform.position.ToString("F2"));

            EditorGUILayout.EndHorizontal();
        }
    }
}
