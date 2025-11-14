using UnityEngine;
using TMPro;

[RequireComponent(typeof(Collider))]
public class ErasableAnswer : MonoBehaviour
{
    [Header("Answer Settings")]
    public bool isCorrectAnswer = false;

    [Header("Erasing Settings")]
    public float timeToFade = 2.0f;
    public float timeToErase = 4.0f;

    private ErasableQuizEvent quizManager;
    private TextMeshPro textMesh;
    private Color originalColor;
    private float contactDuration = 0.0f;
    private bool isBeingErased = false;
    private bool isLocked = false;

    void Awake()
    {
        textMesh = GetComponent<TextMeshPro>();
        originalColor = textMesh.color;
        quizManager = FindFirstObjectByType<ErasableQuizEvent>();
    }

    void Update()
    {
        // 如果被 Lock 咗或者冇被擦緊，就乜都唔做
        if (isLocked || !isBeingErased) return;

        contactDuration += Time.deltaTime;

        if (contactDuration >= timeToErase)
        {
            // 4秒： 完全擦除
            SetAlpha(0.0f);
            isBeingErased = false;
            isLocked = true;
            Debug.Log($"{gameObject.name} 已被擦除");

            quizManager.OnAnswerErased(isCorrectAnswer);
        }
        else if (contactDuration >= timeToFade)
        {
            // 2秒： 半透明
            SetAlpha(0.3f);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // 檢查係咪白板擦
        if (other.GetComponent<WhiteboardEraser>() != null)
        {
            if (isLocked) return;
            isBeingErased = true;
            Debug.Log($"{gameObject.name} 開始被擦");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<WhiteboardEraser>() != null)
        {
            if (isLocked) return;
            isBeingErased = false;
            SetAlpha(1.0f);
            Debug.Log($"{gameObject.name} 停止被擦");
        }
    }

    private void SetAlpha(float alpha)
    {
        textMesh.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
    }

    public void ResetAnswer()
    {
        SetAlpha(1.0f);
        contactDuration = 0.0f;
        isBeingErased = false;
        isLocked = false;
    }

    public void HideAnswer()
    {
        SetAlpha(0.0f);
        isLocked = true;
    }
}
