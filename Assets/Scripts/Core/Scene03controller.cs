using System.Collections;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Scene 03 - Produsen
/// PENTING: Script ini SATU-SATUNYA yang mengontrol Eco di Scene 03.
/// Jangan pasang EcoSequenceManager bersamaan — akan konflik.
/// 
/// Pasang di: [Managers] → Scene03Controller (GameObject)
/// TitikKumpul: hanya berisi Transform, tidak ada script apapun.
/// </summary>
public class Scene03Controller : MonoBehaviour
{
    [Header("=== Eco Character ===")]
    public NavMeshAgent ecoAgent;
    public Animator ecoAnimator;

    [Header("=== Navigasi ===")]
    public Transform titikKumpul;
    public GameObject panahIndikator;
    public Transform kameraPlayer;
    public float radiusDeteksi = 2.5f;

    [Header("=== Audio Dialog ===")]
    public AudioSource ecoAudioSource;
    public AudioClip dialog1_Sapa;
    public AudioClip dialog2_LihatLingkungan;
    public AudioClip dialog3_SalingMembutuhkan;
    public AudioClip dialog4_BeniReaksi;
    public AudioClip klipAjakanKuis;

    [Header("=== Visual Hewan ===")]
    public Transform grupHewanTransform;
    public float durasiTransisiHewan = 1.0f;

    [Header("=== UI Subtitle (Opsional) ===")]
    public CanvasGroup canvasSubtitleGroup;

    [Header("=== Timing ===")]
    public float jedaAwal = 1.5f;
    public float jedaSetelahPlayerSampai = 1.0f;
    public float jedaAntarDialog = 0.8f;

    // ── State ──────────────────────────────────────────────
    private bool ecoSudahSampai   = false;
    private bool playerSudahDekat = false;
    private bool dialogSudahJalan = false;

    // ══════════════════════════════════════════════════════
    // LIFECYCLE
    // ══════════════════════════════════════════════════════

    private void Start()
    {
        if (grupHewanTransform != null)
            grupHewanTransform.localScale = Vector3.zero;

        if (canvasSubtitleGroup != null)
            canvasSubtitleGroup.alpha = 0f;

        if (panahIndikator != null)
            panahIndikator.SetActive(false);

        StartCoroutine(MulaiScene());
    }

    private void Update()
    {
        CekEcoSampai();
        CekPlayerDekat();
    }

    // ══════════════════════════════════════════════════════
    // CEK STATE
    // ══════════════════════════════════════════════════════

// Ganti CekEcoSampai() dengan ini:
private void CekEcoSampai()
{
    if (ecoSudahSampai) return;
    if (ecoAgent == null || titikKumpul == null) return;
    if (!ecoAgent.isOnNavMesh) return;

    // Pakai distance langsung — sama persis dengan cara cek player
    float jarak = Vector3.Distance(
        ecoAgent.transform.position, 
        titikKumpul.position
    );

    if (jarak <= 0.6f) // threshold 1.5m, sesuaikan jika perlu
    {
        EcoSampaiTitikKumpul();
    }
}

    private void CekPlayerDekat()
    {
        if (!ecoSudahSampai) return;
        if (playerSudahDekat) return;
        if (kameraPlayer == null) return;

        float jarak = Vector3.Distance(titikKumpul.position, kameraPlayer.position);
        if (jarak <= radiusDeteksi)
            PlayerSudahDekat();
    }

    // ══════════════════════════════════════════════════════
    // PHASE 1 — Eco mulai berlari
    // ══════════════════════════════════════════════════════

    private IEnumerator MulaiScene()
    {
        yield return new WaitForSeconds(jedaAwal);

        if (ecoAgent == null || titikKumpul == null)
        {
            Debug.LogError("[Scene03] ecoAgent atau titikKumpul belum diassign!");
            yield break;
        }

        // Tunggu NavMesh aktif (max 5 detik)
        float timeout = 5f, elapsed = 0f;
        while (!ecoAgent.isOnNavMesh && elapsed < timeout)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }

        if (!ecoAgent.isOnNavMesh)
        {
            Debug.LogError("[Scene03] Eco TIDAK di NavMesh! Bake NavMesh & cek posisi Y Eco.");
            yield break;
        }

        ecoAgent.SetDestination(titikKumpul.position);
        SetAnimasiEco(isWalking: true);
        Debug.Log("[Scene03] ✓ Eco mulai berlari.");
    }

    // ══════════════════════════════════════════════════════
    // PHASE 2 — Eco tiba, tunggu player
    // ══════════════════════════════════════════════════════

