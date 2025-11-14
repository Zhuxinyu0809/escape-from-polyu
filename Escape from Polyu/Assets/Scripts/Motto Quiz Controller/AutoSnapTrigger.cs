using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(Collider))]
public class AutoSnapTrigger : MonoBehaviour
{
    [Header("ID of the Socket")]
    public string correctCharacterID = "Hoi";

    private XRSocketInteractor targetSocket;
    private XRInteractionManager interactionManager;

    void Start()
    {
        // 確保 Collider 係 Trigger
        GetComponent<Collider>().isTrigger = true;

        // 自動搵佢嘅父物件 Socket
        targetSocket = GetComponentInParent<XRSocketInteractor>();
        if (targetSocket == null) {
            Debug.LogError("AutoSnapTrigger 搵唔到父物件嘅 XRSocketInteractor", this);
        }
        
        // 搵場景中嘅 Interaction Manager
        interactionManager = FindFirstObjectByType<XRInteractionManager>();
        if (interactionManager == null) {
            Debug.LogError("AutoSnapTrigger 搵唔到 XRInteractionManager", this);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // 如果 Socket 已經滿咗，就唔做嘢
        if (targetSocket == null || targetSocket.hasSelection) return;

        // 檢查入嚟嘅係咪 MottoPiece
        MottoPiece piece = other.GetComponent<MottoPiece>();
        if (piece == null) return;

        // 檢查 ID 係咪正確
        if (piece.characterID == correctCharacterID)
        {
            // 檢查件嘢係咪 Grabbable
            XRGrabInteractable grabInteractable = piece.GetComponent<XRGrabInteractable>();
            if (grabInteractable == null) return;

            // 檢查件嘢係咪已經被玩家揸住 (如果揸住就唔搶)
            if (grabInteractable.isSelected)
            {
                return;
            }
            
            // 執行手動吸附
            Debug.Log($"AutoSnap: 檢測到 {piece.characterID}，嘗試吸附到 {targetSocket.name}");
            interactionManager.SelectEnter((IXRSelectInteractor)targetSocket, (IXRSelectInteractable)grabInteractable);
        }
    }
}