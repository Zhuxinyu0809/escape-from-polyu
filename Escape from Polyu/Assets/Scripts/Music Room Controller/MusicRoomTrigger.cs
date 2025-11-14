using UnityEngine;

public class MusicRoomTrigger : MonoBehaviour
{
    private bool hasTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!hasTriggered && other.CompareTag("Player"))
        {
            hasTriggered = true;
            MusicRoomEvent musicEvent = FindFirstObjectByType<MusicRoomEvent>();
            musicEvent.StartEvent();
        }
    }
}
