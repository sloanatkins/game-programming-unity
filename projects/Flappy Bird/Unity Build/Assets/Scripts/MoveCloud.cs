using UnityEngine;

public class MoveCloud : MonoBehaviour
{
    public float moveSpeed = 3f; // Speed of the cloud movement
    public float resetX = -30f; // X position where cloud resets (left side of the screen)
    public float startX = 30f;  // Starting X position of the cloud (right side of the screen)
    public float minY = -7f;      // Minimum Y position (bottom)
    public float maxY = 12f;       // Maximum Y position (top)


    void Start()
    {
        // Start the cloud at a random Y position within the given range
        transform.position = new Vector3(startX, Random.Range(minY, maxY), 0);
    }

    void Update()
    {
        // Move the cloud to the left as the bird progresses
        transform.position += Vector3.left * moveSpeed * Time.deltaTime;

        // If the cloud moves past resetX, reposition it to the right side of the screen
        if (transform.position.x < resetX)
        {
            transform.position = new Vector3(30, Random.Range(minY, maxY), 0);
        }
    }
}
