using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Home : MonoBehaviour
{
    [Tooltip("ホーム画面のパネル")]
    [SerializeField] private GameObject homePanel;

    [Tooltip("スタートボタン")]
    [SerializeField] private GameObject startButton;

    [Tooltip("初級ボタン")]
    [SerializeField] private GameObject biginner;
    [Tooltip("中級ボタン")]
    [SerializeField] private GameObject intermediate;
    [Tooltip("上級ボタン")]
    [SerializeField] private GameObject advanced;

    [Tooltip("カウントダウンのテキストボックス")]
    [SerializeField] private TMP_Text countdownText;

    [Tooltip("ゲームマスター")]
    [SerializeField] private GameMaster gameMaster;

    public float countdownTime = 3.0f; // カウントダウンの時間（秒）
    public float timer = -10f; // カウントダウンのタイマー
    private bool isCountingDown = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // スタートボタンを有効にする
        startButton.SetActive(true);

        // 難易度選択ボタンを非表示にする
        biginner.SetActive(false);
        intermediate.SetActive(false);
        advanced.SetActive(false);

        // カウントダウンテキストを非表示にする
        countdownText.gameObject.SetActive(false);
    }

    // スタートボタンがクリックされたときの処理
    public void OnStartButtonClicked()
    {
        // スタートボタンを非表示にする
        startButton.SetActive(false);

        // 難易度選択ボタンを表示する
        biginner.SetActive(true);
        intermediate.SetActive(true);
        advanced.SetActive(true);
    }

    // 難易度ボタンがクリックされたときの処理
    public void OnDifficultyButtonClicked(int difficulty)
    {
        // ゲームマスターに難易度を設定する
        gameMaster.difficulty = difficulty;

        Debug.Log($"難易度 {difficulty} が選択されました。");

        // ホーム画面を非表示にする
        homePanel.SetActive(false);

        // カウントダウンを開始する
        countdownText.gameObject.SetActive(true);
        timer = countdownTime;
        isCountingDown = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (isCountingDown)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                // カウントダウンが終了したときの処理
                countdownText.text = "スタート！";

                if (timer < -0.5f)
                {
                    isCountingDown = false;
                    countdownText.gameObject.SetActive(false);

                    gameMaster.StartGame(); // ゲームを開始する
                }
            }
            else
            {
                // カウントダウンのテキストを更新する
                countdownText.text = Mathf.Ceil(timer).ToString();
            }
        }
    }
}
