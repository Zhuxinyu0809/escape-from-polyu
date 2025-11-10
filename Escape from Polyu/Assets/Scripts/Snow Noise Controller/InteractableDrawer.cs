using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using System.Collections;

[RequireComponent(typeof(XRSimpleInteractable))]
public class InteractableDrawer : MonoBehaviour
{
    [Header("Drawer Settings")]
    public Vector3 openLocalPosition = new Vector3(-2.8f, 0, 0);
    public float moveSpeed = 1.0f;

    private XRSimpleInteractable interactable;
    private bool isOpen = false;
    private bool isMoving = false;

    void Start()
    {
        interactable = GetComponent<XRSimpleInteractable>();
        interactable.selectEntered.AddListener(OnDrawerClicked);

        isOpen = false;
    }

    void OnDestroy()
    {
        if (interactable != null)
        {
            interactable.selectEntered.RemoveListener(OnDrawerClicked);
        }
    }

    private void OnDrawerClicked(SelectEnterEventArgs arg)
    {
        if (isOpen || isMoving) return;
        
        StartCoroutine(MoveDrawer(openLocalPosition));
        isOpen = true;
        interactable.enabled = false;
    }

    private IEnumerator MoveDrawer(Vector3 targetLocalPosition)
    {
        isMoving = true;
        Vector3 startPos = transform.localPosition;
        float time = 0;

        while (time < 1.0f)
        {
            transform.localPosition = Vector3.Lerp(startPos, targetLocalPosition, time);
            time += Time.deltaTime * moveSpeed;
            yield return null;
        }
        transform.localPosition = targetLocalPosition;
        isMoving = false;
    }
}