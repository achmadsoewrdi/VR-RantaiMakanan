using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Mengelola semua audio dalam satu scene: narasi dan ambience.
/// Letakkan script ini di GameObject "AudioManager" di dalam [Managers].
/// </summary>
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    [Tooltip("AudioSource khusus untuk narasi (voice over).")]
    [SerializeField] private AudioSource narrationSource;

    [Tooltip("AudioSource khusus untuk suara latar (ambience/background).")]
    [SerializeField] private AudioSource ambienceSource;

    [Header("Audio Clips")]
    [Tooltip("Clip narasi untuk scene ini. Drag file audio narasi ke sini.")]
    [SerializeField] private AudioClip narrationClip;

    [Tooltip("Clip suara latar untuk scene ini. Drag file audio ambience ke sini.")]
    [SerializeField] private AudioClip ambienceClip;

    [Header("Settings")]
    [Range(0f, 1f)]
    [SerializeField] private float narrationVolume = 1.0f;
    [Range(0f, 1f)]
    [SerializeField] private float ambienceVolume = 0.3f;

    // Event dipanggil ketika narasi selesai — QuizManager mendengarkan ini
    public event Action OnNarrationFinished;

    // -------------------------------------------------------

    private void Awake()
    {
        // Singleton sederhana — hanya ada 1 AudioManager per scene
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        PlayAmbience();
        PlayNarration();
    }

    // -------------------------------------------------------
    // PUBLIC METHODS
    // -------------------------------------------------------

    /// <summary>Putar narasi scene ini.</summary>
    public void PlayNarration()
    {
        if (narrationClip == null)
        {
            Debug.LogWarning("[AudioManager] Narration clip belum di-assign!");
            // Tetap trigger event supaya quiz tidak tertahan
            OnNarrationFinished?.Invoke();
            return;
        }

        narrationSource.volume = narrationVolume;
        narrationSource.clip = narrationClip;
        narrationSource.Play();

        // Tunggu durasi clip lalu trigger event selesai
        StartCoroutine(WaitForNarrationEnd(narrationClip.length));
    }

    /// <summary>Putar suara latar (loop otomatis).</summary>
    public void PlayAmbience()
    {
        if (ambienceClip == null)
        {
            Debug.LogWarning("[AudioManager] Ambience clip belum di-assign!");
            return;
        }

        ambienceSource.volume = ambienceVolume;
        ambienceSource.clip = ambienceClip;
        ambienceSource.loop = true;
        ambienceSource.Play();
    }

    // ← METHOD BARU: tidak mengubah apapun di atas
    /// <summary>Play narasi dengan clip spesifik (untuk multi-beat).</summary>
    public void PlayNarrationClip(AudioClip clip)
    {
        if (clip == null) return;

        narrationSource.Stop();
        StopAllCoroutines();

        narrationSource.volume = narrationVolume;
        narrationSource.clip = clip;
        narrationSource.Play();

        StartCoroutine(WaitForNarrationEnd(clip.length));
    }

    /// <summary>Hentikan semua audio.</summary>
    public void StopAll()
    {
        narrationSource.Stop();
        ambienceSource.Stop();
        StopAllCoroutines();
    }

    // -------------------------------------------------------
    // PRIVATE
    // -------------------------------------------------------

    private IEnumerator WaitForNarrationEnd(float duration)
    {
        yield return new WaitForSeconds(duration);
        OnNarrationFinished?.Invoke();
        Debug.Log("[AudioManager] Narasi selesai.");
    }
}
