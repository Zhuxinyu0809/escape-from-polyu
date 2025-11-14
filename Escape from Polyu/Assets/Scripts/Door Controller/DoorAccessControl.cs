using UnityEngine;

/// <summary>
/// 掛喺門禁嘅 Trigger Collider 物件上。
/// 負責檢測學生卡並通知 RoomDoorController 開門。
/// </summary>
[RequireComponent(typeof(Collider))]
public class DoorAccessControl : MonoBehaviour
{
    [Header("控制邊間房嘅門？")]
    [Tooltip("將呢間房嘅 RoomDoorController 腳本拖入嚟")]
    public RoomDoorController roomController;

    private bool isActivated = false; // 用呢個旗標嚟令門禁失效

    void Start()
    {
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            col.isTrigger = true;
        }
        else
        {
            Debug.LogError("DoorAccessControl 缺少 Collider!", this);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // 如果已經啟動過，就直接返回，唔再做任何嘢
        // 呢個就係「失效但保持可見」嘅邏輯
        if (isActivated) return;
        
        StudentCard card = other.GetComponent<StudentCard>();
        if (card == null)
        {
            card = other.GetComponentInParent<StudentCard>();
        }

        if (card != null)
        {
            Debug.Log("【門禁】檢測到學生卡！正在開門...");
            isActivated = true; // 標記為已啟動
            
            if (roomController != null)
            {
                roomController.OpenAllDoors();
            }
            else
            {
                Debug.LogError("門禁未連接到 RoomDoorController!", this);
            }

            // 我哋唔再用 SetActive(false)，門禁會保持可見但冇功能
        }
    }
}