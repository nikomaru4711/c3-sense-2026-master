using TMPro;
using UnityEngine;

public class PanelScript : MonoBehaviour
{
    [SerializeField] private RectTransform shutterTransform; // シャッターパネルのRectTransform

    private float timeLimit; // 制限時間
    private float timeElapsed = 0; // 経過時間
    private int shutterType; // シャッターのタイプ
    private float sign; // シャッターパネルの移動方向を決定するための符号

    public void SetAnswerCharactor(string answerCharactor, float timeLimit, int shutterType, int sign)
    {
        TMP_Text answerText = GetComponentInChildren<TMP_Text>();
        if (answerText != null)
        {
            answerText.text = answerCharactor;
        }
        else
        {
            Debug.LogError("TMP_Text component not found in children.");
        }

        this.timeLimit = timeLimit;
        this.shutterType = shutterType;
        // シャッターパネルの移動方向を決定するための符号を設定
        this.sign = sign;
    }

    private void Update()
    {
        if (shutterTransform != null)
        {
            timeElapsed += Time.deltaTime; // 経過時間を更新
            float t = timeElapsed / timeLimit; // 経過時間を制限時間で割って0から1の範囲に正規化
            // シャッターパネルの位置を更新
            float shutterValue = sign * GetShutterValue(t, shutterType);
            // shutterValue = 0のときシャッターパネルが完全に閉じている状態、shutterValue = 1のとき完全に開いている状態
            shutterTransform.anchoredPosition = new Vector2(0, shutterValue * 100f); // シャッターパネルの位置を更新
        }
    }


    float GetShutterValue(float t, int type)
    {
        if (type == 0)
        {
            return Mathf.Min(1f, Mathf.Sin(t * Mathf.PI / 1.6f));
        }
        else if (type == 1)
        {
            return Mathf.Min(1f, Mathf.Pow(t * 1.25f, 5f));
        }
        else
        {
            float val = 0.4f * Mathf.Sin(2.5f * Mathf.PI * t) + 1.25f * t;
            return Mathf.Clamp01(val);
        }
    }
}
