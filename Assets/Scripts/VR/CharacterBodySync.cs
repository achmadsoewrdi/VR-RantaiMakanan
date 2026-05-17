using UnityEngine;

public class CharacterBodySync : MonoBehaviour
{
    [Header("Referensi")]
    public Transform vrCamera;
    public bool syncRotation = true;

    [Header("Offset Posisi")]
    public Vector3 localOffset = new Vector3(0f, -0.52f, 0f);

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        if (vrCamera == null)
            vrCamera = Camera.main?.transform;
    }

    void LateUpdate()
    {
        // Kunci posisi lokal — tidak peduli script lain menggerakkan apa
        transform.localPosition = localOffset;

        // Sinkron rotasi Y mengikuti kamera
        if (syncRotation && vrCamera != null)
        {
            Vector3 forward = vrCamera.forward;
            forward.y = 0f;
            if (forward.sqrMagnitude > 0.01f)
                transform.rotation = Quaternion.LookRotation(forward);
        }
    }
}