using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

/// <summary>
/// Mengontrol alur perpindahan antar scene.
/// Letakkan script ini di GameObject "SceneController" di dalam [Managers].
///
/// Build Settings harus sudah berisi semua scene dengan urutan index yang benar:
/// Index 0: Scene_00_MainMenu
/// Index 1: Scene_01_Opening
/// Index 2: Scene_02_Sawah_Intro
/// ... dst.
/// </summary>
public class SceneController : MonoBehaviour
{
    public static SceneController Instance { get; private set; }

    [Header("Transisi")]
    [Tooltip("Berapa detik fade/jeda sebelum scene berikutnya dimuat.")]
    [SerializeField] private float transitionDelay = 0.5f;

    // -------------------------------------------------------

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    // -------------------------------------------------------
    // PUBLIC METHODS
    // -------------------------------------------------------

    /// <summary>
    /// Load scene berikutnya berdasarkan Build Index saat ini + 1.
    /// Dipanggil otomatis oleh QuizManager setelah jawaban diberikan.
    /// </summary>
    public void LoadNextScene()
    {
        int currentIndex = SceneManager.GetActiveScene().buildIndex;
        int nextIndex = currentIndex + 1;

        if (nextIndex >= SceneManager.sceneCountInBuildSettings)
        {
            Debug.Log("[SceneController] Ini scene terakhir. Kembali ke Main Menu.");
            LoadSceneByIndex(0); // Kembali ke Main Menu
            return;
        }

        StartCoroutine(LoadWithDelay(nextIndex));
    }

    /// <summary>Load scene berdasarkan index Build Settings.</summary>
    public void LoadSceneByIndex(int index)
    {
        StartCoroutine(LoadWithDelay(index));
    }

    /// <summary>Load scene berdasarkan nama file scene.</summary>
    public void LoadSceneByName(string sceneName)
    {
        StartCoroutine(LoadWithDelayByName(sceneName));
    }

    // -------------------------------------------------------
    // PRIVATE
    // -------------------------------------------------------

    private IEnumerator LoadWithDelay(int index)
    {
        yield return new WaitForSeconds(transitionDelay);
        Debug.Log($"[SceneController] Memuat scene index: {index}");
        SceneManager.LoadScene(index);
    }

    private IEnumerator LoadWithDelayByName(string sceneName)
    {
        yield return new WaitForSeconds(transitionDelay);
        Debug.Log($"[SceneController] Memuat scene: {sceneName}");
        SceneManager.LoadScene(sceneName);
    }
}
