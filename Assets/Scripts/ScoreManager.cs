using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    private int score = 0;
    public TextMeshProUGUI scoreText; // ������ �� ��������� �������

    public void ResetScore()
    {
        score = 0;
        UpdateScoreUI();
    }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // ������� ��������� ������� ��� ������
        scoreText = GameObject.Find("ScoreText").GetComponent<TextMeshProUGUI>();
        UpdateScoreUI();
    }

    public void RefreshScoreTextReference()
    {
        scoreText = GameObject.Find("ScoreText").GetComponent<TextMeshProUGUI>();
        UpdateScoreUI();
    }

    public void AddScore(int points)
    {
        score += points;
        UpdateScoreUI();
        Debug.Log("Score: " + score);

        // Play score sound
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.scoreClip);
        }
    }

    public int GetScore()
    {
        return score;
    }

    void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = $"Score: {score}";
        }
    }

    // ��� ������ ��� ������������ �����
    void OnEnable()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode)
    {
        // ��� �������� ����� ����� ������� ��������� ������� �����
        GameObject scoreTextObj = GameObject.Find("ScoreText");
        if (scoreTextObj != null)
        {
            scoreText = scoreTextObj.GetComponent<TextMeshProUGUI>();
            UpdateScoreUI();
        }
    }
}