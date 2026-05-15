using System.Collections;
using UnityEngine;
using TMPro;

/// <summary>
/// Mengelola tampilan dan logika quiz per scene.
/// Letakkan script ini di GameObject "QuizManager" di dalam [Managers].
///
/// Alur:
/// AudioManager selesai narasi
///   → QuizManager.ShowQuiz() dipanggil (otomatis via event)
///   → Panel quiz muncul
///   → User pilih jawaban via gaze
///   → Feedback ditampilkan
///   → SceneController.LoadNextScene() dipanggil
/// </summary>
public class QuizManager : MonoBehaviour
{
    [Header("Data Quiz")]
    [Tooltip("Drag file QuizData_SceneXX.asset milik scene ini ke sini.")]
    [SerializeField] private QuizData quizData;

    [Header("UI References")]
    [Tooltip("Root GameObject panel quiz. Akan di-show/hide.")]
    [SerializeField] private GameObject quizPanel;

    [Tooltip("Text untuk menampilkan pertanyaan.")]
    [SerializeField] private TextMeshProUGUI questionText;

    [Tooltip("Text untuk menampilkan pilihan A.")]
    [SerializeField] private TextMeshProUGUI optionAText;

    [Tooltip("Text untuk menampilkan pilihan B.")]
    [SerializeField] private TextMeshProUGUI optionBText;

    [Tooltip("Text feedback (Benar! / Salah!).")]
    [SerializeField] private TextMeshProUGUI feedbackText;

    [Header("Gaze Buttons")]
    [Tooltip("GazeButton untuk pilihan A.")]
    [SerializeField] private GazeButton buttonA;

    [Tooltip("GazeButton untuk pilihan B.")]
    [SerializeField] private GazeButton buttonB;

    [Header("Warna Feedback")]
    [SerializeField] private Color correctColor = new Color(0.2f, 0.8f, 0.2f);
    [SerializeField] private Color wrongColor = new Color(0.8f, 0.2f, 0.2f);

    // -------------------------------------------------------

    private bool _answered = false;

    // -------------------------------------------------------

    private void Start()
    {
        // Sembunyikan panel saat awal
        quizPanel.SetActive(false);
        feedbackText.gameObject.SetActive(false);

        // Subscribe ke event narasi selesai
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.OnNarrationFinished += OnNarrationFinished;
        }
        else
        {
            Debug.LogWarning("[QuizManager] AudioManager tidak ditemukan! Quiz tidak akan muncul otomatis.");
        }

        // Setup callback tombol
        if (buttonA != null) buttonA.OnSelected += () => HandleAnswer(true);
        if (buttonB != null) buttonB.OnSelected += () => HandleAnswer(false);
    }

    private void OnDestroy()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.OnNarrationFinished -= OnNarrationFinished;
    }

    // -------------------------------------------------------
    // EVENT HANDLERS
    // -------------------------------------------------------

    private void OnNarrationFinished()
    {
        if (quizData == null)
        {
            Debug.LogWarning("[QuizManager] QuizData belum di-assign! Langsung lanjut scene.");
            SceneController.Instance?.LoadNextScene();
            return;
        }

        StartCoroutine(ShowQuizAfterDelay(quizData.displayDelay));
    }

    // -------------------------------------------------------
    // CORE LOGIC
    // -------------------------------------------------------

    private IEnumerator ShowQuizAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        ShowQuiz();
    }

    private void ShowQuiz()
    {
        if (quizData == null) return;

        // Isi teks dari ScriptableObject
        questionText.text = quizData.question;
        optionAText.text = "A. " + quizData.optionA;
        optionBText.text = "B. " + quizData.optionB;

        // Set durasi gaze dari data
        if (buttonA != null) buttonA.SetGazeDuration(quizData.gazeSelectDuration);
        if (buttonB != null) buttonB.SetGazeDuration(quizData.gazeSelectDuration);

        feedbackText.gameObject.SetActive(false);
        _answered = false;

        quizPanel.SetActive(true);
        Debug.Log("[QuizManager] Quiz ditampilkan.");
    }

    private void HandleAnswer(bool isAnswerA)
    {
        if (_answered) return;
        _answered = true;

        // Nonaktifkan tombol supaya tidak bisa diklik lagi
        if (buttonA != null) buttonA.SetInteractable(false);
        if (buttonB != null) buttonB.SetInteractable(false);

        bool isCorrect = (isAnswerA == quizData.isAnswerA);
        ShowFeedback(isCorrect);

        StartCoroutine(ProceedAfterFeedback(quizData.feedbackDuration));
    }

    private void ShowFeedback(bool isCorrect)
    {
        feedbackText.gameObject.SetActive(true);

        if (isCorrect)
        {
            feedbackText.text = "✓ Benar!";
            feedbackText.color = correctColor;
        }
        else
        {
            feedbackText.text = "✗ Salah!";
            feedbackText.color = wrongColor;

            // Tampilkan jawaban yang benar
            string correct = quizData.isAnswerA ? quizData.optionA : quizData.optionB;
            feedbackText.text += "\nJawaban: " + correct;
        }
    }

    private IEnumerator ProceedAfterFeedback(float delay)
    {
        yield return new WaitForSeconds(delay);
        quizPanel.SetActive(false);

        // Minta SceneController untuk load scene berikutnya
        SceneController.Instance?.LoadNextScene();
    }
}
