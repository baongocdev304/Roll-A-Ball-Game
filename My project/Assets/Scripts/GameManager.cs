using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// Quản lý điểm số, số mạng, trạng thái Khiên và điều kiện Thắng / Thua của trò chơi.
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Cấu hình Trò chơi")]
    public int totalPickups = 10;
    public int currentPickups = 0;
    public int score = 0;
    public int lives = 3;
    public bool isGameOver = false;

    [Header("Giao diện UI")]
    public Text scoreText;
    public Text livesText;
    public Text shieldText;
    public GameObject resultPanel;
    public Text resultTitleText;
    public Button restartButton;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        Time.timeScale = 1f;
        isGameOver = false;

        // Tự động đếm tổng số tinh thể Pickup trong màn chơi
        Rotator[] allItems = FindObjectsByType<Rotator>(FindObjectsSortMode.None);
        int count = 0;
        foreach (Rotator item in allItems)
        {
            if (item.itemType == Rotator.ItemType.CrystalPickup && item.gameObject.activeInHierarchy)
            {
                count++;
            }
        }
        if (count > 0)
        {
            totalPickups = count;
        }

        if (resultPanel != null)
        {
            resultPanel.SetActive(false);
        }

        if (restartButton != null)
        {
            restartButton.onClick.RemoveAllListeners();
            restartButton.onClick.AddListener(RestartGame);
        }

        UpdateUI(false, 0f);
    }

    /// <summary>
    /// Cộng điểm khi người chơi thu thập 1 khối tinh thể năng lượng.
    /// </summary>
    public void CollectPickup(int points = 10)
    {
        if (isGameOver) return;

        score += points;
        currentPickups++;
        UpdateUI(false, 0f);

        // Kiểm tra điều kiện Chiến thắng
        if (currentPickups >= totalPickups)
        {
            EndGame(true);
        }
    }

    /// <summary>
    /// Trừ mạng khi người chơi va chạm chướng ngại vật hoặc rơi khỏi sàn.
    /// </summary>
    public void LoseLife()
    {
        if (isGameOver) return;

        lives--;
        UpdateUI(false, 0f);

        // Kiểm tra điều kiện Thất bại
        if (lives <= 0)
        {
            EndGame(false);
        }
    }

    /// <summary>
    /// Cập nhật hiển thị Điểm, Mạng và Khiên bảo vệ trên màn hình.
    /// </summary>
    public void UpdateUI(bool shieldActive, float shieldRemainingTime)
    {
        if (scoreText != null)
        {
            scoreText.text = $"Điểm: {score} ({currentPickups}/{totalPickups})";
        }

        if (livesText != null)
        {
            livesText.text = $"Mạng: {Mathf.Max(0, lives)}";
        }

        if (shieldText != null)
        {
            if (shieldActive)
            {
                shieldText.text = $"Khiên bảo vệ: ĐANG BẬT ({shieldRemainingTime:F1}s)";
                shieldText.color = new Color(0.2f, 0.9f, 1f);
            }
            else
            {
                shieldText.text = "Khiên bảo vệ: Chưa kích hoạt";
                shieldText.color = Color.white;
            }
        }
    }

    /// <summary>
    /// Kết thúc màn chơi (Thắng hoặc Thua).
    /// </summary>
    private void EndGame(bool isWin)
    {
        isGameOver = true;
        Time.timeScale = 0f;

        if (resultPanel != null)
        {
            resultPanel.SetActive(true);
        }

        if (resultTitleText != null)
        {
            if (isWin)
            {
                resultTitleText.text = $"CHIẾN THẮNG!\nTổng điểm: {score}";
                resultTitleText.color = new Color(0.2f, 1f, 0.4f);
            }
            else
            {
                resultTitleText.text = $"THẤT BẠI (GAME OVER)\nĐiểm đạt được: {score}";
                resultTitleText.color = new Color(1f, 0.3f, 0.3f);
            }
        }
    }

    /// <summary>
    /// Tải lại màn chơi hiện tại khi bấm nút Chơi lại.
    /// </summary>
    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}

