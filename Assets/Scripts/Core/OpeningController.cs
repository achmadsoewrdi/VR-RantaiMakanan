using UnityEngine;
using TMPro;
using System.Collections;

public class OpeningController : MonoBehaviour
{
    [Header("Beat 1 — Judul")]
    public GameObject judulPanel;
    public GameObject animasiPanelBackground; // Drag AnimasiPanel (panel coklat)
    public TextMeshProUGUI subtitleText;
    public string narasiBeat1 = "Halo anak-anak! Hari ini kita akan belajar bersama";

    [Header("Beat 2 — Animasi 3D")]
    public GameObject animasi3DContainer;
    public GameObject subtitleBeat2Panel;
    public TextMeshProUGUI subtitleBeat2;
    public string narasiBeat2 = "Semua makhluk hidup membutuhkan makanan untuk bertahan hidup. Tumbuhan membuat makanan sendiri dengan bantuan sinar matahari.";
    public AudioClip narrasiBeat2Clip;

    [Header("Animasi Keluar 3D")]
    [Tooltip("Animasi 3D terbang ke atas saat keluar")]
    public float animasi3DKeluarDuration = 1.0f;
    public float animasi3DKeluarOffsetY = 5f; // Seberapa jauh naik ke atas

    [Header("Transisi")]
    public float fadeInDuration = 1.0f;
    public float fadeOutDuration = 0.5f;
    public float delaySetelasBeat2 = 1.5f;

    void Start()
    {
        // Sembunyikan semua Beat 2
        if (animasi3DContainer != null) animasi3DContainer.SetActive(false);
        if (subtitleBeat2Panel != null) subtitleBeat2Panel.SetActive(false);
        if (subtitleText != null) subtitleText.text = narasiBeat1;

        // Set alpha 0 untuk semua panel yang akan fade in
        SetAlpha(judulPanel, 0f);
        SetAlpha(animasiPanelBackground, 0f);

        if (judulPanel != null) judulPanel.SetActive(true);
        if (animasiPanelBackground != null) animasiPanelBackground.SetActive(true);

        StartCoroutine(MulaiDenganFadeIn());
    }

    private IEnumerator MulaiDenganFadeIn()
    {
        // Fade in panel coklat DAN judul bersamaan
        StartCoroutine(FadeInPanel(animasiPanelBackground, fadeInDuration));
        yield return StartCoroutine(FadeInPanel(judulPanel, fadeInDuration));

        if (AudioManager.Instance != null)
            AudioManager.Instance.OnNarrationFinished += OnBeat1Selesai;
        else
            Debug.LogWarning("[OpeningController] AudioManager tidak ditemukan!");
    }

    private void OnBeat1Selesai()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.OnNarrationFinished -= OnBeat1Selesai;

        StartCoroutine(TransisiBeat2());
    }

    private IEnumerator TransisiBeat2()
    {
        // Fade out panel coklat DAN judul bersamaan
        StartCoroutine(FadeOutPanel(animasiPanelBackground, fadeOutDuration));
        yield return StartCoroutine(FadeOutPanel(judulPanel, fadeOutDuration));

        // Aktifkan asset 3D
        if (animasi3DContainer != null)
            animasi3DContainer.SetActive(true);

        // Fade in subtitle Beat 2
        if (subtitleBeat2Panel != null)
        {
            SetAlpha(subtitleBeat2Panel, 0f);
            subtitleBeat2Panel.SetActive(true);
            yield return StartCoroutine(FadeInPanel(subtitleBeat2Panel, fadeInDuration));
        }

        if (subtitleBeat2 != null)
            subtitleBeat2.text = narasiBeat2;

        if (narrasiBeat2Clip != null && AudioManager.Instance != null)
        {
            AudioManager.Instance.OnNarrationFinished += OnBeat2Selesai;
            AudioManager.Instance.PlayNarrationClip(narrasiBeat2Clip);
        }
        else
        {
            Debug.LogWarning("[OpeningController] Audio Beat 2 belum di-assign, simulasi 5 detik.");
            yield return new WaitForSeconds(5f);
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
        // Fade out subtitle Beat 2
        if (subtitleBeat2Panel != null)
            yield return StartCoroutine(FadeOutPanel(subtitleBeat2Panel, fadeOutDuration));

        // Animasi keluar 3D — terbang ke atas
        if (animasi3DContainer != null)
            yield return StartCoroutine(Animasi3DKeluar());

        yield return new WaitForSeconds(delaySetelasBeat2);

        if (SceneController.Instance != null)
            SceneController.Instance.LoadNextScene();
        else
            Debug.LogWarning("[OpeningController] SceneController tidak ditemukan!");
    }

    // Animasi 3D container terbang ke atas lalu fade out
    private IEnumerator Animasi3DKeluar()
    {
        Vector3 posisiAwal = animasi3DContainer.transform.position;
        Vector3 posisiTujuan = posisiAwal + new Vector3(0, animasi3DKeluarOffsetY, 0);

        // Cari semua Renderer untuk fade out
        Renderer[] renderers = animasi3DContainer.GetComponentsInChildren<Renderer>();

        // Simpan material asli dan set ke mode fade
        float timer = 0f;
        while (timer < animasi3DKeluarDuration)
        {
            timer += Time.deltaTime;
            float progress = Mathf.Clamp01(timer / animasi3DKeluarDuration);

            // Gerak ke atas
            animasi3DContainer.transform.position = Vector3.Lerp(posisiAwal, posisiTujuan, progress);

            // Fade out tiap renderer
            foreach (Renderer r in renderers)
            {
                foreach (Material mat in r.materials)
                {
                    if (mat.HasProperty("_Color"))
                    {
                        Color c = mat.color;
                        c.a = 1f - progress;
                        mat.color = c;
                    }
                }
            }

            yield return null;
        }

        animasi3DContainer.SetActive(false);

        // Reset posisi untuk jaga-jaga
        animasi3DContainer.transform.position = posisiAwal;
    }

    // -------------------------------------------------------
    // HELPER
    // -------------------------------------------------------

    private void SetAlpha(GameObject panel, float alpha)
    {
        if (panel == null) return;
        CanvasGroup cg = panel.GetComponent<CanvasGroup>();
        if (cg == null) cg = panel.AddComponent<CanvasGroup>();
        cg.alpha = alpha;
    }

    private IEnumerator FadeInPanel(GameObject panel, float duration)
    {
        if (panel == null) yield break;
        CanvasGroup cg = panel.GetComponent<CanvasGroup>();
        if (cg == null) yield break;

        float timer = 0f;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            cg.alpha = Mathf.Clamp01(timer / duration);
            yield return null;
        }
        cg.alpha = 1f;
    }

    private IEnumerator FadeOutPanel(GameObject panel, float duration)
    {
        if (panel == null) yield break;
        CanvasGroup cg = panel.GetComponent<CanvasGroup>();
        if (cg == null) { panel.SetActive(false); yield break; }

        float timer = duration;
        while (timer > 0)
        {
            timer -= Time.deltaTime;
            cg.alpha = Mathf.Clamp01(timer / duration);
            yield return null;
        }
        cg.alpha = 0f;
        panel.SetActive(false);
    }

    void OnDestroy()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.OnNarrationFinished -= OnBeat1Selesai;
            AudioManager.Instance.OnNarrationFinished -= OnBeat2Selesai;
        }
    }
}