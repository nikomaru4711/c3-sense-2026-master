using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ComboUI : MonoBehaviour
{
    // 透明度と位置の調整に必要なクラス変数を宣言
    [SerializeField] private Graphic uiElement; // UI要素のGraphicコンポーネント
    [SerializeField] private RectTransform uiTransform; // UI要素のRectTransformコンポーネント
    [SerializeField] private TMP_Text text;

    private float riseDistance = 50f; // 上昇する距離
    private float fadeTime = 0.5f; // フェードアウトにかかる時間
    private float displayTime = 2f; // UI要素が表示される時間
    private float goalY = 200f; // UI要素のY座標
    private float elapsedTime = 0f; // 経過時間

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Setup(int comboCount)
    {
        // 初期位置を設定
        if (uiTransform != null)
        {
            Vector2 startPosition = uiTransform.anchoredPosition;
            startPosition.x = -400f; // X座標を固定
            startPosition.y = goalY - riseDistance; // 上昇する距離分下に配置
            uiTransform.anchoredPosition = startPosition;
        }
        // 初期の透明度を設定
        SetAlpha(0f); // 最初は完全に透明にする
        text.text = comboCount.ToString() + "コンボ！";
    }

    // Update is called once per frame
    void Update()
    {
        // フェードイン中・表示中・フェードアウト中に場合分け
        elapsedTime += Time.deltaTime;
        if (elapsedTime < fadeTime)
        {
            // fadeTime秒かけて上昇しながらフェードイン
            float t = elapsedTime / fadeTime;
            float parameter = GetParameter(t);
            Vector2 curPos = uiTransform.anchoredPosition;
            curPos.y = goalY - (1f - parameter) * riseDistance;
            uiTransform.anchoredPosition = curPos;
            SetAlpha(t); // 透明度を更新
        }
        else if (elapsedTime < fadeTime + displayTime)
        {
            // 表示時間中は位置を固定して透明度を維持
            Vector2 curPos = uiTransform.anchoredPosition;
            curPos.y = goalY;
            uiTransform.anchoredPosition = curPos;
            SetAlpha(1f); // 透明度を維持
        }
        else if (elapsedTime < fadeTime + displayTime + fadeTime)
        {
            // fadeTime秒かけて上昇しながらフェードアウト
            float t = (elapsedTime - fadeTime - displayTime) / fadeTime;
            float parameter = GetParameter(t, 1);
            Vector2 curPos = uiTransform.anchoredPosition;
            curPos.y = goalY + parameter * riseDistance;
            uiTransform.anchoredPosition = curPos;
            SetAlpha(1f - t); // 透明度を更新
        }
        else
        {
            // 完全にフェードアウトしたらオブジェクトを破壊する
            Destroy(gameObject);
        }
    }

    void SetAlpha(float alpha)
    {
        if (uiElement != null)
        {
            Color color = uiElement.color;
            color.a = alpha;
            uiElement.color = color;
        }
    }

    float GetParameter(float t, int type = 0)
    {
        float t1 = (type == 0)? 1 - t: t;
        float t2 = Mathf.Pow(t1, 2);
        if (type == 0)
            t2 = 0.5f - 0.5f * t2;
        return t2;
    }
}
