using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class AssembledRemote : MonoBehaviour
{
    [Header("Event Manager")]
    public SnowNoiseEvent eventManager;

    [Header("Sockets")]
    public XRSocketInteractor batterySocket1;
    public XRSocketInteractor batterySocket2;
    public XRSocketInteractor coverSocket;

    [Header("Interaction System")]
    public XRInteractionManager interactionManager;

    [Header("Remote Settings")]
    public Transform rayOrigin;
    public float maxDistance = 5.0f;
    public LayerMask tvLayerMask = ~0;

    private bool battery1In = false;
    private bool battery2In = false;
    private bool coverIn = false;
    private XRGrabInteractable grabInteractable;

    void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
    }
    
    void Start()
    {
        // 監聽 Socket 係咪有嘢插咗入嚟
        batterySocket1.selectEntered.AddListener(OnBattery1Placed);
        batterySocket2.selectEntered.AddListener(OnBattery2Placed);
        coverSocket.selectEntered.AddListener(OnCoverPlaced);

        // 監聽 Socket 嘅嘢係咪被拎走咗
        batterySocket1.selectExited.AddListener(OnBattery1Removed);
        batterySocket2.selectExited.AddListener(OnBattery2Removed);
        coverSocket.selectExited.AddListener(OnCoverRemoved);

        // 監聽遙控器係咪被激活（撳扳機)
        grabInteractable.activated.AddListener(OnTriggerPulled);
    }

    void OnDestroy()
    {
        batterySocket1.selectEntered.RemoveListener(OnBattery1Placed);
        batterySocket2.selectEntered.RemoveListener(OnBattery2Placed);
        coverSocket.selectEntered.RemoveListener(OnCoverPlaced);
        batterySocket1.selectExited.RemoveListener(OnBattery1Removed);
        batterySocket2.selectExited.RemoveListener(OnBattery2Removed);
        coverSocket.selectExited.RemoveListener(OnCoverRemoved);
        grabInteractable.activated.RemoveListener(OnTriggerPulled);
    }

    public void HandleAutoSnap(Collider other)
    {
        if (interactionManager == null) return;

        // 嘗試由進入嘅 Collider 身上拎可抓取物件
        XRGrabInteractable objectToSocket = other.GetComponent<XRGrabInteractable>();
        
        // 如果入嚟嘅嘢唔係可抓取物件，就唔理佢
        if (objectToSocket == null) return;

        // 處理電池
        if (other.CompareTag("Battery"))
        {
            // 如果 Socket 1 係空，就優先放入 Socket 1
            if (!batterySocket1.hasSelection)
            {
                interactionManager.SelectEnter((IXRSelectInteractor)batterySocket1, (IXRSelectInteractable)objectToSocket);
            }
            // 如果 Socket 1 滿咗，但 Socket 2 係空，就放入 Socket 2
            else if (!batterySocket2.hasSelection)
            {
                interactionManager.SelectEnter((IXRSelectInteractor)batterySocket2, (IXRSelectInteractable)objectToSocket);
            }
        }

        // 處理遙控器蓋
        if (other.CompareTag("RemoteCover"))
        {
            // 確保蓋嘅 Socket 係啟用緊並且係空嘅
            if (coverSocket.gameObject.activeInHierarchy && !coverSocket.hasSelection)
            {
                interactionManager.SelectEnter((IXRSelectInteractor)coverSocket, (IXRSelectInteractable)objectToSocket);
            }
        }
    }

    private void OnBattery1Placed(SelectEnterEventArgs arg) 
    { 
        battery1In = true; 
        LockPartPhysics(arg.interactableObject); // 鎖定物理
        CheckAssembly(); 
    }
    private void OnBattery1Removed(SelectExitEventArgs arg) 
    { 
        battery1In = false; 
        UnlockPartPhysics(arg.interactableObject); // 解鎖物理
        CheckAssembly(); 
    }

    private void OnBattery2Placed(SelectEnterEventArgs arg) 
    { 
        battery2In = true; 
        LockPartPhysics(arg.interactableObject); // 鎖定物理
        CheckAssembly(); 
    }
    private void OnBattery2Removed(SelectExitEventArgs arg) 
    { 
        battery2In = false; 
        UnlockPartPhysics(arg.interactableObject); // 解鎖物理
        CheckAssembly(); 
    }
    
    private void CheckAssembly()
    {
        // 當兩粒電池都裝好，先至啟用個蓋嘅插槽
        if (battery1In && battery2In)
        {
            coverSocket.gameObject.SetActive(true);
        }
        else
        {
            coverSocket.gameObject.SetActive(false); // 如果拎走一粒電，個蓋插槽又會失效
        }
    }
    
    private void OnCoverPlaced(SelectEnterEventArgs arg) 
    {
        coverIn = true; 
        LockPartPhysics(arg.interactableObject);
        eventManager.OnRemoteAssembled();
    }

    private void OnCoverRemoved(SelectExitEventArgs arg)
    {
        coverIn = false;
        UnlockPartPhysics(arg.interactableObject);
        eventManager.OnRemoteDisassembled();
    }

    private void OnTriggerPulled(ActivateEventArgs arg)
    {
        // 檢查係咪組裝完成
        if (!coverIn) return;

        // 確定射線起點 (如果冇設置 rayOrigin，就用遙控器自身中心)
        Transform startPoint = rayOrigin != null ? rayOrigin : transform;

        // 發射物理射線
        RaycastHit hit;
        if (Physics.Raycast(startPoint.position, -startPoint.right, out hit, maxDistance, tvLayerMask))
        {
            Debug.Log($"射線擊中: {hit.collider.name}");

            // 檢查擊中嘅係咪電視屏幕 (通過檢查佢上面有冇 SnowNoiseEvent 引用嘅那個 Interactable)
            if (hit.collider.gameObject == eventManager.tvScreenInteractable.gameObject)
            {
                eventManager.ResolveEvent();
            }
        }
    }
    
    // 鎖定零件嘅物理碰撞，將其 Collider 變為 Trigger 模式，以防止同手部碰撞
    private void LockPartPhysics(IXRSelectInteractable interactable)
    {
        // 獲取被裝入物件嘅 Transform
        Transform partTransform = interactable.transform;
        // 搵出佢身上所有嘅 Collider (包括子物件)
        foreach (var col in partTransform.GetComponentsInChildren<Collider>())
        {
            col.isTrigger = true; // 變為 Trigger，唔再有固體碰撞
        }
    }

    // 解鎖零件嘅物理碰撞，將其 Collider 恢復為固體模式
    private void UnlockPartPhysics(IXRSelectInteractable interactable)
    {
        Transform partTransform = interactable.transform;
        foreach (var col in partTransform.GetComponentsInChildren<Collider>())
        {
            col.isTrigger = false; // 恢復為固體
        }
    }
}