private void EcoSampaiTitikKumpul()
{
    ecoSudahSampai = true;

    ecoAgent.isStopped = true;
    ecoAgent.ResetPath();
    ecoAgent.velocity = Vector3.zero;

    SetAnimasiEco(isWalking: false);

    if (panahIndikator != null)
        panahIndikator.SetActive(true);

    Debug.Log("[Scene03] ✓ Eco sampai.");
}

    // ══════════════════════════════════════════════════════
    // PHASE 3 — Player dekat
    // ══════════════════════════════════════════════════════

    private void PlayerSudahDekat()
    {
        if (dialogSudahJalan) return; // anti double-trigger
        playerSudahDekat = true;
        dialogSudahJalan = true;

        if (panahIndikator != null)
            panahIndikator.SetActive(false);

        StartCoroutine(SequenceDialog());
        Debug.Log("[Scene03] ✓ Player dekat. Dialog dimulai.");
    }

    // ══════════════════════════════════════════════════════
    // PHASE 4 — Sequence Dialog
    // ══════════════════════════════════════════════════════

    private IEnumerator SequenceDialog()
    {
        yield return new WaitForSeconds(jedaSetelahPlayerSampai);
        EcoTatapPlayer();

        // Beat 1 — Sapa
        Debug.Log("[Scene03] Beat 1: Sapa");
        yield return StartCoroutine(PutarDialog(dialog1_Sapa));
        yield return new WaitForSeconds(jedaAntarDialog);

        // Beat 2 — Lihat lingkungan + hewan muncul
        Debug.Log("[Scene03] Beat 2: Lihat lingkungan");
        StartCoroutine(SmoothScale(grupHewanTransform, Vector3.zero, Vector3.one, durasiTransisiHewan));
        StartCoroutine(SmoothFade(canvasSubtitleGroup, 0f, 1f, durasiTransisiHewan));
        yield return StartCoroutine(PutarDialog(dialog2_LihatLingkungan));
        yield return new WaitForSeconds(jedaAntarDialog);

        // Beat 3 — Saling membutuhkan → hewan hilang
        Debug.Log("[Scene03] Beat 3: Saling membutuhkan");
        yield return StartCoroutine(PutarDialog(dialog3_SalingMembutuhkan));
        StartCoroutine(SmoothScale(grupHewanTransform, Vector3.one, Vector3.zero, durasiTransisiHewan));
        StartCoroutine(SmoothFade(canvasSubtitleGroup, 1f, 0f, durasiTransisiHewan));
        yield return new WaitForSeconds(durasiTransisiHewan + jedaAntarDialog);

        // Beat 4 — Reaksi Beni
        Debug.Log("[Scene03] Beat 4: Reaksi Beni");
        yield return StartCoroutine(PutarDialog(dialog4_BeniReaksi));
        yield return new WaitForSeconds(jedaAntarDialog);

        // Beat 5 — Ajakan kuis
        Debug.Log("[Scene03] Beat 5: Ajakan kuis");
        yield return StartCoroutine(PutarDialog(klipAjakanKuis));
        yield return new WaitForSeconds(0.5f);

        TriggerKuis();
    }

    // ══════════════════════════════════════════════════════
    // PHASE 5 — Quiz
    // ══════════════════════════════════════════════════════

    private void TriggerKuis()
    {
        QuizManager kuisSistem = FindAnyObjectByType<QuizManager>();
        if (kuisSistem != null)
        {
            kuisSistem.ShowQuiz();
            Debug.Log("[Scene03] ✓ ShowQuiz() dipanggil.");
        }
        else
        {
            Debug.LogError("[Scene03] QuizManager tidak ditemukan!");
        }
    }

    // ══════════════════════════════════════════════════════
    // HELPERS
    // ══════════════════════════════════════════════════════

    private void SetAnimasiEco(bool isWalking)
    {
        if (ecoAnimator != null)
            ecoAnimator.SetFloat("Speed", isWalking ? 1f : 0f);
    }

    private void EcoTatapPlayer()
    {
        if (ecoAgent == null || kameraPlayer == null) return;
        Vector3 arah = kameraPlayer.position - ecoAgent.transform.position;
        arah.y = 0f;
        if (arah.sqrMagnitude > 0.001f)
            ecoAgent.transform.rotation = Quaternion.LookRotation(arah);
    }

    private IEnumerator PutarDialog(AudioClip klip)
    {
        if (ecoAudioSource == null || klip == null)
        {
            Debug.LogWarning("[Scene03] PutarDialog: clip atau AudioSource null, skip.");
            yield break;
        }
        ecoAudioSource.clip = klip;
        ecoAudioSource.Play();
        yield return new WaitForSeconds(klip.length);
    }

    private IEnumerator SmoothScale(Transform obj, Vector3 dari, Vector3 ke, float durasi)
    {
        if (obj == null) yield break;
        float t = 0f;
        while (t < durasi)
        {
            t += Time.deltaTime;
            obj.localScale = Vector3.Lerp(dari, ke, t / durasi);
            yield return null;
        }
        obj.localScale = ke;
    }

    private IEnumerator SmoothFade(CanvasGroup cg, float dari, float ke, float durasi)
    {
        if (cg == null) yield break;
        float t = 0f;
        while (t < durasi)
        {
            t += Time.deltaTime;
            cg.alpha = Mathf.Lerp(dari, ke, t / durasi);
            yield return null;
        }
        cg.alpha = ke;
    }

    private void OnDrawGizmos()
    {
        if (titikKumpul != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(titikKumpul.position, radiusDeteksi);
        }
    }
}