using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.Video;

public class SnowNoiseEvent : MonoBehaviour, IEvent
{
    [Header("Event Objects")]
    public VideoPlayer tvVideo;
    public AudioSource tvNoise;
    public UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable tvScreenInteractable;

    public bool IsCompleted { get; private set; } = false;

    void Start()
    {
        // 監聽電視屏幕係咪被「點擊」
        if (tvScreenInteractable != null)
        {
            tvScreenInteractable.selectEntered.AddListener(OnTVScreenClicked);
        }
    }
    
    void OnDestroy()
    {
        if (tvScreenInteractable != null)
        {
            tvScreenInteractable.selectEntered.RemoveListener(OnTVScreenClicked);
        }
    }

    public void StartEvent()
    {
        if (IsCompleted) return;
        tvVideo.Play();
        tvNoise.Play();
        // 確保電視一開始係唔可以點擊嘅
        tvScreenInteractable.enabled = false;
    }

    public void ResetEvent() 
    {
        IsCompleted = false;
        tvScreenInteractable.enabled = false;
    }
        
    // 當遙控器組裝好
    public void OnRemoteAssembled()
    {
        if (IsCompleted) return;
        // 啟用電視屏幕嘅「可點擊」功能
        tvScreenInteractable.enabled = true;
    }

    // 當遙控器散咗
    public void OnRemoteDisassembled()
    {
        tvScreenInteractable.enabled = false;
    }

    public void ResolveEvent()
    {
        if (IsCompleted) return;

        Debug.Log("雪花噪音事件完成");
        IsCompleted = true;
        tvVideo.Stop();
        tvNoise.Stop();
        tvScreenInteractable.enabled = false;

        EventManager.instance.CheckAllEventsCompletion();
    }

    // 當玩家用遙控器 (X-Ray) 點擊電視屏幕
    private void OnTVScreenClicked(SelectEnterEventArgs arg)
    {
        ResolveEvent();
    }
}