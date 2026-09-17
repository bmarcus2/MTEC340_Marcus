using UnityEngine;
public class PaddleBehavior : MonoBehaviour


{
   public float Speed = 5.0f;
  
  public KeyCode Rightdirection = KeyCode.RightArrow;
    public KeyCode Leftdirection = KeyCode.LeftArrow;
    void Update()
    {
        float movement = 0.0f;

        if (Input.GetKey(KeyCode.RightArrow))
        {
            movement += Speed;
        }
              if (Input.GetKey(KeyCode.LeftArrow))
        {
            movement -= Speed;
        }

        movement *= Time.deltaTime;

        transform.Translate(0.0f, movement, 0.0f);
    }
}
