using UnityEngine;
using System.Collections;

/// <summary>
/// 掛喺一間房嘅門嘅父物件上。
/// 統一管理呢間房所有門嘅開啓 (使用 Hinge Joint)。
/// </summary>
public class RoomDoorController : MonoBehaviour
{
    [System.Serializable]
    public class DoorInfo
    {
        [Tooltip("門上面嘅 Hinge Joint 組件")]
        public HingeJoint doorHinge; 
        [Tooltip("呢隻門係左門定右門？")]
        public enum DoorSide { Left, Right }
        public DoorSide side;

        // ======== 新增內容：儲存 Rigidbody ========
        [HideInInspector] public Rigidbody doorRigidbody;
        // ===================================
    }

    [Header("呢間房嘅所有門")]
    public DoorInfo[] doorsInRoom;
    
    [Header("Hinge Joint 設定")]
    [Tooltip("開門嘅彈簧力度")]
    public float springForce = 100f;
    [Tooltip("彈簧嘅阻尼，防止震盪")]
    public float springDamper = 10f;

    private bool doorsAreOpen = false;

    void Start()
    {
        foreach (var door in doorsInRoom)
        {
            if (door.doorHinge == null)
            {
                Debug.LogError($"有名為 {door.doorHinge.gameObject.name} 嘅門未設置 Hinge Joint！", this);
                continue;
            }

            // ======== 新增內容：獲取並鎖定 Rigidbody ========
            // 1. 獲取 Rigidbody
            door.doorRigidbody = door.doorHinge.GetComponent<Rigidbody>();
            if (door.doorRigidbody == null)
            {
                Debug.LogError($"門 {door.doorHinge.gameObject.name} 缺少 Rigidbody！", this);
                continue;
            }
            
            // 2. 將門設置為 Kinematic (鎖定狀態)，咁就唔會被撞開
            door.doorRigidbody.isKinematic = true;
            // =============================================

            // (即使係 Kinematic 狀態，我哋依然可以預先設置好 Hinge Joint)
            JointSpring spring = door.doorHinge.spring;
            JointLimits limits = door.doorHinge.limits;

            spring.spring = springForce;
            spring.damper = springDamper;
            spring.targetPosition = 0; // 初始位置係關閉 (0度)
            door.doorHinge.spring = spring;
            door.doorHinge.useSpring = true;

            if (door.side == DoorInfo.DoorSide.Left)
            {
                limits.min = -90f;
                limits.max = 0f;
            }
            else
            {
                limits.min = 0f;
                limits.max = 90f;
            }
            door.doorHinge.limits = limits;
            door.doorHinge.useLimits = true;
        }
    }

    public void OpenAllDoors()
    {
        if (doorsAreOpen) return;
        doorsAreOpen = true;

        foreach (var door in doorsInRoom)
        {
            if (door.doorHinge == null || door.doorRigidbody == null) continue;

            // ======== 新增內容：解鎖 Rigidbody ========
            // 1. 解鎖！令門可以被物理引擎（同 Hinge Joint）控制
            door.doorRigidbody.isKinematic = false;
            // =============================================

            JointSpring spring = door.doorHinge.spring;

            // 2. 設置新嘅目標角度
            if (door.side == DoorInfo.DoorSide.Left)
            {
                spring.targetPosition = -90f; 
            }
            else
            {
                spring.targetPosition = 90f;
            }

            door.doorHinge.spring = spring;
        }
    }
}