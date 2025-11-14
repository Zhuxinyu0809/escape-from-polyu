using UnityEngine;
using System.Collections;

public class CorridorLoopManager : MonoBehaviour
{
    [Header("Plyer")]
    public Transform xrOrigin;

    [Header("Anchor")]
    public Transform startSpaceAnchor;
    public Transform endSpaceAnchor;

    [Header("Doors")]
    public Transform startDoor1Left;
    public Transform startDoor1Right;
    public Transform startDoor2Left;
    public Transform startDoor2Right;
    public Transform endDoor1Left;
    public Transform endDoor1Right;
    public Transform endDoor2Left;
    public Transform endDoor2Right;

    [Header("Game Logic")]
    public bool conditionMet = false;

    private Quaternion startDoor1Left_CloseRotation;
    private Quaternion startDoor1Right_CloseRotation;
    private Quaternion startDoor2Left_CloseRotation;
    private Quaternion startDoor2Right_CloseRotation;
    private Quaternion endDoor1Left_CloseRotation;
    private Quaternion endDoor1Right_CloseRotation;
    private Quaternion endDoor2Left_OpenRotation;
    private Quaternion endDoor2Right_OpenRotation;

    private bool isTeleporting = false;
    private bool playerIsInEndSpace = false;

    void Start()
    {
        // 記錄門關閉時嘅初始 rotation
        startDoor1Left_CloseRotation = startDoor1Left.localRotation;
        startDoor1Right_CloseRotation = startDoor1Right.localRotation;
        startDoor2Left_CloseRotation = startDoor2Left.localRotation;
        startDoor2Right_CloseRotation = startDoor2Right.localRotation;
        endDoor1Left_CloseRotation = endDoor1Left.localRotation;
        endDoor1Right_CloseRotation = endDoor1Right.localRotation;

        // 設定 end door 2 開啓時嘅 rotation
        endDoor2Left_OpenRotation = endDoor2Left.localRotation * Quaternion.Euler(0, -90, 0);
        endDoor2Right_OpenRotation = endDoor2Right.localRotation * Quaternion.Euler(0, 90, 0);
    }

    public void PlayerEnteredStartSpace()
    {
        // 自動關閉 start door 1
        Debug.Log("玩家進入 start space, 關閉 start door 1");
        StartCoroutine(MoveDoor(startDoor1Left, startDoor1Left_CloseRotation));
        StartCoroutine(MoveDoor(startDoor1Right, startDoor1Right_CloseRotation));
    }

    public void PlayerEnteredCorridor()
    {
        // 自動關閉 start door 2
        Debug.Log("玩家進入 corridor, 關閉 start door 2");
        StartCoroutine(MoveDoor(startDoor2Left, startDoor2Left_CloseRotation));
        StartCoroutine(MoveDoor(startDoor2Right, startDoor2Right_CloseRotation));
    }

    public void PlayerEnteredEndSpace()
    {
        if (playerIsInEndSpace || isTeleporting) return; // 防止重複觸發

        playerIsInEndSpace = true;

        // 自動關閉 end door 1
        Debug.Log("玩家進入 end space, 關閉 end door 1");
        StartCoroutine(MoveDoor(endDoor1Left, endDoor1Left_CloseRotation));
        StartCoroutine(MoveDoor(endDoor1Right, endDoor1Right_CloseRotation));

        // 檢查條件
        if (!conditionMet)
        {
            Debug.Log("條件未完成，回到 start space");
            StartCoroutine(TeleportPlayer());
        }
        else
        {
            Debug.Log("條件已完成，逃離 polyu");
            StartCoroutine(MoveDoor(endDoor2Left, endDoor2Left_OpenRotation));
            StartCoroutine(MoveDoor(endDoor2Right, endDoor2Right_OpenRotation));
            // 停用 EndSpaceTrigger
            FindFirstObjectByType<ActionTrigger>()?.gameObject.SetActive(false);
        }
    }

    private IEnumerator TeleportPlayer()
    {
        isTeleporting = true;

        // 等待一幀
        yield return null;

        // 計算玩家相對於 end sapce anchor 嘅位置同旋轉
        Vector3 playerOffset = endSpaceAnchor.InverseTransformPoint(xrOrigin.position);
        Quaternion playerRotationOffset = Quaternion.Inverse(endSpaceAnchor.rotation) * xrOrigin.rotation;

        // 將呢個相對位置同旋轉應用喺 start space anchor
        xrOrigin.position = startSpaceAnchor.TransformPoint(playerOffset);
        xrOrigin.rotation = startSpaceAnchor.rotation * playerRotationOffset;

        Debug.Log("回到 start sapce");
        isTeleporting = false;
        playerIsInEndSpace = false;
    }

    private IEnumerator MoveDoor(Transform door, Quaternion targetRotation)
    {
        float duration = 1.0f; // 關門所需時間
        float elapsedTime = 0f;
        Quaternion startingRotation = door.localRotation;

        while (elapsedTime < duration)
        {
            door.localRotation = Quaternion.Slerp(startingRotation, targetRotation, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        door.localRotation = targetRotation;
    }
}
