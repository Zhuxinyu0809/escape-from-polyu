using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(XRSocketInteractor))]
public class MottoSocket : MonoBehaviour
{
    [Header("ID of the Socket")]
    public string correctCharacterID = "Hoi";

    private XRSocketInteractor socket;
    private MottoQuizEvent quizManager;

    void Start()
    {
        socket = GetComponent<XRSocketInteractor>();
        quizManager = FindFirstObjectByType<MottoQuizEvent>();

        // 監聽 Socket 事件
        socket.selectEntered.AddListener(OnPiecePlaced);
        socket.selectExited.AddListener(OnPieceRemoved);
    }
    
    void OnDestroy()
    {
        socket.selectEntered.RemoveListener(OnPiecePlaced);
        socket.selectExited.RemoveListener(OnPieceRemoved);
    }

    private void OnPiecePlaced(SelectEnterEventArgs arg)
    {
        MottoPiece piece = arg.interactableObject.transform.GetComponent<MottoPiece>();
        if (piece == null) return;

        // 檢查 ID 係咪匹配
        if (piece.characterID == correctCharacterID)
        {
            quizManager.OnPieceCorrectlyPlaced();
        }
        // 可選：如果擺錯字，可以喺呢度加一個「彈開」嘅邏輯
    }

    private void OnPieceRemoved(SelectExitEventArgs arg)
    {
        MottoPiece piece = arg.interactableObject.transform.GetComponent<MottoPiece>();
        if (piece == null) return;

        if (piece.characterID == correctCharacterID)
        {
            quizManager.OnCorrectPieceRemoved();
        }
    }
}