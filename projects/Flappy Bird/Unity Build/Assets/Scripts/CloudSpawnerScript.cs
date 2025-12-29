using UnityEngine;

public class CloudSpawner : MonoBehaviour
{
    public GameObject cloudPrefab;    // The cloud prefab
    public int cloudPoolSize = 6;     // Number of clouds to be active at any time
    public float minY = -7f, maxY = 12f; // Random Y position range for clouds
    public float spawnX = 30f;    // X position for spawning clouds (off-screen right)
    public float spawnInterval = 5f;  // Time interval between cloud spawn checks

    private GameObject[] cloudPool;   // Array to hold the pool of cloud objects
    private float timer;

    void Start()
    {
        // Initialize the cloud pool
        cloudPool = new GameObject[cloudPoolSize];
        for (int i = 0; i < cloudPoolSize; i++)
        {
            // Instantiate the cloud and deactivate it
            cloudPool[i] = Instantiate(cloudPrefab);
            cloudPool[i].SetActive(false);
        }
    }

    void Update()
    {
        // Timer counts down and spawns a new cloud if one is off-screen
        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            CheckAndRespawnClouds();
            timer = 0f; // Reset the timer
        }
    }

    void CheckAndRespawnClouds()
    {
        // Loop through the cloud pool to check if any clouds need to be reset (i.e., moved off-screen to the left)
        for (int i = 0; i < cloudPool.Length; i++)
        {
            // Check if the cloud has gone off-screen
            if (!cloudPool[i].activeInHierarchy || cloudPool[i].transform.position.x < -23f) // Off-screen check
            {
                // Reposition the cloud to the right side and make it active again
                cloudPool[i].transform.position = new Vector3(30, Random.Range(minY, maxY), 0);
                cloudPool[i].SetActive(true);
                break; // Only respawn one cloud at a time
            }
        }
    }
}
