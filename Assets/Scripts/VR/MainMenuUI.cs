using UnityEngine;
using UnityEngine.InputSystem.XR;
using System.Collections;

public class MainMenuUI : MonoBehaviour
{
    [Header("Referensi")]
    public UIPanelAnimation panelAnimasi;
    public GameObject characterDuplikat;

    [Header("Freeze Movement")]
    public VRMovementAnimator vrMovementAnimator;
    public CharacterController characterController;
    public TrackedPoseDriver trackedPoseDriver;

    [Header("Scene Transition")]
    public Transform xrOrigin; // Drag XR Origin (VR)

    void Start()
    {
        if (vrMovementAnimator != null)
            vrMovementAnimator.enabled = false;

        if (characterController != null)
            characterController.enabled = false;

        if (trackedPoseDriver != null)
            trackedPoseDriver.trackingType = TrackedPoseDriver.TrackingType.RotationOnly;

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
        // Simpan posisi terakhir XR Origin ke PlayerPrefs
        if (xrOrigin != null)
        {
            PlayerPrefs.SetFloat("SpawnX", xrOrigin.position.x);
            PlayerPrefs.SetFloat("SpawnY", xrOrigin.position.y);
            PlayerPrefs.SetFloat("SpawnZ", xrOrigin.position.z);
            PlayerPrefs.Save();
        }

        // Aktifkan kembali movement
        if (vrMovementAnimator != null)
            vrMovementAnimator.enabled = true;

        if (characterController != null)
            characterController.enabled = true;

        if (trackedPoseDriver != null)
            trackedPoseDriver.trackingType = TrackedPoseDriver.TrackingType.RotationAndPosition;

        // Load scene berikutnya via SceneController yang sudah ada
        if (SceneController.Instance != null)
            SceneController.Instance.LoadNextScene();
        else
            Debug.LogWarning("[MainMenuUI] SceneController.Instance tidak ditemukan!");

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