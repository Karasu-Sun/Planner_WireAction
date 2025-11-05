using UnityEngine;
using UnityEngine.UI;

public class SceneFader : MonoBehaviour
{
    public static SceneFader Instance { get; private set; }

    [Header("Canvas Group Settings")]
    [SerializeField] private CanvasGroup fadeCanvasGroup;
    [SerializeField] private float fadeDuration = 1.0f;

    private float fadeTimer = 0f;
    public bool isFading { get; private set; } = false;

    private System.Action onFadeComplete;

    private enum FadeType { None, In, Out }
    private FadeType currentFade = FadeType.None;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitFadeCanvas();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitFadeCanvas()
    {
        if (fadeCanvasGroup != null)
        {
            fadeCanvasGroup.alpha = 1f;
            fadeCanvasGroup.blocksRaycasts = false;
            fadeCanvasGroup.interactable = false;
        }
        else
        {
            Debug.LogWarning("[SceneFader] CanvasGroup が未設定です。");
        }
    }

    public void StartFadeOut(System.Action onComplete = null)
    {
        if (fadeCanvasGroup == null) return;

        isFading = true;
        currentFade = FadeType.Out;
        fadeTimer = 0f;
        onFadeComplete = onComplete;

        fadeCanvasGroup.alpha = 0f;
        fadeCanvasGroup.blocksRaycasts = true;
        fadeCanvasGroup.interactable = true;
    }

    public void StartFadeIn(System.Action onComplete = null)
    {
        if (fadeCanvasGroup == null) return;

        isFading = true;
        currentFade = FadeType.In;
        fadeTimer = 0f;
        onFadeComplete = onComplete;

        fadeCanvasGroup.alpha = 1f;
        fadeCanvasGroup.blocksRaycasts = true;
        fadeCanvasGroup.interactable = true;
    }

    private void Update()
    {
        if (!isFading) return;

        Time.timeScale = 0f; // フェード中はゲーム停止
        Debug.Log("Game is stopping");

        switch (currentFade)
        {
            case FadeType.Out:
                FadeOut();
                break;
            case FadeType.In:
                FadeIn();
                break;
        }
    }

    private void FadeOut()
    {
        fadeTimer += Time.unscaledDeltaTime;
        float alpha = Mathf.Clamp01(fadeTimer / fadeDuration);

        SetFadeAlpha(alpha);

        if (fadeTimer >= fadeDuration)
            EndFade(1f);
    }

    private void FadeIn()
    {
        fadeTimer += Time.unscaledDeltaTime;
        float alpha = Mathf.Clamp01(1f - (fadeTimer / fadeDuration));

        SetFadeAlpha(alpha);

        if (fadeTimer >= fadeDuration)
            EndFade(0f);
    }

    private void SetFadeAlpha(float alpha)
    {
        fadeCanvasGroup.alpha = alpha;
    }

    private void EndFade(float finalAlpha)
    {
        isFading = false;
        fadeCanvasGroup.alpha = finalAlpha;

        fadeCanvasGroup.blocksRaycasts = false;
        fadeCanvasGroup.interactable = false;

        Time.timeScale = 1f;
        onFadeComplete?.Invoke();
        onFadeComplete = null;
        currentFade = FadeType.None;
    }
}