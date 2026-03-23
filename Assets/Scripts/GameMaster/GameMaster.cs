using TMPro;
using UnityEngine;

public class GameMaster : MonoBehaviour
{
    [Header("関連するスクリプトの参照")]
    [Tooltip("QuestionSetterの参照")]
    [SerializeField] private QuestionSetter questionSetter;
    [Tooltip("PanelCreatorの参照")]
    [SerializeField] private PanelCreator panelCreator;


    [Header("ゲームオブジェクトの参照")]
    [Tooltip("プレイ中UIのパネル")]
    [SerializeField] private GameObject playUIPanel;
    [Tooltip("リザルトUIのパネル")]
    [SerializeField] private GameObject resultUIPanel;

    [Header("テキストの参照")]
    [Tooltip("回答時間のテキスト")]
    [SerializeField] private TMP_Text timeLimitText;
    [Tooltip("現在のスコアのテキスト")]
    [SerializeField] private TMP_Text currentScoreText;
    [Tooltip("残りライフのテキスト")]
    [SerializeField] private TMP_Text livesText;
    [Tooltip("リザルトのスコアテキスト")]
    [SerializeField] private TMP_Text resultScoreText;
    [Tooltip("リザルト画面のタイトル")]
    [SerializeField] private TMP_Text resultTitleText;

    [Header("ゲームの状態を管理する変数")]
    public int difficulty = -1; // -1: 未選択, 0: 初級, 1: 中級, 2: 上級
    public bool isGameStarted = false; // ゲームが開始されたかどうか
    public int livesRemaining; // プレイヤーの残りライフ
    public int score; // プレイヤーのスコア

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        questionSetter.enabled = false; // ゲーム開始前はQuestionSetterを無効にする
        playUIPanel.SetActive(false); // プレイ中UIを非表示にする
        resultUIPanel.SetActive(false); // リザルトUIを非表示にする
    }

    // Update is called once per frame
    void Update()
    {
        if (isGameStarted)
        {
            questionSetter.timeRemaining -= Time.deltaTime; // 残り時間を減少させる
            if (questionSetter.timeRemaining <= 0f)
            {
                if (livesRemaining > 1)
                {
                    livesRemaining--; // ライフを減らす
                    Debug.Log($"時間切れです。残りライフ: {livesRemaining}");
                    questionSetter.FinishQuestion(); // 問題を終了するメソッドを呼び出す
                }
                else
                {
                    Debug.Log("ゲームオーバーです。");
                    FinishGame(0); // ゲームを終了するメソッドを呼び出す
                    return;
                }
            }
            else
            {
                questionSetter.CheckAnswer(); // プレイヤーの入力をチェックするメソッドを呼び出す
            }
        }

        // UIの更新
        timeLimitText.text = $"残り時間: {Mathf.CeilToInt(questionSetter.timeRemaining)}秒";
        currentScoreText.text = $"スコア: {score}";
        livesText.text = $"残機: {livesRemaining}";
    }

    public void StartGame()
    {
        if (difficulty == -1)
        {
            Debug.LogWarning("難易度が選択されていません。ゲームを開始できません。");
            return;
        }
        questionSetter.enabled = true; // QuestionSetterを有効にする
        playUIPanel.SetActive(true); // プレイ中UIを表示する
        Debug.Log("ゲームが開始されました。");
    }

    public void FinishGame(int isCleared)
    {
        isGameStarted = false; // ゲームを終了する
        panelCreator.DestroyPanel(); // 現在の問題のパネルを破壊する
        Debug.Log("ゲームが終了しました。");
        resultScoreText.text = $"最終スコア: {score}"; // リザルトのスコアテキストを更新する
        if (isCleared == 1)
        {
            resultTitleText.text = "ゲームクリア！";
        }
        else
        {
            resultTitleText.text = "ゲームオーバー";
        }
        playUIPanel.SetActive(false); // プレイ中UIを非表示にする
        resultUIPanel.SetActive(true); // リザルトUIを表示する
    }
}
