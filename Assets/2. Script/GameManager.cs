using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Game Settings")]
    [SerializeField] private int targetItemCount = 10;  // 클리어에 필요한 아이템 개수

    private int currentScore = 0;                       // 현재 수집한 아이템 개수
    private bool isGameClear = false;                   // 게임 클리어 여부

    public System.Action<int, int> OnScoreChanged;      // (현재점수, 목표점수)
    public System.Action OnGameClear;                   // 게임 클리어 시

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        // 초기 UI 업데이트, 현재 점수를 0으로 설정
        OnScoreChanged?.Invoke(currentScore, targetItemCount);
    }


    // 아이템 수집
    public void CollectItem()
    {
        if (isGameClear) return;

        currentScore++;
        OnScoreChanged?.Invoke(currentScore, targetItemCount);
        // 아이템 수집을 하게되면 OnScoreChanged 에 구독된 메서드들이 호출되어 UI가 업데이트 됨 

        Debug.Log($"아이템 수집! ({currentScore}/{targetItemCount})");

        // 목표 달성 체크
        if (currentScore >= targetItemCount)
        {
            GameClear();
        }
    }

    // 게임 클리어 처리
    private void GameClear()
    {
        isGameClear = true;
        Time.timeScale = 0f; // 게임 일시정지
        OnGameClear?.Invoke(); // 게임 클리어 시 구독된 메서드들 호출 
        Debug.Log("게임 클리어!");
    }

    /// 게임 재시작
    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // 게임 종료
    public void QuitGame()
    {
        Debug.Log("게임 종료");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;  // 에디터 모드 종료
#else
        Application.Quit();
#endif
    }

    // Getter
    public int GetTargetItemCount() => targetItemCount;  // 목표 아이템 개수 반환
    public int GetCurrentScore() => currentScore; // 현재 점수 반환
}