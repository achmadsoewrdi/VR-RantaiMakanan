using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class SequenceDioramaFinal : MonoBehaviour
{
    [Header("Mekanik Awal (Fase Lari)")]
    public NavMeshAgent ecoAgent;
    public Animator ecoAnimator;
    public Transform titikKumpul;
    public GameObject panahIndikator;
    public Transform kameraPlayer;
    public float radiusDeteksi = 2.5f;

    [Header("Referensi Kuis & Visual")]
    public QuizManager kuisSistem;
    public QuizData dataKuis1, dataKuis2, dataKuis3;
    public Transform visualBabak1, visualBabak2, visualBabak3;
    
    [Header("Referensi Sawah (Untuk Babak 3)")]
    public Transform parentPadi; // Tarik objek induk "Padi" ke sini
    public Color warnaSawahRusak = new Color(0.6f, 0.4f, 0.2f); // Coklat Pudar
    
    private Color warnaSawahAsli;
    private Renderer[] semuaPadiRenderers; // Array untuk menyimpan semua daun padi

    [Header("Audio (Eco)")]
    public AudioSource ecoAudio;
    public AudioClip suaraAyoIkut, suaraBabak1, suaraBabak2, suaraBabak3, suaraAjakanKuis;

    private bool ecoSudahSampai = false;
    private bool userSudahSampai = false;

    void Start()
    {
        // Matikan semua visual di awal
        if (visualBabak1 != null) visualBabak1.localScale = Vector3.zero;
        if (visualBabak2 != null) visualBabak2.localScale = Vector3.zero;
        if (visualBabak3 != null) visualBabak3.localScale = Vector3.zero;

        // --- KODE BARU: Mengambil semua padi di dalam Parent ---
        if (parentPadi != null)
        {
            semuaPadiRenderers = parentPadi.GetComponentsInChildren<Renderer>();
            if (semuaPadiRenderers.Length > 0)
            {
                warnaSawahAsli = semuaPadiRenderers[0].material.color; // Simpan warna hijau aslinya
            }
        }

        // Eco mulai berlari
        if (ecoAgent != null && titikKumpul != null)
        {
            ecoAgent.SetDestination(titikKumpul.position);
            ecoAnimator.SetFloat("Speed", 1f); 
            if (ecoAudio != null && suaraAyoIkut != null) ecoAudio.PlayOneShot(suaraAyoIkut);
        }
    }

    void Update()
    {
        // Cek Eco sampai
        if (!ecoSudahSampai && ecoAgent != null && !ecoAgent.pathPending && ecoAgent.remainingDistance <= ecoAgent.stoppingDistance)
        {
            ecoSudahSampai = true;
            ecoAnimator.SetFloat("Speed", 0f); 
            if (panahIndikator != null) panahIndikator.SetActive(true);
        }

        // Cek User sampai
        if (ecoSudahSampai && !userSudahSampai && kameraPlayer != null)
        {
            if (Vector3.Distance(titikKumpul.position, kameraPlayer.position) <= radiusDeteksi)
            {
                userSudahSampai = true; 
                if (panahIndikator != null) panahIndikator.SetActive(false); 
                
                // USER SAMPAI, MULAI BABAK 1
                StartCoroutine(MulaiBabak1());
            }
        }
    }

    // ==================== BABAK 1 (ENERGI MATAHARI) ====================
    IEnumerator MulaiBabak1()
    {
        EcoTatapUser();
        
        // Jeda 3 Detik setelah User sampai (biar napas dulu)
        yield return new WaitForSeconds(3.0f); 

        // Visual muncul
        StartCoroutine(SmoothScale(visualBabak1, Vector3.zero, Vector3.one));
        
        // Jeda 1.5 detik memandangi matahari sebelum Eco ngomong
        yield return new WaitForSeconds(1.5f); 
        yield return PutarAudio(suaraBabak1);
        
        // Matahari hilang
        StartCoroutine(SmoothScale(visualBabak1, Vector3.one, Vector3.zero));
        
        // Jeda 2 detik meresapi penjelasan sebelum diajak kuis
        yield return new WaitForSeconds(2.0f);
        yield return PutarAudio(suaraAjakanKuis);

        kuisSistem.SetQuizData(dataKuis1);
        kuisSistem.OnQuizSelesai += LanjutKeBabak2; 
        kuisSistem.ShowQuiz();
    }

    void LanjutKeBabak2()
    {
        kuisSistem.OnQuizSelesai -= LanjutKeBabak2; 
        StartCoroutine(MulaiBabak2());
    }

    // ==================== BABAK 2 (PENGURAI) ====================
    IEnumerator MulaiBabak2()
    {
        // Jeda 3 detik menatap sawah kosong setelah Kuis 1 hilang
        yield return new WaitForSeconds(3.0f); 

        // Visual jamur/bakteri muncul
        StartCoroutine(SmoothScale(visualBabak2, Vector3.zero, Vector3.one));
        
        // Jeda 1.5 detik melihat objek sebelum penjelasan
        yield return new WaitForSeconds(1.5f);
        yield return PutarAudio(suaraBabak2);
        
        // Jamur hilang
        StartCoroutine(SmoothScale(visualBabak2, Vector3.one, Vector3.zero));

        // Jeda 2 detik mencerna materi sebelum Kuis 2
        yield return new WaitForSeconds(2.0f);
        yield return PutarAudio(suaraAjakanKuis);

        kuisSistem.SetQuizData(dataKuis2);
        kuisSistem.OnQuizSelesai += LanjutKeBabak3; 
        kuisSistem.ShowQuiz();
    }

    void LanjutKeBabak3()
    {
        kuisSistem.OnQuizSelesai -= LanjutKeBabak3;
        StartCoroutine(MulaiBabak3());
    }

    // ==================== BABAK 3 (KEKACAUAN & KESIMPULAN) ====================
    IEnumerator MulaiBabak3()
    {
        // Jeda 3 detik setelah Kuis 2 hilang
        yield return new WaitForSeconds(3.0f);

        // Transisi Sawah Rusak & Muncul Belalang (diperlama jadi 3 detik agar lebih dramatis)
        StartCoroutine(TransisiWarnaSawah(warnaSawahRusak, 3f));
        StartCoroutine(SmoothScale(visualBabak3, Vector3.zero, Vector3.one));

        // Jeda 2 detik melihat kekacauan sawah sebelum Eco ngomong
        yield return new WaitForSeconds(2.0f);

        // Putar audio narasi panjang
        if (ecoAudio != null && suaraBabak3 != null)
        {
            ecoAudio.clip = suaraBabak3;
            ecoAudio.Play();

            // Tunggu sampai hampir di ujung narasi (-4 detik dari total)
            float waktuTunggu = suaraBabak3.length > 4f ? suaraBabak3.length - 4f : suaraBabak3.length;
            yield return new WaitForSeconds(waktuTunggu);
        }

        // Kesimpulan: Sawah kembali hijau pelan-pelan (3 detik)
        StartCoroutine(TransisiWarnaSawah(warnaSawahAsli, 3f));
        StartCoroutine(SmoothScale(visualBabak3, Vector3.one, Vector3.zero));
        
        // Tunggu sisa audio habis + jeda ekstra 2 detik meresapi "pesan moral" Eco
        yield return new WaitForSeconds(6.0f); 
        
        yield return PutarAudio(suaraAjakanKuis);

        kuisSistem.SetQuizData(dataKuis3);
        kuisSistem.ShowQuiz();
    }


    // ==================== FUNGSI HELPER ====================
    void EcoTatapUser()
    {
        Vector3 arah = kameraPlayer.position - ecoAgent.transform.position;
        arah.y = 0;
        ecoAgent.transform.rotation = Quaternion.LookRotation(arah);
    }

    IEnumerator PutarAudio(AudioClip klip)
    {
        if (ecoAudio != null && klip != null)
        {
            ecoAudio.clip = klip;
            ecoAudio.Play();
            yield return new WaitForSeconds(klip.length);
        }
    }

    IEnumerator SmoothScale(Transform obj, Vector3 start, Vector3 end)
    {
        if (obj == null) yield break;
        obj.gameObject.SetActive(true);
        float t = 0;
        while (t < 1f)
        {
            t += Time.deltaTime;
            obj.localScale = Vector3.Lerp(start, end, t);
            yield return null;
        }
        obj.localScale = end;
    }

    IEnumerator TransisiWarnaSawah(Color targetColor, float durasi)
    {
        if (semuaPadiRenderers == null || semuaPadiRenderers.Length == 0) yield break;

        Color startColor = semuaPadiRenderers[0].material.color;
        float t = 0;
        
        while (t < durasi)
        {
            t += Time.deltaTime;
            Color warnaTransisi = Color.Lerp(startColor, targetColor, t / durasi);
            
            // Terapkan warna ke SEMUA daun padi secara bersamaan
            foreach (Renderer r in semuaPadiRenderers)
            {
                r.material.color = warnaTransisi;
            }
            yield return null;
        }

        // Pastikan warna akhir pas di target
        foreach (Renderer r in semuaPadiRenderers)
        {
            r.material.color = targetColor;
        }
    }
}