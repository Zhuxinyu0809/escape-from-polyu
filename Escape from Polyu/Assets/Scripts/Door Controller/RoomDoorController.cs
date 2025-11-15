using UnityEngine;
using System.Collections;

public class RoomDoorController : MonoBehaviour
{
    [System.Serializable]
    public class DoorInfo
    {
        public HingeJoint doorHinge; 
        public enum DoorSide { Left, Right }
        public DoorSide side;

        [HideInInspector] public Rigidbody doorRigidbody;
    }

    [Header("Doors")]
    public DoorInfo[] doorsInRoom;
    
    [Header("Hinge Joint Settings")]
    public float springForce = 100f;
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

            door.doorRigidbody = door.doorHinge.GetComponent<Rigidbody>();
            door.doorRigidbody.isKinematic = true;

            JointSpring spring = door.doorHinge.spring;
            JointLimits limits = door.doorHinge.limits;

            spring.spring = springForce;
            spring.damper = springDamper;
            spring.targetPosition = 0; // 初始位置係關閉
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

            // 解鎖，令門可以被物理引擎同 Hinge Joint控制
            door.doorRigidbody.isKinematic = false;

            JointSpring spring = door.doorHinge.spring;

            // 設置新嘅目標角度
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