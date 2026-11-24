using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

/// <summary>
/// Temporary debug helper. Add to any active GameObject.
/// On every left click it logs what the UI raycast hit, the cursor state and the EventSystem count.
/// Delete it once the click problem is solved.
/// </summary>
public class UIClickDebugger : MonoBehaviour
{
    private readonly List<RaycastResult> results = new List<RaycastResult>();

    private void Update()
    {
        if (Mouse.current == null || !Mouse.current.leftButton.wasPressedThisFrame) return;

        var es = EventSystem.current;
        if (es == null)
        {
            Debug.LogWarning("[UIClickDebugger] No EventSystem.current");
            return;
        }

        var data = new PointerEventData(es) { position = Mouse.current.position.ReadValue() };
        results.Clear();
        es.RaycastAll(data, results);

        string top = results.Count > 0 ? results[0].gameObject.name : "NOTHING";
        Debug.Log(
            $"[UIClickDebugger] Click at {data.position} | top hit: {top} | total hits: {results.Count} | " +
            $"cursor lock: {Cursor.lockState} | visible: {Cursor.visible} | paused: {PauseMenu.GetIsPaused()} | " +
            $"EventSystems in scene: {FindObjectsByType<EventSystem>(FindObjectsSortMode.None).Length} | " +
            $"module: {es.currentInputModule?.GetType().Name ?? "none"}");

        for (int i = 0; i < results.Count; i++)
            Debug.Log($"   hit {i}: {results[i].gameObject.name}", results[i].gameObject);
    }
}