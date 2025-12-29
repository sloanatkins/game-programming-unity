using UnityEngine;

public class PipeSpawnScript : MonoBehaviour
{
    public GameObject pipe;
    public float spawnRate = 2f;           // Initial spawn interval (seconds)
    public float initialPipeSpeed = 5f;    // The pipe speed at the start (world units per second)

    // These values determine the vertical spawn range:
    public float heightOfSet = 3f;
    public float pipeHeightDivisorTop;
    public float pipeHeightDivisorBottom;

    private float spacing;               // Desired spacing (world units) between pipes
    private float timer = 0f;

    void Start()
    {
        // Calculate the desired spacing based on initial values.
        // For example, if the pipes move at 5 units/sec and spawn every 2 seconds,
        // they will be spaced 10 units apart.
        spacing = initialPipeSpeed * spawnRate;

        // Spawn the first pipe.
        spawnPipe();
    }

    void Update()
    {
        // Assume your PipeMoveScript has a static variable 'globalPipeSpeed'
        // that represents the current speed of the pipes.
        float currentPipeSpeed = PipeMove.globalPipeSpeed;

        // Calculate the dynamic spawn rate so that:
        // spacing = currentPipeSpeed * dynamicSpawnRate
        float dynamicSpawnRate = spacing / currentPipeSpeed;

        timer += Time.deltaTime;
        if (timer >= dynamicSpawnRate)
        {
            spawnPipe();
            timer = 0f;
        }
    }

    void spawnPipe()
    {
        float lowestPoint = transform.position.y - heightOfSet + pipeHeightDivisorBottom;
        float highestPoint = transform.position.y + heightOfSet * pipeHeightDivisorTop;
        Instantiate(pipe, new Vector3(transform.position.x, Random.Range(lowestPoint, highestPoint), 0), transform.rotation);
    }
}