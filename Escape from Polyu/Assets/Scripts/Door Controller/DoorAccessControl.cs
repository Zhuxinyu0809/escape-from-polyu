using UnityEngine;

[RequireComponent(typeof(Collider))]
public class DoorAccessControl : MonoBehaviour
{
    [Header("Room Door Controller")]
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
        if (isActivated) return;
        
        StudentCard card = other.GetComponent<StudentCard>();
        if (card == null)
        {
            card = other.GetComponentInParent<StudentCard>();
        }

        if (card != null)
        {
            Debug.Log("門禁檢測到學生卡，正在開門...");
            isActivated = true; // 標記為已啟動
            
            if (roomController != null)
            {
                roomController.OpenAllDoors();
            }
            else
            {
                Debug.LogError("門禁未連接到 RoomDoorController!", this);
            }
        }
    }
}