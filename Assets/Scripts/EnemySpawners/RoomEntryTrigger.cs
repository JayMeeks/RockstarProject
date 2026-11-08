using UnityEngine;

public class RoomEntryTrigger : MonoBehaviour
{
    [SerializeField] RoomController roomController;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            roomController.OnRoomEnter();
            enabled = false;
        }
    }
}
