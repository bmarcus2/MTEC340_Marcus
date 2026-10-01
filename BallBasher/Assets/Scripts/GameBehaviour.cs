using System.Net.NetworkInformation;
using Unity.VisualScripting;
using UnityEngine;
using TMPro;

public class GameBehavior : MonoBehaviour
{
    public static GameBehavior Instance;

    public Player Player;
    public TMP_Text scoreText;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        ResetGame();
        UpdateScoreText();
    }

    void ResetGame()
    {
        Player.Score = 0;
    }

    public void ScorePoint()
    {
        Player.Score++;

        Debug.Log("Score: " + Player.Score);

        UpdateScoreText();
    }

    void UpdateScoreText()
    {
        scoreText.text = "Score: " + Player.Score;
    }
}
