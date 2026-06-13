using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("UI 연결")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI bestScoreText;
    public TextMeshProUGUI finalScoreText;
    public GameObject gameOverPanel;

    [Header("속도 감소 아이템 설정")]
    public float slowDuration = 5f;

    private float score = 0f;
    private int bestScore = 0;
    private bool isGameOver = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        bestScore = PlayerPrefs.GetInt("SkyFallBestScore", 0);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        UpdateUI();

        // 게임 BGM 재생
        if (SoundManager.Instance != null)
            SoundManager.Instance.PlayBGMForCurrentScene();

        if (ObstacleSpawner.Instance != null)
            ObstacleSpawner.Instance.StartSpawning();
    }

    void Update()
    {
        if (isGameOver) return;

        score += Time.deltaTime;
        UpdateUI();
    }

    void UpdateUI()
    {
        if (scoreText != null)
            scoreText.text = "SCORE : " + Mathf.FloorToInt(score).ToString();
        if (bestScoreText != null)
            bestScoreText.text = "BEST : " + bestScore.ToString();
    }

    public void SlowObstacles()
    {
        if (ObstacleSpawner.Instance != null)
            ObstacleSpawner.Instance.SlowAllObstacles(slowDuration);
    }

    public void GameOver()
    {
        if (isGameOver) return;
        isGameOver = true;

        // 게임오버 사운드
        if (SoundManager.Instance != null)
            SoundManager.Instance.PlayGameOverSound();

        if (ObstacleSpawner.Instance != null)
            ObstacleSpawner.Instance.StopSpawning();

        int currentScore = Mathf.FloorToInt(score);

        if (currentScore > bestScore)
        {
            bestScore = currentScore;
            PlayerPrefs.SetInt("SkyFallBestScore", bestScore);
            PlayerPrefs.Save();
        }

        UpdateUI();

        if (finalScoreText != null)
            finalScoreText.text = "SCORE : " + currentScore.ToString();

        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);
    }

    public void RestartGame()
    {
        // 버튼 사운드
        if (SoundManager.Instance != null)
            SoundManager.Instance.PlayButtonSound();

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}