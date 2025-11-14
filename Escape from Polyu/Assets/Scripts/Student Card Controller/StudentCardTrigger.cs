using UnityEngine;

public class StudentCardTrigger : MonoBehaviour
{
    public GameObject studentCard;
    public int timesToTrigger = 2;
    private bool hasTriggered = false;
    private int triggerCount = 0;

    void Start()
    {
        studentCard.SetActive(false);
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (!hasTriggered && other.CompareTag("Player"))
        {
            triggerCount++;
            if (triggerCount < timesToTrigger)
            {
                Debug.Log($"第 {triggerCount} 次觸發 StudentCardTrigger");
            }
            else
            {
                hasTriggered = true;
                studentCard.SetActive(true);
                Debug.Log($"第 {triggerCount} 次觸發 StudentCardTrigger， 學生卡跌落");
            }
        }
    }
}
