using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CustomGraphicRaycaster : GraphicRaycaster
{
    //This script exists only for in world menus
    //When the cursor is disabled, Unity defaults to placing its position somewhere outside the screen bounds
    //For in world buttons, this position is overwritten to be the center of the screen where the player crosshair is
    public override void Raycast(PointerEventData eventData, List<RaycastResult> resultAppendList)
    {
        eventData.position = new(Screen.width / 2, Screen.height / 2);
        base.Raycast(eventData, resultAppendList);
    }
}
