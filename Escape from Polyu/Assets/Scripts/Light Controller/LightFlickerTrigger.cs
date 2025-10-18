using UnityEngine;

public class LightFlickerTrigger : MonoBehaviour
{
    private bool hasTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!hasTriggered && other.CompareTag("Player"))
        {
            hasTriggered = true;
            LightFlickerEvent lightEvent = FindFirstObjectByType<LightFlickerEvent>();
            lightEvent.StartEvent();
        }
    }
}
