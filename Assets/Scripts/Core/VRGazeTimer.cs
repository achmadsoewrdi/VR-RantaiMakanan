using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro; // Gunakan ini jika Text Feedback-mu pakai TextMeshPro

public class VRGazeTimer : MonoBehaviour
{
    [Header("Referensi Sistem")]
    public Transform kameraUser; // Titik tembak laser (Main Camera)
    public GameObject canvasQuiz; // UI Kuis keseluruhan
    public TextMeshProUGUI textFeedback; // Teks Benar/Salah (ganti jadi Text biasa jika tidak pakai TMP)

    [Header("Pengaturan Gaze")]
    public float waktuTunggu = 3.0f; // 3 Detik
    public Image ringLoading; // UI lingkaran yang akan penuh

    private float timerMata = 0f;
    private GameObject kotakDitatapAwal;
    private bool kuisSedangBerjalan = false;

    // Fungsi ini dipanggil oleh EcoSequenceManager saat narasi selesai
    public void MulaiKuis()
    {
        if (canvasQuiz != null) canvasQuiz.SetActive(true); // Munculkan panel kuis
        if (textFeedback != null) textFeedback.text = ""; // Kosongkan teks feedback
        kuisSedangBerjalan = true;
        ringLoading.fillAmount = 0f;
    }

    void Update()
    {
        if (!kuisSedangBerjalan) return; // Jika kuis belum/sudah selesai, matikan laser

        RaycastHit hit;
        // Tembakkan laser ke depan dari tengah kamera sepanjang 10 meter
        if (Physics.Raycast(kameraUser.position, kameraUser.forward, out hit, 10f))
        {
            // Jika laser menabrak sesuatu yang punya Tag Jawaban
            if (hit.collider.CompareTag("JawabanBenar") || hit.collider.CompareTag("JawabanSalah"))
            {
                if (kotakDitatapAwal != hit.collider.gameObject)
                {
                    // Jika User menggeser pandangan ke kotak lain, reset timer!
                    ResetTimer();
                    kotakDitatapAwal = hit.collider.gameObject;
                }

                // Tambah waktu selama mata terus menatap kotak yang sama
                timerMata += Time.deltaTime;
                if (ringLoading != null) ringLoading.fillAmount = timerMata / waktuTunggu; // Animasi UI penuh

                // Jika sudah menatap 3 detik
                if (timerMata >= waktuTunggu)
                {
                    KunciJawaban(hit.collider.tag);
                }
            }
            else
            {
                ResetTimer(); // Laser menabrak tembok/pohon, reset.
            }
        }
        else
        {
            ResetTimer(); // Laser menabrak udara kosong, reset.
        }
    }

    void ResetTimer()
    {
        timerMata = 0f;
        kotakDitatapAwal = null;
        if (ringLoading != null) ringLoading.fillAmount = 0f;
    }

    void KunciJawaban(string tagJawaban)
    {
        kuisSedangBerjalan = false; // Hentikan laser
        ResetTimer();

        // Cek Jawaban
        if (tagJawaban == "JawabanBenar")
        {
            textFeedback.text = "BENAR! Katak adalah konsumen tingkat kedua.";
            textFeedback.color = Color.green;
        }
        else if (tagJawaban == "JawabanSalah")
        {
            textFeedback.text = "SALAH! Coba ingat lagi penjelasannya.";
            textFeedback.color = Color.red;
        }

        // Mulai hitung mundur 5 detik untuk menghapus Quiz
        StartCoroutine(HilangkanKuisDelay());
    }

    IEnumerator HilangkanKuisDelay()
    {
        yield return new WaitForSeconds(5.0f);
        if (canvasQuiz != null) canvasQuiz.SetActive(false);
        Debug.Log("Scene Rantai Makanan SELESAI!");
    }
}