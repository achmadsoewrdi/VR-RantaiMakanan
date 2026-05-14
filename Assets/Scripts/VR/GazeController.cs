using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Mengontrol input berbasis tatapan (gaze) untuk Google Cardboard.
/// Letakkan script ini di GameObject yang sama dengan Main Camera.
///
/// Cara kerja:
/// - Raycast dari tengah kamera ke depan
/// - Jika mengenai objek dengan IGazeInteractable, mulai hitung durasi
/// - Setelah durasi terpenuhi (gazeSelectDuration), trigger OnGazeSelect
/// </summary>
public class GazeController : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("Jarak maksimal raycast gaze (meter).")]
    [SerializeField] private float gazeDistance = 10f;

    [Tooltip("Layer yang bisa diinteraksi dengan gaze.")]
    [SerializeField] private LayerMask interactableLayer;

    [Header("Reticle (titik tengah layar)")]
    [Tooltip("Transform reticle — objek kecil di tengah layar sebagai pointer.")]
    [SerializeField] private Transform reticle;

    [Tooltip("Ukuran reticle saat idle.")]
    [SerializeField] private float reticleIdleScale = 0.02f;

    [Tooltip("Ukuran reticle saat mengarah ke interactable.")]
    [SerializeField] private float reticleActiveScale = 0.025f;

    // -------------------------------------------------------

    private IGazeInteractable _currentTarget;
    private float _gazeTimer = 0f;
    private bool _isGazing = false;

    // -------------------------------------------------------

    private void Update()
    {
        PerformGazeRaycast();
    }

    // -------------------------------------------------------

    private void PerformGazeRaycast()
    {
        Ray ray = new Ray(transform.position, transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, gazeDistance, interactableLayer))
        {
            IGazeInteractable target = hit.collider.GetComponent<IGazeInteractable>();

            if (target != null)
            {
                // Masih menatap target yang sama
                if (target == _currentTarget)
                {
                    _gazeTimer += Time.deltaTime;
                    UpdateReticleProgress(_gazeTimer / _currentTarget.GazeDuration);

                    if (_gazeTimer >= _currentTarget.GazeDuration)
                    {
                        _currentTarget.OnGazeSelect();
                        ResetGaze();
                    }
                }
                else
                {
                    // Target baru — reset dan mulai lagi
                    ResetGaze();
                    _currentTarget = target;
                    _currentTarget.OnGazeEnter();
                    SetReticleActive(true);
                }
                return;
            }
        }

        // Tidak ada target — reset
        if (_currentTarget != null)
        {
            _currentTarget.OnGazeExit();
            ResetGaze();
            SetReticleActive(false);
        }
    }

    private void ResetGaze()
    {
        _gazeTimer = 0f;
        _currentTarget = null;
        UpdateReticleProgress(0f);
    }

    private void SetReticleActive(bool active)
    {
        if (reticle == null) return;
        float scale = active ? reticleActiveScale : reticleIdleScale;
        reticle.localScale = Vector3.one * scale;
    }

    private void UpdateReticleProgress(float progress)
    {
        // Bisa dikembangkan: isi progress ring pada reticle
        // Untuk sekarang cukup scale sebagai feedback visual
        if (reticle == null) return;
        float scale = Mathf.Lerp(reticleIdleScale, reticleActiveScale * 1.5f, progress);
        reticle.localScale = Vector3.one * scale;
    }
}
