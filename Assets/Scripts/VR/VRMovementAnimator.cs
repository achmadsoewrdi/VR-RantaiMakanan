using UnityEngine;

public class VRMovementAnimator : MonoBehaviour
{
    [Header("Referensi Komponen")]
    public Transform vrHeadset; 
    public Animator animator;   

    [Header("Pengaturan")]
    public float animationSmoothing = 5f; 

    private Vector3 previousPosition;
    private float smoothedMoveX = 0f;
    private float smoothedMoveZ = 0f;

    void Start()
    {
        if (vrHeadset != null) previousPosition = vrHeadset.position;
    }

    void Update()
    {
        if (vrHeadset == null || animator == null) return;

        // --- 1. SINKRONISASI POSISI & ROTASI TUBUH ---
        // Menarik badan karakter agar selalu berada di bawah kepala pemain
        Vector3 headForward = vrHeadset.forward;
        headForward.y = 0; // Abaikan dongakan kepala ke atas/bawah
        headForward.Normalize();

        // Menarik badan karakter agar berada di bawah kepala, TAPI mundur 15cm ke belakang
        // Ubah angka 0.15f menjadi 0.2f jika masih terlihat tembus
        Vector3 bodyPosition = vrHeadset.position - (headForward * 0.15f); 
        bodyPosition.y = transform.position.y; // Ketinggian badan tetap di tanah
        transform.position = bodyPosition;

        // Memutar badan karakter agar menghadap searah dengan pandangan VR
        if (headForward != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(headForward);
        }

        // --- 2. SISTEM ANIMASI JALAN ---
        Vector3 movementVector = (vrHeadset.position - previousPosition) / Time.deltaTime;
        movementVector.y = 0; 

        Vector3 localVelocity = transform.InverseTransformDirection(movementVector);

        smoothedMoveX = Mathf.Lerp(smoothedMoveX, localVelocity.x, Time.deltaTime * animationSmoothing);
        smoothedMoveZ = Mathf.Lerp(smoothedMoveZ, localVelocity.z, Time.deltaTime * animationSmoothing);

        animator.SetFloat("MoveX", smoothedMoveX);
        animator.SetFloat("MoveZ", smoothedMoveZ);
        animator.SetFloat("Speed", movementVector.magnitude);

        previousPosition = vrHeadset.position;
    }
}