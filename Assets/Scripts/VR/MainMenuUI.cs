using UnityEngine;
using UnityEngine.InputSystem.XR;

public class MainMenuUI : MonoBehaviour
{
    [Header("Referensi")]
    public UIPanelAnimation panelAnimasi;
    public GameObject characterDuplikat;

    [Header("Freeze Movement")]
    public VRMovementAnimator vrMovementAnimator;
    public CharacterController characterController;
    public TrackedPoseDriver trackedPoseDriver;

    void Start()
    {
        if (vrMovementAnimator != null)
            vrMovementAnimator.enabled = false;

        if (characterController != null)
            characterController.enabled = false;

        // Hanya freeze posisi, rotasi tetap jalan
        if (trackedPoseDriver != null)
            trackedPoseDriver.trackingType = TrackedPoseDriver.TrackingType.RotationOnly;
        else
            Debug.LogWarning("[MainMenuUI] TrackedPoseDriver belum di-assign!");

        if (characterDuplikat != null)
            characterDuplikat.SetActive(false);
    }

    public void OnTombolMulai()
    {
        if (panelAnimasi != null)
            panelAnimasi.TutupPanel(OnPanelSelesai);
        else
            OnPanelSelesai();
    }

    private void OnPanelSelesai()
    {
        if (vrMovementAnimator != null)
            vrMovementAnimator.enabled = true;

        if (characterController != null)
            characterController.enabled = true;

        // Kembalikan ke Rotation And Position setelah panel selesai
        if (trackedPoseDriver != null)
            trackedPoseDriver.trackingType = TrackedPoseDriver.TrackingType.RotationAndPosition;

        this.enabled = false;
    }

    public void OnTombolKeluar()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}