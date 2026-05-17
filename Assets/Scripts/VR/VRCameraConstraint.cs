using UnityEngine;

public class VRCameraConstraint : MonoBehaviour
{
    [Header("Batas Rotasi Kepala (Derajat)")]
    public float minXRotation = -30f; // batas dongak ke bawah
    public float maxXRotation = 30f;  // batas dongak ke atas

    void LateUpdate()
    {
        Vector3 euler = transform.localEulerAngles;

        // Konversi dari 0-360 ke -180~180
        float xAngle = euler.x;
        if (xAngle > 180f) xAngle -= 360f;

        // Clamp rotasi X
        xAngle = Mathf.Clamp(xAngle, minXRotation, maxXRotation);

        transform.localEulerAngles = new Vector3(xAngle, euler.y, euler.z);
    }
}