using UnityEngine;

public class ActionTrigger : MonoBehaviour
{
    public enum TriggerAction { EnterStartSpace, EnterCorridor, EnterEndSpace }
    public TriggerAction actionToPerform;

    private CorridorLoopManager loopManager;

    void Start()
    {
        loopManager = FindFirstObjectByType<CorridorLoopManager>();
        if (loopManager == null)
        {
            Debug.LogError("缺少 CorridorLoopManager Script");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // 如果已經觸發過，或者進入嘅唔係玩家，就直接返回
        if (!other.CompareTag("Player")) return;

        if (loopManager != null)
        {
            switch (actionToPerform)
            {
                case TriggerAction.EnterStartSpace:
                    loopManager.PlayerEnteredStartSpace();
                    break;
                case TriggerAction.EnterCorridor:
                    loopManager.PlayerEnteredCorridor();
                    break;
                case TriggerAction.EnterEndSpace:
                    loopManager.PlayerEnteredEndSpace();
                    break;
            }
        }
    }
}
