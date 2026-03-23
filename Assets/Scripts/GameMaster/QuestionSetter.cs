using UnityEngine;

public class QuestionSetter : MonoBehaviour
{
    [Header("関連するスクリプトの参照")]
    [Tooltip("GameMasterの参照")]
    [SerializeField] private GameMaster gameMaster;
    [Tooltip("PanelCreatorの参照")]
    [SerializeField] private PanelCreator panelCreator;

    private int difficulty;
    public int currentQuestionIndex = 0; // 現在の問題のインデックス
    public int numberOfQuestions; // 出題する問題の数
    public float timeLimitPerQuestion; // 1問あたりの制限時間（秒）
    public int penaltyPoints = 30; // 不正解のキーが押されたときの減点ポイント
    public float timeRemaining; // 現在の問題の残り時間（秒）
    public float currentQuestionScore; // 現在の問題のスコア
    public KeyCode[] CorrectAnswers; // 現在の問題の正解のキーコード
    public int lives; // プレイヤーのライフ
    private int[] LivesByDifficulty = { 5, 3, 1 }; // 難易度ごとのライフ数
    private int[] NumOfQs = { 10, 15, 20 }; // 難易度ごとの出題数
    private float[] TimeLimits = { 10f, 8f, 6f }; // 難易度ごとの制限時間

    // 数字とアルファベットのキーコードのリスト
    private KeyCode[] AnswerKeys = new KeyCode[]
    {
        KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.Alpha4, KeyCode.Alpha5, KeyCode.Alpha6, KeyCode.Alpha7, KeyCode.Alpha8, KeyCode.Alpha9, KeyCode.Alpha0,
        KeyCode.Q, KeyCode.W, KeyCode.E, KeyCode.R, KeyCode.T, KeyCode.Y, KeyCode.U, KeyCode.I, KeyCode.O, KeyCode.P,
        KeyCode.A, KeyCode.S, KeyCode.D, KeyCode.F, KeyCode.G, KeyCode.H, KeyCode.J, KeyCode.K, KeyCode.L,
        KeyCode.Z, KeyCode.X, KeyCode.C, KeyCode.V, KeyCode.B, KeyCode.N, KeyCode.M
    };

    private bool[] PlayerInputs = new bool[36]; // プレイヤーの入力状態を管理する配列

