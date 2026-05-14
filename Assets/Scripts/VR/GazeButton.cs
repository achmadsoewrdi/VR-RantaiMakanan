using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Komponen untuk tombol yang dipilih via gaze.
/// Letakkan script ini di GameObject tombol (Option A / Option B) di QuizPanel.
///
/// Requires: Collider (untuk raycast) dan Image (untuk visual progress).
/// </summary>
[RequireComponent(typeof(Collider))]
public class GazeButton : MonoBehaviour, IGazeInteractable
{
    [Header("Visual")]
    [Tooltip("Image yang akan diisi sebagai progress bar saat user menatap.")]
    [SerializeField] private Image progressFill;

    [Tooltip("Warna tombol saat normal.")]
    [SerializeField] private Color normalColor = new Color(0.2f, 0.6f, 1f, 0.9f);

    [Tooltip("Warna tombol saat sedang di-gaze.")]
    [SerializeField] private Color gazeColor = new Color(1f, 0.8f, 0.2f, 1f);

    [SerializeField] private Image buttonBackground;

    // -------------------------------------------------------

    // Event dipanggil saat tombol ini dipilih
    public event Action OnSelected;

    private float _gazeDuration = 2f;
    private bool _interactable = true;
    private float _currentProgress = 0f;

    // -------------------------------------------------------
    // IGazeInteractable implementation
    // -------------------------------------------------------

    public float GazeDuration => _gazeDuration;

    public void OnGazeEnter()
    {
        if (!_interactable) return;
        if (buttonBackground != null)
            buttonBackground.color = gazeColor;
        Debug.Log($"[GazeButton] Gaze masuk: {gameObject.name}");
    }

    public void OnGazeExit()
    {
        _currentProgress = 0f;
        UpdateProgressFill(0f);
        if (buttonBackground != null)
            buttonBackground.color = normalColor;
    }

    public void OnGazeSelect()
    {
        if (!_interactable) return;
        Debug.Log($"[GazeButton] Dipilih: {gameObject.name}");
        OnSelected?.Invoke();
    }

    // -------------------------------------------------------
    // PUBLIC METHODS
    // -------------------------------------------------------

    public void SetGazeDuration(float duration)
    {
        _gazeDuration = duration;
    }

    public void SetInteractable(bool value)
    {
        _interactable = value;
        if (!value)
        {
            OnGazeExit(); // Reset visual
            if (buttonBackground != null)
                buttonBackground.color = Color.gray;
        }
    }

    // -------------------------------------------------------
    // PRIVATE
    // -------------------------------------------------------

    private void Update()
    {
        // GazeController update progress dari luar via OnGazeEnter/Exit
        // Progress fill diupdate di sini tiap frame kalau sedang di-gaze
    }

    private void UpdateProgressFill(float progress)
    {
        if (progressFill == null) return;
        progressFill.fillAmount = progress;
    }

    private void Start()
    {
        if (buttonBackground != null)
            buttonBackground.color = normalColor;
        UpdateProgressFill(0f);
    }
}
