/// <summary>
/// Interface yang harus diimplementasi oleh semua objek
/// yang bisa diinteraksi via gaze (tatapan).
///
/// Contoh penggunaan: tombol quiz, tombol next scene, dll.
/// </summary>
public interface IGazeInteractable
{
    /// <summary>Berapa detik user harus menatap objek ini untuk memilihnya.</summary>
    float GazeDuration { get; }

    /// <summary>Dipanggil saat gaze mulai mengarah ke objek ini.</summary>
    void OnGazeEnter();

    /// <summary>Dipanggil saat gaze keluar dari objek ini sebelum selesai.</summary>
    void OnGazeExit();

    /// <summary>Dipanggil saat gaze sudah cukup lama — ini trigger utama aksi.</summary>
    void OnGazeSelect();
}
