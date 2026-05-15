using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EcoSequenceManager : MonoBehaviour
{
    [Header("Referensi Fase 1 & 2")]
    public NavMeshAgent ecoAgent;
    public Animator ecoAnimator;
    public Transform titikKumpul;
    public GameObject panahIndikator;
    public Transform kameraPlayer;
    public float radiusDeteksi = 2.5f;

    [Header("Referensi Fase 3 (Narasi)")]
    public AudioSource ecoAudioSource;
    public AudioClip klipNarasi;
    public AudioClip klipAjakanKuis;
    
    [Header("Referensi Transisi Smooth")]
    public Transform grupHewanTransform; // Ganti ke Transform objek Grup_Hewan
    public CanvasGroup canvasPenjelasanGroup; // Ganti ke komponen CanvasGroup
    public float durasiTransisiSmooth = 1.0f; // Kecepatan muncul/hilang (detik)

    private bool ecoSudahSampai = false;
    private bool userSudahSampai = false;

    void Start()
    {
        // Pastikan hewan berukuran 0 di awal (jika lupa setting di Inspector)
        if (grupHewanTransform != null) grupHewanTransform.localScale = Vector3.zero;
        // Pastikan Canvas transparan di awal
        if (canvasPenjelasanGroup != null) canvasPenjelasanGroup.alpha = 0f;

        if (ecoAgent != null && titikKumpul != null)
        {
            ecoAgent.SetDestination(titikKumpul.position);
            ecoAnimator.SetFloat("Speed", 1f); 
        }
    }

    void Update()
    {
        if (!ecoSudahSampai && ecoAgent != null)
        {
            if (!ecoAgent.pathPending && ecoAgent.remainingDistance <= ecoAgent.stoppingDistance)
            {
                ecoSudahSampai = true;
                ecoAnimator.SetFloat("Speed", 0f); 
                if (panahIndikator != null) panahIndikator.SetActive(true);
            }
        }

        if (ecoSudahSampai && !userSudahSampai && kameraPlayer != null)
        {
            float jarak = Vector3.Distance(titikKumpul.position, kameraPlayer.position);
            if (jarak <= radiusDeteksi)
            {
                userSudahSampai = true; 
                if (panahIndikator != null) panahIndikator.SetActive(false); 
                
                StartCoroutine(MainkanFase3Smooth());
            }
        }
    }

    // --- COROUTINE SUTRADARA FASE 3 (SMOOTH) ---
    IEnumerator MainkanFase3Smooth()
    {
        // 1. Eco Menatap User
        Vector3 arahTatap = kameraPlayer.position - ecoAgent.transform.position;
        arahTatap.y = 0; 
        ecoAgent.transform.rotation = Quaternion.LookRotation(arahTatap);

        // 2. [MUNCUL HALUS] - Hewan membesar, Teks memudar masuk
        // Kita jalankan dua Coroutine sekaligus (Parallel) agar berjalan bersamaan
        StartCoroutine(SmoothScaleObject(grupHewanTransform, Vector3.zero, Vector3.one)); // Dari 0 ke 1
        StartCoroutine(SmoothFadeCanvas(canvasPenjelasanGroup, 0f, 1f)); // Dari transparan ke muncul

        // Beri waktu delay agar User melihat kemunculannya
        yield return new WaitForSeconds(1.5f); 

        // 3. Mainkan Audio Narasi
        if (ecoAudioSource != null && klipNarasi != null)
        {
            ecoAudioSource.clip = klipNarasi;
            ecoAudioSource.Play();
            yield return new WaitForSeconds(klipNarasi.length);
        }

        // [MUNCUL HALUS SELESAI] - Matikan Hewan
        // Kita jalankan dua Coroutine sekaligus untuk menghilang (Parallel)
        StartCoroutine(SmoothScaleObject(grupHewanTransform, Vector3.one, Vector3.zero)); // Dari 1 ke 0
        StartCoroutine(SmoothFadeCanvas(canvasPenjelasanGroup, 1f, 0f)); // Dari muncul ke transparan

        // Tunggu transisi menghilang selesai sebelum lanjut
        yield return new WaitForSeconds(durasiTransisiSmooth); 

        // [TAMBAHAN JEDA]
        yield return new WaitForSeconds(1.0f); 

        // 4. Mainkan Audio Ajakan Kuis
        if (ecoAudioSource != null && klipAjakanKuis != null)
        {
            ecoAudioSource.clip = klipAjakanKuis;
            ecoAudioSource.Play();
            yield return new WaitForSeconds(klipAjakanKuis.length);
        }

        Debug.Log("Fase 3 Selesai! Masuk Fase 4.");

        QuizManager kuisSistem = FindAnyObjectByType<QuizManager>();
        if (kuisSistem != null)
        {
            kuisSistem.ShowQuiz();
        }
    }


    // =========================================================
    // --- FUNGSI HELPER UNTUK TRANSISI HALUS (LERP) ---
    // =========================================================

    // Fungsi untuk membesarkan/mengecilkan objek 3D secara halus
    IEnumerator SmoothScaleObject(Transform objTransform, Vector3 startScale, Vector3 endScale)
    {
        if (objTransform == null) yield break;

        float currentTime = 0f;
        while (currentTime < durasiTransisiSmooth)
        {
            currentTime += Time.deltaTime;
            // Menghitung perubahan ukuran berdasarkan waktu
            float normalTime = currentTime / durasiTransisiSmooth;
            objTransform.localScale = Vector3.Lerp(startScale, endScale, normalTime);
            yield return null; // Tunggu ke frame berikutnya
        }
        // Pastikan ukurannya pas di angka target di akhir
        objTransform.localScale = endScale;
    }

    // Fungsi untuk membuat UI Canvas memudar/muncul secara halus
    IEnumerator SmoothFadeCanvas(CanvasGroup cg, float startAlpha, float endAlpha)
    {
        if (cg == null) yield break;

        float currentTime = 0f;
        while (currentTime < durasiTransisiSmooth)
        {
            currentTime += Time.deltaTime;
            float normalTime = currentTime / durasiTransisiSmooth;
            cg.alpha = Mathf.Lerp(startAlpha, endAlpha, normalTime);
            yield return null;
        }
        cg.alpha = endAlpha;
    }

    private void OnDrawGizmos()
    {
        if (titikKumpul != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(titikKumpul.position, radiusDeteksi);
        }
    }
}