using UnityEngine;

/// <summary>
/// ScriptableObject untuk menyimpan data quiz per scene.
/// Cara buat: Klik kanan di Project window → Create → VR Edu → Quiz Data
/// </summary>
[CreateAssetMenu(fileName = "QuizData_SceneXX", menuName = "VR Edu/Quiz Data")]
public class QuizData : ScriptableObject
{
    [Header("Pertanyaan")]
    [TextArea(2, 4)]
    public string question = "Isi pertanyaan di sini...";

    [Header("Pilihan Jawaban")]
    public string optionA = "Pilihan A";
    public string optionB = "Pilihan B";

    [Header("Jawaban Benar")]
    [Tooltip("Centang = A yang benar. Kosong = B yang benar.")]
    public bool isAnswerA = true;

    [Header("Timing")]
    [Tooltip("Berapa detik setelah narasi selesai, quiz muncul.")]
    public float displayDelay = 0.5f;

    [Tooltip("Berapa detik user harus menatap tombol untuk memilih jawaban.")]
    public float gazeSelectDuration = 2.0f;

    [Tooltip("Berapa detik feedback (benar/salah) ditampilkan sebelum lanjut.")]
    public float feedbackDuration = 1.5f;
}
