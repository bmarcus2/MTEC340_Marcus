using UnityEngine;

public class Player : MonoBehaviour
{
    private int _score;

    public int Score
    {
        get => _score;

        set
        {
            _score = value;
        }
    }
}