    private int minQuestionNumber; // 問題番号の最小値
    private int maxQuestionNumber; // 問題番号の最大値


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        difficulty = gameMaster.difficulty; // GameMasterから難易度を取得
        currentQuestionIndex = 0;
        if (difficulty >= 0 && difficulty < NumOfQs.Length)
        {
            numberOfQuestions = NumOfQs[difficulty]; // 難易度に応じた出題数を設定
            timeLimitPerQuestion = TimeLimits[difficulty]; // 難易度に応じた制限時間を設定
            timeRemaining = timeLimitPerQuestion; // 最初の問題の残り時間を設定
            lives = LivesByDifficulty[difficulty]; // 難易度に応じたライフ数を設定
            gameMaster.livesRemaining = lives; // GameMasterの残りライフを設定
            if (difficulty == 0)
            {
                int questionSetNumber = Random.Range(0, 4); // 0から3のランダムな整数を生成
                if (questionSetNumber == 0)
                {
                    minQuestionNumber = 0;
                    maxQuestionNumber = 9;
                }
                else if (questionSetNumber == 1)
                {
                    minQuestionNumber = 10;
                    maxQuestionNumber = 19;
                }
                else if (questionSetNumber == 2)
                {
                    minQuestionNumber = 20;
                    maxQuestionNumber = 28;
                }
                else
                {
                    minQuestionNumber = 29;
                    maxQuestionNumber = 35;
                }
            }
            else
            {
                minQuestionNumber = 0;
                maxQuestionNumber = 35;
            }
            SetNextQuestion(); // 最初の問題を設定するメソッドを呼び出す
            gameMaster.isGameStarted = true; // ゲーム開始フラグを立てる
            Debug.Log($"ゲームが開始されました。難易度: {difficulty}, 出題数: {numberOfQuestions}, 制限時間: {timeLimitPerQuestion}秒, ライフ: {lives}");
        }
        else
        {
            Debug.LogError("無効な難易度が設定されています。");
            numberOfQuestions = 0;
            timeLimitPerQuestion = 0f;
            timeRemaining = 0f;
        }
    }

    public void FinishQuestion()
    {
        panelCreator.DestroyPanel(); // 現在の問題のパネルを破壊する

        if (currentQuestionIndex < numberOfQuestions)
        {
            // 次の問題を出題する処理をここに追加
            currentQuestionIndex++;
            timeRemaining = timeLimitPerQuestion; // 次の問題の残り時間をリセット
            SetNextQuestion(); // 次の問題を設定するメソッドを呼び出す
            Debug.Log($"次の問題が出題されました。現在の問題インデックス: {currentQuestionIndex}");
            Debug.Log($"現在のスコア: {gameMaster.score}");
            Debug.Log($"残りライフ: {gameMaster.livesRemaining}");
        }
        else
        {
            Debug.Log("すべての問題が出題されました。");
            gameMaster.FinishGame(1); // ゲームを終了するメソッドを呼び出す
        }
    }

    public void UpdateScore(float timeRemaining, float questionScore)
    {
        // スコアの更新処理
        int score = (int)(questionScore); // 問題のスコアを整数に変換
        gameMaster.score += score; // GameMasterのスコアを更新
        Debug.Log($"スコアが更新されました。現在のスコア: {gameMaster.score}");
    }

    public void CheckAnswer()
    {
        // プレイヤーの入力が変化したかどうかをチェック
        // 新しく押されたキーが不正解の場合、スコアを減点する
        // 完全一致の場合正解とする

        int correctInputCount = 0; // プレイヤーが正解のキーを押した個数

        for (int i = 0; i < AnswerKeys.Length; i++)
        {
            if (Input.GetKey(AnswerKeys[i]))
            {
                if (System.Array.Exists(CorrectAnswers, key => key == AnswerKeys[i]))
                {
                    correctInputCount++; // プレイヤーが正解のキーを押した個数を増やす
                }

                if (!PlayerInputs[i])
                {
                    if (!System.Array.Exists(CorrectAnswers, key => key == AnswerKeys[i]))
                    {
                        gameMaster.score = Mathf.Max(0, gameMaster.score - penaltyPoints); // 不正解のキーが押された場合はスコアを減点（最低0点まで）
                        Debug.Log($"不正解のキーが押されました: {AnswerKeys[i]}. スコアが減点されました。現在のスコア: {gameMaster.score}");
                    }

                    PlayerInputs[i] = true; // プレイヤーの入力状態を更新
                }
            }
            else if (PlayerInputs[i])
            {
                PlayerInputs[i] = false; // プレイヤーの入力状態を更新
            }
        }

        if (correctInputCount == CorrectAnswers.Length)
        {
            // プレイヤーがすべての正解のキーを押した場合は正解とする
            int numberOfCorrectAnswers = CorrectAnswers.Length;

            UpdateScore(timeRemaining, 100f); // 正解の場合はスコアを加算
            Debug.Log("正解です！");
            FinishQuestion(); // 次の問題を設定する
        }
    }

    public void SetNextQuestion()
    {
        // 指定の問題番号内でランダムに正解を設定
        // 正解が重複しないようにする
        int numberOfAnswers = Random.Range(1, difficulty + 2); // 正解の数をランダムに決定（難易度に応じて1～difficulty+1）
        CorrectAnswers = new KeyCode[numberOfAnswers];
        for (int i = 0; i < CorrectAnswers.Length; i++)
        {
            while (true)
            {
                int randomIndex = Random.Range(minQuestionNumber, maxQuestionNumber + 1);
                KeyCode randomKey = AnswerKeys[randomIndex];
                if (!System.Array.Exists(CorrectAnswers, key => key == randomKey))
                {
                    CorrectAnswers[i] = randomKey;
                    break;
                }
            }
        }

        // PanelCreatorに正解の文字を渡してパネルを生成する
        string[] answerCharactors = new string[CorrectAnswers.Length];
        for (int i = 0; i < CorrectAnswers.Length; i++)
        {
            answerCharactors[i] = CorrectAnswers[i].ToString().Replace("Alpha", ""); // KeyCodeから"Alpha"を取り除いて文字を取得
        }
        panelCreator.CreatePanel(CorrectAnswers.Length, answerCharactors);

        // 次の問題の内容をコンソールに表示する
        Debug.Log($"次の問題が設定されました。正解のキーコード: {string.Join(", ", CorrectAnswers)}");
    }
}
