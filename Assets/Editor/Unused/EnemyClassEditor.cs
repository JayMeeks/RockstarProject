using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(EnemyClass))]
public class EnemyClassDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        property.isExpanded = EditorGUI.Foldout(new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight), property.isExpanded, label);

        if (property.isExpanded)
        {
            EditorGUI.indentLevel++;

            float y = position.y + EditorGUIUtility.singleLineHeight + 2;

            SerializedProperty spawnLocation = property.FindPropertyRelative("spawnLocation");

            SerializedProperty enemyType = property.FindPropertyRelative("enemyType");

            RoomController room = property.serializedObject.targetObject as RoomController;

            if (room != null)
            {
                LocationMarker[] markers = room.GetComponentsInChildren<LocationMarker>(true);

                string[] options = new string[markers.Length + 1];
                options[0] = "None";

                for (int i = 0; i < markers.Length; i++)
                {
                    options[i + 1] = markers[i].LocationName;
                }

                int currentIndex = System.Array.IndexOf(markers, spawnLocation.objectReferenceValue) + 1;

                int selectedIndex = EditorGUI.Popup(new Rect(position.x, y, position.width, EditorGUIUtility.singleLineHeight), "Spawn Location", currentIndex, options);

                if (selectedIndex != currentIndex)
                {
                    spawnLocation.objectReferenceValue = selectedIndex == 0 ? null : markers[selectedIndex - 1];
                }
            }

            y += EditorGUIUtility.singleLineHeight + 2;

            EditorGUI.PropertyField(new Rect(position.x, y, position.width, EditorGUIUtility.singleLineHeight), enemyType);

            EditorGUI.indentLevel--;
        }

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (!property.isExpanded)
        {
            return EditorGUIUtility.singleLineHeight;
        }

        return EditorGUIUtility.singleLineHeight * 3 + 6;
    }
}