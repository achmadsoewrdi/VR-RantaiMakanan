using UnityEngine;
using UnityEngine.Events;

public class MainMenuUI : MonoBehaviour
{
    // Ini mirip seperti passing "props" fungsi di Svelte/React
    public UnityEvent OnStartGame;
    public UnityEvent OnExitGame;

    // Fungsi ini akan dipanggil oleh tombol "Mulai Petualangan"
    public void TriggerStartGame()
    {
        Debug.Log("Tombol Mulai Ditekan!");
        OnStartGame?.Invoke(); // Memberitahu SceneController untuk jalan
    }

    // Fungsi ini akan dipanggil oleh tombol "Keluar"
    public void TriggerExitGame()
    {
        Debug.Log("Tombol Keluar Ditekan!");
        OnExitGame?.Invoke();
    }
}