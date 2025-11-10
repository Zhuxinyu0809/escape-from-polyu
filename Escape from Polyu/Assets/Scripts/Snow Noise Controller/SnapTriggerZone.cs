using UnityEngine;

public class SnapTriggerZone : MonoBehaviour
{
    [Header("Remote Body")]
    public AssembledRemote mainRemoteScript;

    void Start()
    {
        if (mainRemoteScript == null)
        {
            mainRemoteScript = GetComponentInParent<AssembledRemote>();
        }

        if (mainRemoteScript == null)
        {
            Debug.LogError("SnapTriggerZone 搵唔到父物件上嘅 AssembledRemote script");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (mainRemoteScript != null)
        {
            mainRemoteScript.HandleAutoSnap(other);
        }
    }
}