using UnityEngine;

public class SnowNoiseTrigger : MonoBehaviour
{
    private bool hasTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!hasTriggered && other.CompareTag("Player"))
        {
            hasTriggered = true;
            SnowNoiseEvent snowNoiseEvent = FindFirstObjectByType<SnowNoiseEvent>();
            snowNoiseEvent.StartEvent();
        }
    }
}
