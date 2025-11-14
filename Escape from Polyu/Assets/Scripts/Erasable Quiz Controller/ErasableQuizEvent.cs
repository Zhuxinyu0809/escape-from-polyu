using UnityEngine;
using System.Collections;
using TMPro;

public class ErasableQuizEvent : MonoBehaviour, IEvent
{
    [Header("Quiz Objects")]
    public GameObject quizContainer;
    public TextMeshPro successText;

    [Header("Flicking Settings")]
    public Color flashColor = Color.red;
    public float flashDuration = 0.2f;
    public int flashCount = 3;

    private TextMeshPro[] allTextElements;
    private Color[] originalTextColors;
    private ErasableAnswer[] allAnswers;
    private bool isHandlingResult = false;

    public bool IsCompleted { get; private set; } = false;

    void Start()
    {
        quizContainer.SetActive(false);
        if (successText != null) successText.text = "";

        allTextElements = quizContainer.GetComponentsInChildren<TextMeshPro>();
        allAnswers = quizContainer.GetComponentsInChildren<ErasableAnswer>();

        originalTextColors = new Color[allTextElements.Length];
        for (int i = 0; i < allTextElements.Length; i++)
        {
            originalTextColors[i] = allTextElements[i].color;
        }
    }

    public void StartEvent()
    {
        if (IsCompleted) return;

        Debug.Log("測驗事件開始");
        quizContainer.SetActive(true);
    }

    public void ResetEvent()
    {
        IsCompleted = false;
        ResetAllAnswers();
        quizContainer.SetActive(false);
        if (successText != null) successText.text = "";
    }

    public void OnAnswerErased(bool wasCorrect)
    {
        if (isHandlingResult || IsCompleted) return;

        isHandlingResult = true;

        if (wasCorrect)
        {
            StartCoroutine(HandleCorrectAnswer());
        }
        else
        {
            StartCoroutine(HandleWrongAnswer());
        }
    }

    private IEnumerator HandleCorrectAnswer()
    {
        Debug.Log("答案正確");
        yield return new WaitForSeconds(2.0f);

        // 隱藏所有文字
        foreach (var text in allTextElements)
        {
            text.text = "";
        }

        if (successText != null)
        {
            successText.text = "Correct!";
        }

        IsCompleted = true;

        EventManager.instance.CheckAllEventsCompletion();
    }

    private IEnumerator HandleWrongAnswer()
    {
        Debug.Log("答案錯誤");
        yield return new WaitForSeconds(2.0f);

        // 開始閃爍
        for (int i = 0; i < flashCount; i++)
        {
            SetAllTextColors(flashColor);
            yield return new WaitForSeconds(flashDuration);
            ResetAllTextColors();
            yield return new WaitForSeconds(flashDuration);
        }

        // 閃爍完畢，重置所有答案狀態
        ResetAllAnswers();
        isHandlingResult = false;
    }

    private void SetAllTextColors(Color color)
    {
        for (int i = 0; i < allTextElements.Length; i++)
        {
            allTextElements[i].color = color;
        }
    }

    private void ResetAllTextColors()
    {
        for (int i = 0; i < allTextElements.Length; i++)
        {
            allTextElements[i].color = originalTextColors[i];
        }
    }

    private void ResetAllAnswers()
    {
        foreach (var answer in allAnswers)
        {
            answer.ResetAnswer();
        }
    }
}
