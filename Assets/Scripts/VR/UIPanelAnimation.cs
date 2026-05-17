using UnityEngine;
using System.Collections;

public class UIPanelAnimation : MonoBehaviour
{
    [Header("Pengaturan Animasi")]
    public float durasi = 0.8f;
    public float startOffsetY = 600f;
    public AnimationCurve kurvaMulus;

    private Vector3 posisiAsli;
    private RectTransform rectTransform;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        posisiAsli = rectTransform.localPosition;
    }

    void OnEnable()
    {
        StopAllCoroutines();
        StartCoroutine(AnimasiTurun());
    }

    // Dipanggil oleh MainMenuUI saat tombol Start di-gaze
    public void TutupPanel(System.Action onSelesai = null)
    {
        StopAllCoroutines();
        StartCoroutine(AnimasiNaik(onSelesai));
    }

    IEnumerator AnimasiTurun()
    {
        float timer = 0;
        Vector3 posisiAwal = posisiAsli + new Vector3(0, startOffsetY, 0);
        rectTransform.localPosition = posisiAwal;

        while (timer < durasi)
        {
            timer += Time.deltaTime;
            float curveValue = kurvaMulus.Evaluate(timer / durasi);
            rectTransform.localPosition = Vector3.LerpUnclamped(posisiAwal, posisiAsli, curveValue);
            yield return null;
        }
        rectTransform.localPosition = posisiAsli;
    }

    IEnumerator AnimasiNaik(System.Action onSelesai)
    {
        float timer = 0;
        Vector3 posisiSekarang = rectTransform.localPosition;
        Vector3 posisiTujuan = posisiAsli + new Vector3(0, startOffsetY, 0);

        while (timer < durasi)
        {
            timer += Time.deltaTime;
            float curveValue = kurvaMulus.Evaluate(timer / durasi);
            rectTransform.localPosition = Vector3.LerpUnclamped(posisiSekarang, posisiTujuan, curveValue);
            yield return null;
        }

        gameObject.SetActive(false);
        onSelesai?.Invoke(); // callback ke MainMenuUI
    }
}