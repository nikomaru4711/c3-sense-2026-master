using UnityEngine;

public class PanelCreator : MonoBehaviour
{
    [Header("パラメータの設定")]
    [Tooltip("パネルの間隔")]
    [SerializeField] private float panelSpacing;

    [Header("プレハブの参照")]
    [Tooltip("生成するパネルのプレハブ")]
    [SerializeField] private GameObject panelPrefab;
    [Tooltip("プラス記号のプレハブ")]
    [SerializeField] private GameObject plusPrefab;
    [Header("関連するスクリプトの参照")]
    [SerializeField] private QuestionSetter questionSetter;
    [SerializeField] private GameMaster gameMaster;

    private GameObject[] AnswerPanels; // 生成されたパネルの配列
    private GameObject[] PlusObjects; // 生成されたプラス記号の配列


    // 問題設定時にQuestionSetterから呼び出されるメソッド
    public void CreatePanel(int numbeOfAnswer, string[] AnswerCharactors)
    {
        AnswerPanels = new GameObject[numbeOfAnswer];
        for (int i = 0; i < numbeOfAnswer; i++)
        {
            AnswerPanels[i] = Instantiate(panelPrefab, transform);
            PanelScript panelScript = AnswerPanels[i].GetComponent<PanelScript>();
            if (panelScript != null)
            {
                int shutterType = Random.Range(0, gameMaster.difficulty + 1); // シャッターの種類をランダムに選択
                int sign;
                if (gameMaster.difficulty == 0)
                {
                    sign = 1;
                }
                else
                {
                    sign = Random.Range(0, 2) * 2 - 1; // ランダムに-1か1を選択
                }
                panelScript.SetAnswerCharactor(AnswerCharactors[i], questionSetter.timeLimitPerQuestion, shutterType, sign);
            }
            else
            {
                Debug.LogError("PanelScript component not found on the instantiated panel.");
            }
        }

        // 位置の調整
        if (numbeOfAnswer == 1)
        {
            AnswerPanels[0].GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            PlusObjects = null;
        }
        else if (numbeOfAnswer == 2)
        {
            AnswerPanels[0].GetComponent<RectTransform>().anchoredPosition = new Vector2(-panelSpacing / 2, 0);
            AnswerPanels[1].GetComponent<RectTransform>().anchoredPosition = new Vector2(panelSpacing / 2, 0);
            PlusObjects = new GameObject[1];
            PlusObjects[0] = Instantiate(plusPrefab, transform);
            PlusObjects[0].GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        }
        else
        {
            AnswerPanels[0].GetComponent<RectTransform>().anchoredPosition = new Vector2(-panelSpacing, 0);
            AnswerPanels[1].GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            AnswerPanels[2].GetComponent<RectTransform>().anchoredPosition = new Vector2(panelSpacing, 0);
            PlusObjects = new GameObject[2];
            PlusObjects[0] = Instantiate(plusPrefab, transform);
            PlusObjects[0].GetComponent<RectTransform>().anchoredPosition = new Vector2(-panelSpacing / 2, 0);
            PlusObjects[1] = Instantiate(plusPrefab, transform);
            PlusObjects[1].GetComponent<RectTransform>().anchoredPosition = new Vector2(panelSpacing / 2, 0);
        }
    }


    public void DestroyPanel()
    {
        if (AnswerPanels != null)
        {
            foreach (GameObject panel in AnswerPanels)
            {
                Destroy(panel);
            }
        }
        if (PlusObjects != null)
        {
            foreach (GameObject plus in PlusObjects)
            {
                Destroy(plus);
            }
        }
    }
}
