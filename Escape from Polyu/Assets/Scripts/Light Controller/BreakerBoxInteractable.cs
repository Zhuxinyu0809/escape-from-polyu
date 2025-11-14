using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class BreakerBoxInteractable : MonoBehaviour
{
    // 定義配電箱嘅所有狀態
    private enum BreakerBoxState
    {
        Closed,         // 初始狀態，𢫏蓋
        Opened,         // 蓋被打開
        LightsOff,      // 燈已關閉
        LightsRestored, // 燈已恢復，事件完成
        Locked
    }

    [Header("BreakerBox Models")]
    public GameObject breakerBoxClosedModel;
    public GameObject breakerBoxOpenedModel;

    [Header("Light Flicker Event Reference")]
    public LightFlickerEvent lightEvent;

    private BreakerBoxState currentState = BreakerBoxState.Closed;
    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable interactable;

    void Awake()
    {
        // 獲取 XR 互動組件
        interactable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable>();
    }

    void OnEnable()
    {
        // 監聽 "selectEntered" 事件
        interactable.selectEntered.AddListener(OnBoxClicked);
    }

    void OnDisable()
    {
        // 移除監聽
        interactable.selectEntered.RemoveListener(OnBoxClicked);
    }

    // 當玩家點擊配電箱時，呢個方法會被調用
    private void OnBoxClicked(SelectEnterEventArgs arg)
    {
        Debug.Log($"配電箱被點擊，當前狀態: {currentState}");

        // 根據當前狀態，決定下一步做咩
        switch (currentState)
        {
            case BreakerBoxState.Closed:
                // 1. 打開配電箱蓋
                breakerBoxOpenedModel.SetActive(true);
                breakerBoxClosedModel.SetActive(false);
                currentState = BreakerBoxState.Opened;
                break;

            case BreakerBoxState.Opened:
                // 2. 關閉走廊燈
                lightEvent.SetLights(false);
                currentState = BreakerBoxState.LightsOff;
                break;

            case BreakerBoxState.LightsOff:
                // 3. 恢復走廊燈並解決事件
                lightEvent.ResolveEvent();
                currentState = BreakerBoxState.LightsRestored;
                break;

            case BreakerBoxState.LightsRestored:
                // 4. 𢫏蓋，進入鎖定狀態
                breakerBoxOpenedModel.SetActive(false);
                breakerBoxClosedModel.SetActive(true);
                currentState = BreakerBoxState.Locked;
                Debug.Log("事件完成，配電箱鎖定");
                lightEvent.ResolveEvent();
                break;

            case BreakerBoxState.Locked:
                // 如果已經鎖定，就咩都唔做
                break;
        }
    }
}
