using UnityEngine;
using System.Collections;

public class LightFlickerEvent : MonoBehaviour, IEvent
{
    [Header("Event Object")]
    public Light[] corridorLights;

    [Header("Flicker Settings")]
    public float minFlickerSpeed = 0.05f;
    public float maxFlickerSpeed = 0.3f;

    public bool IsCompleted { get; private set; } = false;
    private Coroutine flickerCoroutine;

    public void StartEvent()
    {
        if (IsCompleted || flickerCoroutine != null) return;
        Debug.Log("閃燈事件開始");
        flickerCoroutine = StartCoroutine(FlickerLights());
    }

    // 畀 BreakerBox 使用，用來強制開關燈
    public void SetLights(bool isOn)
    {
        // 如果燈喺度閃，先停止
        if (flickerCoroutine != null)
        {
            StopCoroutine(flickerCoroutine);
            flickerCoroutine = null;
        }

        foreach (var light in corridorLights)
        {
            light.enabled = isOn;
        }
        Debug.Log($"所有走廊燈已經被設定為: {(isOn ? "開啓" : "關閉")}");
    }

    public void ResolveEvent()
    {
        if (IsCompleted) return;
        Debug.Log("閃燈事件解決");

        // 確保燈光係正常開啓嘅
        SetLights(true);
        IsCompleted = true;
        EventManager.instance.CheckAllEventsCompletion();
    }

    public void ResetEvent()
    {
        Debug.Log("閃燈事件重置");
    }

    private IEnumerator FlickerLights()
    {
        while (!IsCompleted)
        {
            foreach (var light in corridorLights)
            {
                light.enabled = Random.value > 0.5f;
            }
            yield return new WaitForSeconds(Random.Range(minFlickerSpeed, maxFlickerSpeed));
        }
    }
}
