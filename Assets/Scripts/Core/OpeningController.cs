using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

/// <summary>
/// Mengontrol urutan tampilan Scene_01_Opening.
/// Beat 1: Judul + narasi pertama
/// Beat 2: Animasi matahari + narasi kedua
/// Setelah selesai: load scene berikutnya via SceneController
/// </summary>
public class OpeningController : MonoBehaviour
{
    [Header("Beat 1 — Judul")]
    public GameObject judulPanel;           // Panel judul "Rantai Makanan"
    public TextMeshProUGUI subtitleText;    // Teks narasi Beat 1
    public string narasiBeat1 = "Halo anak-anak! Hari ini kita akan belajar bersama";

    [Header("Beat 2 — Animasi Matahari")]
    public GameObject animasiPanel;         // Panel animasi matahari + tumbuhan
    public TextMeshProUGUI subtitleBeat2;   // Teks narasi Beat 2
    public string narasiBeat2 = "Semua makhluk hidup membutuhkan makanan untuk bertahan hidup. Tumbuhan membuat makanan sendiri dengan bantuan sinar matahari.";
    public AudioClip narrasiBeat2Clip;      // Drag audio clip narasi Beat 2

    [Header("Transisi")]
    public float delaySetelasBeat2 = 1.5f; // Jeda sebelum pindah scene

    void Start()
    {
        // Sembunyikan Beat 2 dulu
        if (animasiPanel != null) animasiPanel.SetActive(false);
        if (subtitleText != null) subtitleText.text = narasiBeat1;

        // Dengarkan event narasi selesai dari AudioManager
        if (AudioManager.Instance != null)
            AudioManager.Instance.OnNarrationFinished += OnBeat1Selesai;
        else
            Debug.LogWarning("[OpeningController] AudioManager tidak ditemukan!");
    }

    private void OnBeat1Selesai()
    {
        // Unsubscribe dulu supaya tidak double trigger
        if (AudioManager.Instance != null)
            AudioManager.Instance.OnNarrationFinished -= OnBeat1Selesai;

        StartCoroutine(TransisiBeat2());
    }

    private IEnumerator TransisiBeat2()
    {
        // Sembunyikan Beat 1
        if (judulPanel != null)
            yield return StartCoroutine(FadeOutPanel(judulPanel));

        // Tampilkan Beat 2
        if (animasiPanel != null)
        {
            animasiPanel.SetActive(true);
            yield return StartCoroutine(FadeInPanel(animasiPanel));
        }

        if (subtitleBeat2 != null)
            subtitleBeat2.text = narasiBeat2;

        // Play narasi Beat 2
        if (narrasiBeat2Clip != null && AudioManager.Instance != null)
        {
            AudioManager.Instance.OnNarrationFinished += OnBeat2Selesai;
            // Override clip narasi dengan Beat 2
            AudioManager.Instance.PlayNarrationClip(narrasiBeat2Clip);
        }
        else
        {
            // Tidak ada audio — tunggu sebentar lalu lanjut
            yield return new WaitForSeconds(3f);
            OnBeat2Selesai();
        }
    }

    private void OnBeat2Selesai()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.OnNarrationFinished -= OnBeat2Selesai;

        StartCoroutine(LanjutKeSceneBerikutnya());
    }

    private IEnumerator LanjutKeSceneBerikutnya()
    {
        yield return new WaitForSeconds(delaySetelasBeat2);

        if (SceneController.Instance != null)
            SceneController.Instance.LoadNextScene();
        else
            Debug.LogWarning("[OpeningController] SceneController tidak ditemukan!");
    }

    // Helper fade panel
    private IEnumerator FadeOutPanel(GameObject panel)
    {
        CanvasGroup cg = panel.GetComponent<CanvasGroup>();
        if (cg == null) { panel.SetActive(false); yield break; }

        float timer = 0.5f;
        while (timer > 0)
        {
            timer -= Time.deltaTime;
            cg.alpha = timer / 0.5f;
            yield return null;
        }
        panel.SetActive(false);
    }

    private IEnumerator FadeInPanel(GameObject panel)
    {
        CanvasGroup cg = panel.GetComponent<CanvasGroup>();
        if (cg == null) yield break;

        cg.alpha = 0f;
        float timer = 0f;
        while (timer < 0.5f)
        {
            timer += Time.deltaTime;
            cg.alpha = timer / 0.5f;
            yield return null;
        }
        cg.alpha = 1f;
    }

    void OnDestroy()
    {
        // Cleanup event supaya tidak memory leak
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.OnNarrationFinished -= OnBeat1Selesai;
            AudioManager.Instance.OnNarrationFinished -= OnBeat2Selesai;
        }
    }
}