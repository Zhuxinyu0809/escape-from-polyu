using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.Video;
using System.Linq;

public class MusicRoomEvent : MonoBehaviour, IEvent
{
    [Header("Event Objects")]
    public AudioSource musicSource;
    public VideoPlayer monitorVideo;
    public UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable stopButton;
    public Transform chairsContainer;

    [Header("Button Materials")]
    public Material musicPlayMaterial;
    public Material musicStopMaterial;

    [Header("After Event Settings")]
    public AudioSource voiceOverSource;

    public bool IsCompleted { get; private set; } = false;

    private SpinningChair[] allChairs;
    private bool isMusicPlaying = false;
    private bool hintPlayed = false;

    private Renderer stopButtonRenderer;

    void Start()
    {
        if (chairsContainer != null)
        {
            allChairs = chairsContainer.GetComponentsInChildren<SpinningChair>();
        }

        if (stopButton != null)
        {
            stopButton.selectEntered.AddListener(OnStopButtonSelected);
            stopButtonRenderer = stopButton.GetComponent<Renderer>();
            if (stopButtonRenderer != null)
            {
                stopButtonRenderer.material = musicStopMaterial;
            }
        }
    }

    void OnDestroy()
    {
        if (stopButton != null)
        {
            stopButton.selectEntered.RemoveListener(OnStopButtonSelected);
        }
    }

    public void StartEvent()
    {
        if (isMusicPlaying || IsCompleted) return;

        Debug.Log("音樂房間事件開始");
        ToggleMusic(true);
        ToggleSpin(true);
    }

    private void OnStopButtonSelected(SelectEnterEventArgs arg)
    {
        // 情況一：事件第一次被完成
        if (!IsCompleted)
        {
            Debug.Log("音樂房間事件完成");
            IsCompleted = true;
            // 停晒所有嘢
            ToggleMusic(false);
            ToggleSpin(false);

            // 播放提示音
            if (voiceOverSource != null && !hintPlayed)
            {
                voiceOverSource.Play();
                hintPlayed = true;
            }

            EventManager.instance.CheckAllEventsCompletion();
        }
        // 情況二：事件完成後，只有選擇播 / 停音樂
        else
        {
            ToggleMusic(!isMusicPlaying);
        }
    }

    private void ToggleMusic(bool play)
    {
        isMusicPlaying = play;
        if (play)
        {
            musicSource.Play();
            monitorVideo.Play();
            if (stopButtonRenderer != null)
            {
                stopButtonRenderer.material = musicPlayMaterial;
            }
        }
        else
        {
            musicSource.Pause();
            monitorVideo.Pause();
            if (stopButtonRenderer != null)
            {
                stopButtonRenderer.material = musicStopMaterial;
            }
        }
    }

    private void ToggleSpin(bool spin)
    {
        if (spin)
        {
            foreach (var chair in allChairs)
            {
                chair.StartSpin();
            }
        }
        else
        {
            foreach (var chair in allChairs)
            {
                chair.StopSpin();
            }
        }
    }
    
    public void ResetEvent()
    {
        IsCompleted = false;
        hintPlayed = false;
    }
}
