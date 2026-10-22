
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("UI Panels")]
    public GameObject startMenuPanel;
    public GameObject gameOverPanel;
    public GameObject pausePanel;
    public GameObject settingsPanel;

    [Header("Score Texts")]
    public TMP_Text scoreText;
    public TMP_Text finalScoreText;
    public TMP_Text highScoreText;

    [Header("Audio Settings Toggles")]
    public Toggle musicToggle;
    public Toggle sfxToggle;

    public bool IsPlaying { get; private set; }
    private float score;
    private int highScore;
    private int lastMilestone;

    private void Awake()
    {
        Instance = this;
        Time.timeScale = 1f;

        // Ensure smooth 60 FPS on Android and iOS (Unity defaults to 30 FPS on mobile otherwise)
        Application.targetFrameRate = 60;

        // Load saved High Score
        highScore = PlayerPrefs.GetInt("DinoHighScore", 0);
    }

    private void Start()
    {
        // Start in Menu state if startMenuPanel is assigned
        if (startMenuPanel != null)
        {
            IsPlaying = false;
            startMenuPanel.SetActive(true);
        }
        else
        {
            // If no start panel assigned yet, play immediately
            IsPlaying = true;
        }

        score = 0f;
        lastMilestone = 0;

        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (pausePanel != null) pausePanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(false);

        // Initialize Settings Toggles from saved audio preferences
        if (AudioManager.Instance != null)
        {
            if (musicToggle != null)
            {
                musicToggle.isOn = AudioManager.Instance.IsMusicEnabled;
                musicToggle.onValueChanged.AddListener(OnMusicToggleChanged);
            }

            if (sfxToggle != null)
            {
                sfxToggle.isOn = AudioManager.Instance.IsSFXEnabled;
                sfxToggle.onValueChanged.AddListener(OnSFXToggleChanged);
            }
        }

        UpdateHighScoreDisplay();
    }

    private void Update()
    {
        if (!IsPlaying) return;

        score += Time.deltaTime * 10f;
        int currentScore = Mathf.FloorToInt(score);

        // Play milestone chime every 100 points
        if (currentScore / 100 > lastMilestone)
        {
            lastMilestone = currentScore / 100;
            if (AudioManager.Instance != null)
                AudioManager.Instance.PlayScore();
        }

        if (scoreText != null)
            scoreText.text = "SCORE: " + currentScore.ToString("D5");
    }

    // --- BUTTON EVENT HANDLERS ---

    public void StartGame()
    {
        PlayClickSound();
        IsPlaying = true;
        Time.timeScale = 1f;

        if (startMenuPanel != null) startMenuPanel.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (pausePanel != null) pausePanel.SetActive(false);
    }

    public void GameOver()
    {
        if (!IsPlaying) return;

        IsPlaying = false;
        int currentScore = Mathf.FloorToInt(score);

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayHit();

        // Check & Save High Score
        if (currentScore > highScore)
        {
            highScore = currentScore;
            PlayerPrefs.SetInt("DinoHighScore", highScore);
            PlayerPrefs.Save();
        }

        UpdateHighScoreDisplay();

        if (finalScoreText != null)
            finalScoreText.text = "SCORE: " + currentScore.ToString("D5");

        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);
    }

    public void PauseGame()
    {
        if (!IsPlaying) return;

        PlayClickSound();
        Time.timeScale = 0f;
        if (pausePanel != null) pausePanel.SetActive(true);
    }

    public void ResumeGame()
    {
        PlayClickSound();
        Time.timeScale = 1f;
        if (pausePanel != null) pausePanel.SetActive(false);
    }

    public void OpenSettings()
    {
        PlayClickSound();
        if (settingsPanel != null) settingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        PlayClickSound();
        if (settingsPanel != null) settingsPanel.SetActive(false);
    }

    public void RestartGame()
    {
        PlayClickSound();
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // --- AUDIO SETTINGS TOGGLE HANDLERS ---

    public void OnMusicToggleChanged(bool isEnabled)
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetMusicEnabled(isEnabled);
            AudioManager.Instance.PlayClick();
        }
    }

    public void OnSFXToggleChanged(bool isEnabled)
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetSFXEnabled(isEnabled);
            AudioManager.Instance.PlayClick();
        }
    }

    public void ToggleMusic()
    {
        if (AudioManager.Instance != null)
        {
            bool state = AudioManager.Instance.ToggleMusic();
            if (musicToggle != null) musicToggle.isOn = state;
            AudioManager.Instance.PlayClick();
        }
    }

    public void ToggleSFX()
    {
        if (AudioManager.Instance != null)
        {
            bool state = AudioManager.Instance.ToggleSFX();
            if (sfxToggle != null) sfxToggle.isOn = state;
            AudioManager.Instance.PlayClick();
        }
    }

    private void PlayClickSound()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayClick();
    }

    private void UpdateHighScoreDisplay()
    {
        if (highScoreText != null)
            highScoreText.text = "HIGH: " + highScore.ToString("D5");
    }
}
