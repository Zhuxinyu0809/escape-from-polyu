using UnityEngine;

public class MottoQuizEvent : MonoBehaviour, IEvent
{
    [Header("Event Objetcs")]
    public ChairColorController chairController;
    
    [Header("Amount of Pieces Required")]
    public int totalPiecesRequired = 6; // 總共 8 個字，牆上已有 2 個

    public bool IsCompleted { get; private set; } = false;

    private int correctPiecesCount = 0;

    public void StartEvent()
    {
        if (IsCompleted) return;
        Debug.Log("Motto Even】事件開始! 凳變色");
        if (chairController != null)
        {
            chairController.SetWarningPattern();
        }
    }

    public void ResetEvent()
    {
        IsCompleted = false;
        correctPiecesCount = 0;
        if (chairController != null)
        {
            chairController.SetWarningPattern();
        }
    }

    public void OnPieceCorrectlyPlaced()
    {
        if (IsCompleted) return;

        correctPiecesCount++;
        Debug.Log($"呢個字擺放正確。目前進度: {correctPiecesCount} / {totalPiecesRequired}");

        if (correctPiecesCount >= totalPiecesRequired)
        {
            CompleteEvent();
        }
    }

    public void OnCorrectPieceRemoved()
    {
        if (IsCompleted) return;
        
        correctPiecesCount--;
        Debug.Log($"哎呀，正確嘅字被掹走咗。目前進度: {correctPiecesCount} / {totalPiecesRequired}");
    }

    private void CompleteEvent()
    {
        IsCompleted = true;
        Debug.Log("Motto Event 事件完成！校訓已補完，凳恢復正常。");
        
        // 將凳恢復正常顏色
        if (chairController != null)
        {
            chairController.SetNormalPattern();
        }
        
        if (EventManager.instance != null)
        {
            EventManager.instance.CheckAllEventsCompletion();
        }
    }
}