

using UnityEngine;
using TMPro;

public enum GameState
{
    Home,
    Playing,
    Paused,
    GameOver,
    Victory
}

public sealed class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("State")]
    [SerializeField] private GameState currentState = GameState.Home;
    public GameState CurrentState => currentState;
    public bool IsPlaying => currentState == GameState.Playing;

    [Header("Score")]
    [SerializeField] private int score;

    public int Score
    {
        get => score;
        private set
        {
            score = Mathf.Max(0, value);

            if (scoreText != null)
                scoreText.text = $"Score: {score}";
        }
    }

    [Header("UI")]
    [SerializeField] private GameObject homePanel;
    [SerializeField] private GameObject hudPanel;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject victoryPanel;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text waveText;
    [SerializeField] private TMP_Text finalScoreText;

    [Header("Managers")]
    [SerializeField] private WaveManager waveManager;

    [Header("Gameplay Objects")]
    [SerializeField] private GameObject playerObject;

    [Header("Player Reset")]
    [SerializeField] private Transform playerSpawnPoint;

    [Header("Audio")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioClip music;
    [SerializeField] private AudioClip shootSfx;
    [SerializeField] private AudioClip enemyDeathSfx;
    [SerializeField] private AudioClip powerUpSfx;
    [SerializeField] private AudioClip uiSfx;

    private Vector3 initialPlayerPosition;
    private Quaternion initialPlayerRotation;
    private bool hasSavedPlayerPosition;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        Time.timeScale = 1f;

        if (playerObject != null)
        {
            initialPlayerPosition = playerObject.transform.position;
            initialPlayerRotation = playerObject.transform.rotation;
            hasSavedPlayerPosition = true;
        }

        Score = 0;
    }

    private void Start()
    {
        if (musicSource != null && music != null)
        {
            musicSource.clip = music;
            musicSource.loop = true;
            musicSource.Play();
        }

        ClearGameplayRemnants();

        if (playerObject != null)
            playerObject.SetActive(false);

        SetState(GameState.Home);
    }

    private void Update()
    {
        if (currentState == GameState.Home &&
            Input.GetKeyDown(KeyCode.Return))
        {
            StartGame();
        }
        else if (currentState == GameState.Playing &&
                 Input.GetKeyDown(KeyCode.Escape))
        {
            PauseGame();
        }
        else if (currentState == GameState.Paused &&
                 Input.GetKeyDown(KeyCode.Escape))
        {
            ResumeGame();
        }
    }

    public void StartGame()
    {
        Time.timeScale = 1f;

        ClearGameplayRemnants();
        ResetPlayer();

        Score = 0;

        if (waveText != null)
            waveText.text = "Wave: 1";

        SetState(GameState.Playing);

        if (waveManager != null)
            waveManager.StartWaves();

        PlaySfx(uiSfx);
    }

    private void ResetPlayer()
    {
        if (playerObject == null)
            return;

        playerObject.SetActive(true);

        if (playerSpawnPoint != null)
        {
            playerObject.transform.position =
                playerSpawnPoint.position;

            playerObject.transform.rotation =
                playerSpawnPoint.rotation;
        }
        else if (hasSavedPlayerPosition)
        {
            playerObject.transform.position =
                initialPlayerPosition;

            playerObject.transform.rotation =
                initialPlayerRotation;
        }

        Rigidbody2D rb2D =
            playerObject.GetComponent<Rigidbody2D>();

        if (rb2D != null)
        {
            rb2D.linearVelocity = Vector2.zero;
            rb2D.angularVelocity = 0f;
        }

        Rigidbody rb3D =
            playerObject.GetComponent<Rigidbody>();

        if (rb3D != null)
        {
            rb3D.linearVelocity = Vector3.zero;
            rb3D.angularVelocity = Vector3.zero;
        }

        PlayerController playerController =
            playerObject.GetComponent<PlayerController>();

        if (playerController != null)
        {
            playerController.ResetPowerEffects();
        }
    }

    private void ClearGameplayRemnants()
    {
        foreach (GameObject enemy in
                 GameObject.FindGameObjectsWithTag("Enemy"))
        {
            Destroy(enemy);
        }

        PowerUp[] powerUps =
            FindObjectsByType<PowerUp>(
                FindObjectsSortMode.None);

        foreach (PowerUp powerUp in powerUps)
        {
            Destroy(powerUp.gameObject);
        }
    }

    public void AddScore(int amount)
    {
        if (!IsPlaying)
            return;

        Score += amount;
    }

    public void SetWave(int wave)
    {
        if (waveText != null)
            waveText.text = $"Wave: {wave}";
    }

    public void PauseGame()
    {
        if (!IsPlaying)
            return;

        SetState(GameState.Paused);
        Time.timeScale = 0f;

        PlaySfx(uiSfx);
    }

    public void ResumeGame()
    {
        if (currentState != GameState.Paused)
            return;

        Time.timeScale = 1f;
        SetState(GameState.Playing);

        PlaySfx(uiSfx);
    }

    public void GameOver()
    {
        if (!IsPlaying)
            return;

        if (waveManager != null)
            waveManager.StopWaves();

        Time.timeScale = 0f;
        SetState(GameState.GameOver);

        if (finalScoreText != null)
            finalScoreText.text = $"Final Score: {Score}";

        PlaySfx(enemyDeathSfx);
    }

    public void WinGame()
    {
        if (!IsPlaying)
            return;

        if (waveManager != null)
            waveManager.StopWaves();

        Time.timeScale = 0f;
        SetState(GameState.Victory);
    }

    public void ReturnHome()
    {
        if (waveManager != null)
            waveManager.StopWaves();

        Time.timeScale = 1f;

        ClearGameplayRemnants();

        if (playerObject != null)
            playerObject.SetActive(false);

        SetState(GameState.Home);

        PlaySfx(uiSfx);
    }

    public void PlayShootSfx() =>
        PlaySfx(shootSfx);

    public void PlayEnemyDeathSfx() =>
        PlaySfx(enemyDeathSfx);

    public void PlayPowerUpSfx() =>
        PlaySfx(powerUpSfx);

    public void PlaySfx(AudioClip clip)
    {
        if (sfxSource != null && clip != null)
            sfxSource.PlayOneShot(clip);
    }

    public void QuitGame()
    {
        Time.timeScale = 1f;
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    private void SetState(GameState newState)
    {
        currentState = newState;

        if (homePanel != null)
            homePanel.SetActive(newState == GameState.Home);

        if (hudPanel != null)
        {
            hudPanel.SetActive(
                newState == GameState.Playing ||
                newState == GameState.Paused);
        }

        if (pausePanel != null)
            pausePanel.SetActive(newState == GameState.Paused);

        if (gameOverPanel != null)
            gameOverPanel.SetActive(newState == GameState.GameOver);

        if (victoryPanel != null)
            victoryPanel.SetActive(newState == GameState.Victory);
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
            Time.timeScale = 1f;
        }
    }
}
