using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using TMPro;

public class FloatingChairEvent : MonoBehaviour, IEvent
{
    [Header("Event Objects")]
    public Transform chairsContainer;

    [Header("Whiteboard Settings")]
    public TextMeshPro whiteboardText;
    public string completionMessage = "Class Started!";

    // 櫈仔列表
    private List<InteractableChair> allChairs;

    public bool IsCompleted { get; private set; } = false;

    void Start()
    {
        allChairs = new List<InteractableChair>();
        if (chairsContainer != null)
        {
            allChairs = chairsContainer.GetComponentsInChildren<InteractableChair>().ToList();
        }

        if (allChairs.Count == 0)
        {
            Debug.LogWarning("冇揾到任何櫈仔");
        }
        else
        {
            Debug.Log($"一共需要擺平 {allChairs.Count} 張櫈仔");
        }

        whiteboardText.text = "Something Wrong...";
    }

    // 由於呢個事件係被動嘅，所以 StartEvent 唔使做嘢
    public void StartEvent() { }

    // 重置事件時做，暫時唔使用
    public void ResetEvent()
    {
        IsCompleted = false;
    }
    
    // 呢個方法喺每一張櫈飛完落地之後調用
    public void OnChairGrounded()
    {
        // 如果事件已經解決咗，就唔使檢查
        if (IsCompleted) return;

        // 檢查係咪所有嘅等都已經落地
        bool allGrounded = allChairs.All(chair => chair.IsChairGrounded());

        if (allGrounded)
        {
            Debug.Log("所有櫈仔都已經落地，事件完成");
            IsCompleted = true;

            // 事件完成時，更新白板內容
            whiteboardText.text = completionMessage;

            EventManager.instance.CheckAllEventsCompletion();
        }
        else
        {
            int remaining = allChairs.Count(chair => !chair.IsChairGrounded());
            Debug.Log($"仲有 {remaining} 張櫈未落地");
        }
    }
}
