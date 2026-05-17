using System.Collections;
using UnityEngine;

public class SequenceSceneKatak : MonoBehaviour
{
    [Header("Referensi Objek")]
    public Transform grupHewan;
    public CanvasGroup canvasPenjelasanGroup;
    public AudioSource ecoAudioSource;
    public AudioClip klipNarasi;
    public AudioClip klipAjakanKuis;

    [Header("Pengaturan Animasi")]
    public float durasiTransisi = 1.0f;

    void Start()
    {
        // 1. Set awal: sembunyikan hewan dan teks agar bisa muncul smooth
        if (grupHewan != null) grupHewan.localScale = Vector3.zero;
        if (canvasPenjelasanGroup != null) canvasPenjelasanGroup.alpha = 0f;

        // 2. Langsung jalankan cerita sesaat setelah scene loading
        StartCoroutine(JalankanCeritaKatak());
    }

    IEnumerator JalankanCeritaKatak()
    {
        // Jeda sejenak saat scene baru mulai agar User bisa melihat sekeliling dulu
        yield return new WaitForSeconds(1.5f);

        // --- MUNCULKAN HEWAN & TEKS (SMOOTH) ---
        StartCoroutine(SmoothScale(grupHewan, Vector3.zero, Vector3.one));
        StartCoroutine(SmoothFade(canvasPenjelasanGroup, 0f, 1f));

        // --- MAINKAN NARASI ---
        if (ecoAudioSource != null && klipNarasi != null)
        {
            ecoAudioSource.clip = klipNarasi;
            ecoAudioSource.Play();
            // Tunggu sampai narasi Eco benar-benar selesai
            yield return new WaitForSeconds(klipNarasi.length);
        }

        // --- HILANGKAN HEWAN & TEKS (SMOOTH) ---
        StartCoroutine(SmoothScale(grupHewan, Vector3.one, Vector3.zero));
        StartCoroutine(SmoothFade(canvasPenjelasanGroup, 1f, 0f));
        
        // Tunggu animasi menghilang selesai sebelum lanjut
        yield return new WaitForSeconds(durasiTransisi);

        // --- MAINKAN SUARA AJAKAN KUIS & MUNCULKAN KUIS BERSAMAAN ---
        if (ecoAudioSource != null && klipAjakanKuis != null)
        {
            ecoAudioSource.clip = klipAjakanKuis;
            ecoAudioSource.Play();
        }

        // Panggil sistem kuis (otomatis memunculkan panel)
        QuizManager kuisSistem = FindAnyObjectByType<QuizManager>();
        if (kuisSistem != null)
        {
            kuisSistem.ShowQuiz();
        }

        // Selesai! Titik putih Gaze, Countdown 3 detik, dan Feedback 
        // akan otomatis dijalankan oleh GazeController dan QuizManager milikmu!
    }

    // ======================================================
    // FUNGSI HELPER UNTUK ANIMASI HALUS (LERP)
    // ======================================================
    IEnumerator SmoothScale(Transform obj, Vector3 start, Vector3 end)
    {
        if (obj == null) yield break;
        float time = 0;
        while (time < durasiTransisi)
        {
            time += Time.deltaTime;
            obj.localScale = Vector3.Lerp(start, end, time / durasiTransisi);
            yield return null;
        }
        obj.localScale = end;
    }

    IEnumerator SmoothFade(CanvasGroup cg, float start, float end)
    {
        if (cg == null) yield break;
        float time = 0;
        while (time < durasiTransisi)
        {
            time += Time.deltaTime;
            cg.alpha = Mathf.Lerp(start, end, time / durasiTransisi);
            yield return null;
        }
        cg.alpha = end;
    }
}