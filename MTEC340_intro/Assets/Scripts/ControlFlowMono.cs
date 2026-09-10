using UnityEngine;

public class ControlFlowMono : MonoBehaviour
{public bool flag;
void Start()
    {
        if (flag)
        {
            Debug.Log("Boolean flag is set");
        }
        else
        {
            Debug.Log("Boolean flag is not set");
        }

        for (int x = 1; x <= 10; x++)
        {
            int y = (int)Mathf.Pow(2, x);
            Debug.Log($"The {x} power of 2 is {y}");
        }
    }
}