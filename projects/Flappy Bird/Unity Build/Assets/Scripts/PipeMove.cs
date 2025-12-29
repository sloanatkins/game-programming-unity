using UnityEngine;

public class PipeMove : MonoBehaviour
{
    // Global speed shared by all pipes.
    public static float globalPipeSpeed = 5f;
    public float acceleration = 0.1f;
    public float maxSpeed = 15f;
    public float deadZone = -30f;

    void Update()
    {
        // Increase the global speed over time until it reaches maxSpeed.
        globalPipeSpeed = Mathf.Min(globalPipeSpeed + acceleration * Time.deltaTime, maxSpeed);

        // Move the pipe using the global speed.
        transform.position += Vector3.left * globalPipeSpeed * Time.deltaTime;

        // Destroy the pipe if it goes off-screen.
        if (transform.position.x < deadZone)
        {
            Destroy(gameObject);
        }
    }
}
