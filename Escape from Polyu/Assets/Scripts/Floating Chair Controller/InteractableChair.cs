using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using System.Collections;

[RequireComponent(typeof(UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable), typeof(BoxCollider))]
public class InteractableChair : MonoBehaviour
{
    [Header("Float Settings")]
    public float floatHeight = 1.5f;
    public float floatRadius = 1.4f;
    public float moveSpeed = 1.0f;

    private FloatingChairEvent eventManager;
    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable interactable;

    private Vector3 groundedPosition;
    private Quaternion groundedRotation;

    private Vector3 floatingPosition;
    private Quaternion floatingRotation;

    private bool isGrounded = false;

    void Awake()
    {
        // 獲取組件
        interactable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable>();

        // 儲存落地位置
        groundedPosition = transform.position;
        groundedRotation = transform.rotation;

        // 計算一個隨機嘅飄浮狀態
        Vector3 randomOffset = Random.insideUnitSphere * floatRadius;
        floatingPosition = groundedPosition + (Vector3.up * floatHeight) + randomOffset;
        floatingRotation = Random.rotation;
    }

    void Start()
    {
        // 喺遊戲一開始，櫈就進入飄浮狀態
        transform.position = floatingPosition;
        transform.rotation = floatingRotation;

        eventManager = FindFirstObjectByType<FloatingChairEvent>();
    }

    void OnEnable()
    {
        interactable.selectEntered.AddListener(OnChairClicked);
    }

    void OnDisable()
    {
        interactable.selectEntered.RemoveListener(OnChairClicked);
    }

    private void OnChairClicked(SelectEnterEventArgs arg)
    {
        // 如果已經落地或者喺度飛緊，就唔做任何嘢
        if (isGrounded) return;

        Debug.Log("櫈被點擊，飛返地面");
        StartCoroutine(MoveToGround());
    }

    private IEnumerator MoveToGround()
    {
        // 標記「已經落地」
        isGrounded = true;
        interactable.enabled = false;

        Vector3 startPos = transform.position;
        Quaternion startRot = transform.rotation;
        float time = 0;

        // 用 Lerp 同 Slerp 平滑咁飛返去
        while (time < 1.0f)
        {
            transform.position = Vector3.Lerp(startPos, groundedPosition, time);
            transform.rotation = Quaternion.Slerp(startRot, groundedRotation, time);
            time += Time.deltaTime * moveSpeed;
            yield return null;
        }

        // Make sure 最終嘅 position 同 rotation 完全準確
        transform.position = groundedPosition;
        transform.rotation = groundedRotation;

        eventManager?.OnChairGrounded();
    }

    public bool IsChairGrounded()
    {
        return isGrounded;
    }
}
