using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    [Header("Referensi")]
    public Transform xrOrigin;

    [Header("Fallback Spawn Point")]
    public Transform defaultSpawnPoint;

    void Start()
    {
        if (!PlayerPrefs.HasKey("SpawnX"))
        {
            // Tidak ada data posisi — pakai spawn point default
            if (defaultSpawnPoint != null && xrOrigin != null)
                xrOrigin.position = defaultSpawnPoint.position;
            return;
        }

        // Ada data posisi dari Scene_00
        Vector3 spawnPos = new Vector3(
            PlayerPrefs.GetFloat("SpawnX"),
            PlayerPrefs.GetFloat("SpawnY"),
            PlayerPrefs.GetFloat("SpawnZ")
        );

        if (xrOrigin != null)
            xrOrigin.position = spawnPos;

        // Hapus setelah dipakai
        PlayerPrefs.DeleteKey("SpawnX");
        PlayerPrefs.DeleteKey("SpawnY");
        PlayerPrefs.DeleteKey("SpawnZ");
    }
}