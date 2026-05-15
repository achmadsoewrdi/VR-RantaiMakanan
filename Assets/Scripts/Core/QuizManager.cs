using System.Collections;
using UnityEngine;
using TMPro;

public class QuizManager : MonoBehaviour
{
    [Header("Data Quiz")]
    [SerializeField] private QuizData quizData;

    [Header("UI References")]
    [SerializeField] private GameObject quizPanel;
    [SerializeField] private TextMeshProUGUI questionText;
    [SerializeField] private TextMeshProUGUI optionAText;
    [SerializeField] private TextMeshProUGUI optionBText;
    [SerializeField] private TextMeshProUGUI feedbackText;

    [Header("Gaze Buttons")]
    [SerializeField] private GazeButton buttonA;
    [SerializeField] private GazeButton buttonB;

    [Header("Warna Feedback")]
    [SerializeField] private Color correctColor = new Color(0.2f, 0.8f, 0.2f);
    [SerializeField] private Color wrongColor = new Color(0.8f, 0.2f, 0.2f);

    public System.Action OnQuizSelesai; // Alarm penanda kuis beres
    private bool _answered = false;

    private void Start()
    {
        // Pastikan panel mati di awal
        if (quizPanel != null) quizPanel.SetActive(false);
        if (feedbackText != null) feedbackText.gameObject.SetActive(false);

        // Setup callback tombol
        if (buttonA != null) buttonA.OnSelected += () => HandleAnswer(true);
        if (buttonB != null) buttonB.OnSelected += () => HandleAnswer(false);
    }

    public void SetQuizData(QuizData newData) 
    { 
        quizData = newData; 
    }

    // --- DIUBAH MENJADI PUBLIC AGAR BISA DIPANGGIL SUTRADARA ---
    public void ShowQuiz()
    {
        if (quizData == null) return;

        questionText.text = quizData.question;
        optionAText.text = "A. " + quizData.optionA;
        optionBText.text = "B. " + quizData.optionB;

        feedbackText.gameObject.SetActive(false);
        _answered = false;

        // PENTING: Nyalakan kembali tombol untuk Kuis Beruntun!
        if (buttonA != null) buttonA.SetInteractable(true);
        if (buttonB != null) buttonB.SetInteractable(true);

        // --- ANIMASI MUNCUL SMOOTH ---
        quizPanel.SetActive(true); // Aktifkan dulu objeknya
        StartCoroutine(FadePanel(0, 1, 0.0001f, 0.003f)); // Pudar 0 ke 1, Skala 0.8 ke 1
        Debug.Log("[QuizManager] Quiz muncul halus.");
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
            feedbackText.text = "Benar!";
            feedbackText.color = correctColor;
        }
        else
        {
            feedbackText.text = "Salah!";
            feedbackText.color = wrongColor;

            // Tampilkan jawaban yang benar
            string correct = quizData.isAnswerA ? quizData.optionA : quizData.optionB;
            feedbackText.text += "\nJawaban: " + correct;
        }
    }

    private IEnumerator ProceedAfterFeedback(float delay)
    {
        yield return new WaitForSeconds(delay);
        
        // --- ANIMASI HILANG SMOOTH ---
        yield return StartCoroutine(FadePanel(1, 0, 0.003f, 0f)); // Pudar 1 ke 0
        quizPanel.SetActive(false);

        // --- PERBAIKAN LOGIKA ANTISIPASI ERROR ---
        if (OnQuizSelesai != null)
        {
            // Jika ada antrean (Scene Beruntun), langsung panggil
            OnQuizSelesai.Invoke();
        }
        else 
        {
            // Jika tidak ada antrean (Scene biasa), tunggu jeda lalu pindah scene
            yield return new WaitForSeconds(1.5f); 

            if (SceneController.Instance != null)
            {
                SceneController.Instance.LoadNextScene();
            }
        }
    }

    IEnumerator FadePanel(float startAlpha, float endAlpha, float startScale, float endScale)
    {
        CanvasGroup cg = quizPanel.GetComponent<CanvasGroup>();
        if (cg == null) yield break;

        float duration = 0.5f; // Kecepatan animasi (0.5 detik)
        float time = 0;

        while (time < duration)
        {
            time += Time.deltaTime;
            float lerpTime = time / duration;
            cg.alpha = Mathf.Lerp(startAlpha, endAlpha, lerpTime);
            quizPanel.transform.localScale = Vector3.Lerp(Vector3.one * startScale, Vector3.one * endScale, lerpTime);
            yield return null;
        }
        cg.alpha = endAlpha;
        quizPanel.transform.localScale = Vector3.one * endScale;
    }
}