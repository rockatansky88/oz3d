using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("Score UI")]
    [SerializeField] private TextMeshProUGUI scoreText; // 또는 Text 사용

    [Header("Clear Panel")]
    [SerializeField] private GameObject clearPanel;     // 클리어 패널
    [SerializeField] private Button restartButton;      // 재시작 버튼
    [SerializeField] private Button quitButton;         // 종료 버튼

    void Start()
    {
        // GameManager 이벤트 구독
        GameManager.instance.OnScoreChanged += UpdateScoreUI;
        GameManager.instance.OnGameClear += ShowClearPanel;

        // 클리어 패널 초기 비활성화
        clearPanel.SetActive(false);

        // 버튼 이벤트 연결
        restartButton.onClick.AddListener(GameManager.instance.RestartGame);
        quitButton.onClick.AddListener(GameManager.instance.QuitGame);
    }

    // 점수 UI 업데이트
    private void UpdateScoreUI(int current, int target)
    {
        scoreText.text = $" ITEM: {current} / {target}";
    }

    // 클리어 패널 
    private void ShowClearPanel()
    {
        clearPanel.SetActive(true);
    }

    void OnDestroy()
    {
        // 이벤트 구독 해제
        if (GameManager.instance != null)
        {
            GameManager.instance.OnScoreChanged -= UpdateScoreUI;
            GameManager.instance.OnGameClear -= ShowClearPanel;
        }
    }
}