using UnityEditor;
using UnityEngine;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

[CustomEditor(typeof(RoomWaveController))]
public class RoomWaveControllerEditor : Editor
{
    public VisualTreeAsset VisualTree;

    public override VisualElement CreateInspectorGUI()
    {
        VisualElement root = new VisualElement();

        VisualTree.CloneTree(root);

        return root;
    }
}
