using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class SceneController : MonoBehaviour
{
    public static SceneController Instance { get; private set; }

    [Header("Transisi")]
    [SerializeField] private float transitionDelay = 0.5f;
    [SerializeField] private float fadeDuration = 1.0f;
    [SerializeField] private Color fadeColor = Color.white;

    private CanvasGroup fadeCanvasGroup;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject); // Bertahan antar scene
        SetupFadeCanvas();
    }

    private void SetupFadeCanvas()
    {
        // Buat canvas fade otomatis via code — tidak perlu setup di Editor
        GameObject canvasGO = new GameObject("FadeCanvas");
        canvasGO.transform.SetParent(transform);

        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 999;
        canvasGO.AddComponent<CanvasScaler>();
        canvasGO.AddComponent<GraphicRaycaster>();

        GameObject panelGO = new GameObject("FadePanel");
        panelGO.transform.SetParent(canvasGO.transform, false);

        Image image = panelGO.AddComponent<Image>();
        image.color = fadeColor;

        RectTransform rect = panelGO.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        fadeCanvasGroup = panelGO.AddComponent<CanvasGroup>();
        fadeCanvasGroup.alpha = 0f;
        fadeCanvasGroup.blocksRaycasts = false;
        fadeCanvasGroup.interactable = false;
    }

    // -------------------------------------------------------
    // PUBLIC METHODS — sama seperti sebelumnya, tidak ada yang berubah
    // -------------------------------------------------------

    public void LoadNextScene()
    {
        int nextIndex = SceneManager.GetActiveScene().buildIndex + 1;

        if (nextIndex >= SceneManager.sceneCountInBuildSettings)
        {
            Debug.Log("[SceneController] Ini scene terakhir. Kembali ke Main Menu.");
            LoadSceneByIndex(0);
            return;
        }

        StartCoroutine(LoadWithFade(nextIndex));
    }

    public void LoadSceneByIndex(int index)
    {
        StartCoroutine(LoadWithFade(index));
    }

    public void LoadSceneByName(string sceneName)
    {
        StartCoroutine(LoadWithFadeByName(sceneName));
    }

    // -------------------------------------------------------
    // PRIVATE — fade in → load → fade out
    // -------------------------------------------------------

    private IEnumerator LoadWithFade(int index)
    {
        yield return StartCoroutine(FadeIn());
        yield return new WaitForSeconds(transitionDelay);
        yield return SceneManager.LoadSceneAsync(index);
        yield return StartCoroutine(FadeOut());
    }

    private IEnumerator LoadWithFadeByName(string sceneName)
    {
        yield return StartCoroutine(FadeIn());
        yield return new WaitForSeconds(transitionDelay);
        yield return SceneManager.LoadSceneAsync(sceneName);
        yield return StartCoroutine(FadeOut());
    }

    private IEnumerator FadeIn()
    {
        // Transparan → putih
        fadeCanvasGroup.blocksRaycasts = true;
        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            fadeCanvasGroup.alpha = Mathf.Clamp01(timer / fadeDuration);
            yield return null;
        }
        fadeCanvasGroup.alpha = 1f;
    }

    private IEnumerator FadeOut()
    {
        // Putih → transparan
        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            fadeCanvasGroup.alpha = 1f - Mathf.Clamp01(timer / fadeDuration);
            yield return null;
        }
        fadeCanvasGroup.alpha = 0f;
        fadeCanvasGroup.blocksRaycasts = false;
    }
}