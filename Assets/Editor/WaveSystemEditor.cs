using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(WaveSystem))]

public class WaveSystemEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        WaveSystem waveSystem = (WaveSystem)target;

        if (GUILayout.Button("Add Room Controller"))
        {
            GameObject roomController = new GameObject("Room Controller");

            Undo.RegisterCreatedObjectUndo(roomController, "Add Spawn Location");

            Undo.SetTransformParent(roomController.transform, waveSystem.transform, "Parent Spawn Location");

            roomController.transform.localPosition = Vector3.zero;
            roomController.transform.localRotation = Quaternion.identity;

            Undo.AddComponent<RoomController>(roomController);

            Selection.activeGameObject = roomController;
        }
    }
}
