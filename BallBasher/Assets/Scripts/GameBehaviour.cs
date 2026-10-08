using System.Net.NetworkInformation;
using Unity.VisualScripting;
using UnityEngine;
using TMPro;
using Unity.Tutorials.Editor;

public class GameBehavior : MonoBehaviour
{
    public static GameBehavior Instance;

    public Utilities.GameState State;

    public Player Player;
    public TMP_Text scoreText;

    [SerializeField] private int _winningScore = 5;
    [SerializeField] private GameObject _pausePanel;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        State = Utilities.GameState.Play;
    }

    private void Start()
    {
        ResetGame();
        UpdateScoreText();

        
        _pausePanel.SetActive(false);
    }

    private void ResetGame()
    {
        Player.Score = 0;
    }

    public void ScorePoint()
    {
        Player.Score++;

        Debug.Log("Score: " + Player.Score);

        UpdateScoreText();

        if (Player.Score >= _winningScore)
        {
            Debug.Log("You Win!");
        }
    }

    private void UpdateScoreText()
    {
        scoreText.text = "Score: " + Player.Score;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (State == Utilities.GameState.Play)
            {
                State = Utilities.GameState.Pause;

                
                _pausePanel.SetActive(true);

                Debug.Log("Game Paused");
            }
            else
            {
                State = Utilities.GameState.Play;

                
                _pausePanel.SetActive(false);

                Debug.Log("Game Resumed");
            }
        }
    }
}