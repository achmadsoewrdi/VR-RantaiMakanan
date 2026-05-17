using UnityEngine;

public class RotasiOtomatis : MonoBehaviour
{
    public float kecepatanPutar = 50f;

    void Update()
    {
        // Memutar objek pada sumbu Y (kiri-kanan) secara terus-menerus
        transform.Rotate(0, kecepatanPutar * Time.deltaTime, 0);
    }
}