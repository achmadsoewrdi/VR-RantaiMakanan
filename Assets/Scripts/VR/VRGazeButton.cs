using UnityEngine;
using UnityEngine.Events;

public class VRGazeButton : MonoBehaviour, IGazeInteractable
{
    [Header("Gaze Settings")]
    [SerializeField] private float gazeDuration = 2f;
    public float GazeDuration => gazeDuration;  // Property untuk interface

    [Header("Visual Feedback")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color hoverColor = Color.green;
    [SerializeField] private Color selectedColor = Color.blue;

    [Header("Events")]
    public UnityEvent onGazeEnter;
    public UnityEvent onGazeExit;
    public UnityEvent onGazeSelect;  // Dipanggil saat gaze selesai

    private UnityEngine.UI.Image buttonImage;

    void Awake()
    {
        buttonImage = GetComponent<UnityEngine.UI.Image>();
        if (buttonImage != null)
            buttonImage.color = normalColor;
    }

    public void OnGazeEnter()
    {
        if (buttonImage != null)
            buttonImage.color = hoverColor;
        
        onGazeEnter?.Invoke();
        Debug.Log("👁️ Gaze ENTER: " + gameObject.name);
    }

    public void OnGazeExit()
    {
        if (buttonImage != null)
            buttonImage.color = normalColor;
        
        onGazeExit?.Invoke();
        Debug.Log("👁️ Gaze EXIT: " + gameObject.name);
    }

    public void OnGazeSelect()
    {
        if (buttonImage != null)
            buttonImage.color = selectedColor;
        
        onGazeSelect?.Invoke();
        Debug.Log("✅ Gaze SELECT: " + gameObject.name);
    }
}