using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI; // Wajib ditambahkan untuk memanggil komponen Image

public class GazeController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float gazeDistance = 10f;
    [SerializeField] private LayerMask interactableLayer;

    [Header("Reticle (titik tengah layar)")]
    [SerializeField] private Transform reticle;
    [SerializeField] private float reticleIdleScale = 0.02f;
    [SerializeField] private float reticleActiveScale = 0.025f;

    // --- VARIABEL BARU UNTUK RING LOADING ---
    [Tooltip("Masukkan objek RingLoading yang ada di Canvas Reticle")]
    [SerializeField] private Image ringLoadingImage; 

    private IGazeInteractable _currentTarget;
    private float _gazeTimer = 0f;

    private void Update()
    {
        PerformGazeRaycast();
    }

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
                    
                    // Kirim persentase (0.0 sampai 1.0) ke animasi UI
                    UpdateReticleProgress(_gazeTimer / _currentTarget.GazeDuration);

                    if (_gazeTimer >= _currentTarget.GazeDuration)
                    {
                        _currentTarget.OnGazeSelect();
                        ResetGaze();
                    }
                }
                else
                {
                    // Pindah menatap kotak lain — reset dan mulai lagi
                    ResetGaze();
                    _currentTarget = target;
                    _currentTarget.OnGazeEnter();
                    SetReticleActive(true);
                }
                return;
            }
        }

        // Mata menatap udara kosong — reset
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
        UpdateReticleProgress(0f); // Kosongkan ring loading saat reset
    }

    private void SetReticleActive(bool active)
    {
        if (reticle != null)
        {
            float scale = active ? reticleActiveScale : reticleIdleScale;
            reticle.localScale = Vector3.one * scale;
        }

        // Kalau mata tidak menatap tombol, pastikan ring loading hilang
        if (!active && ringLoadingImage != null)
        {
            ringLoadingImage.fillAmount = 0f;
        }
    }

    private void UpdateReticleProgress(float progress)
    {
        if (reticle != null)
        {
            float scale = Mathf.Lerp(reticleIdleScale, reticleActiveScale * 1.5f, progress);
            reticle.localScale = Vector3.one * scale;
        }

        // --- KODE BARU UNTUK MENGISI ANIMASI MUTER ---
        if (ringLoadingImage != null)
        {
            // Mathf.Clamp01 memastikan angkanya mentok dari 0 sampai 1
            ringLoadingImage.fillAmount = Mathf.Clamp01(progress); 
        }
    }
}