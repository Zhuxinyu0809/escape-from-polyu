using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class EventManager : MonoBehaviour
{
    public static EventManager instance;

    public CorridorLoopManager corridorLoopManager;
    private List<IEvent> allEvents; // 一個列表，裝住所有事件

    void Awake()
    {
        // 設置 Singleton
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // 自動喺場景中揾出所有實現咗 IEvent 嘅組件
        allEvents = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None).OfType<IEvent>().ToList();
        Debug.Log($"揾到 {allEvents.Count} 個事件");
    }

    // 畀任何事件完成後調用
    public void CheckAllEventsCompletion()
    {
        bool allDone = allEvents.All(e => e.IsCompleted);

        if (allDone)
        {
            Debug.Log("所有事件已完成！可以逃離 PolyU!");
            corridorLoopManager.conditionMet = true;
        }
        else
        {
            Debug.Log($"仍有 {allEvents.Count(e => !e.IsCompleted)} 個事件未完成");
        }
    }
}
