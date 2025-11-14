using UnityEngine;

public class MottoQuizTrigger : MonoBehaviour
{
    private bool hasTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!hasTriggered && other.CompareTag("Player"))
        {
            hasTriggered = true;
            MottoQuizEvent mottoQuizEvent = FindFirstObjectByType<MottoQuizEvent>();
            mottoQuizEvent.StartEvent();
        }
    }
}
