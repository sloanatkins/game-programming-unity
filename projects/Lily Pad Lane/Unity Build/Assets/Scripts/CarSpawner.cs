using UnityEngine;
using System.Collections.Generic;

public class CarSpawner : MonoBehaviour
{
    public GameObject[] leftCarPrefabs;   // For cars moving left
    public GameObject[] rightCarPrefabs;  // For cars moving right
    public float carSpeedMin = 5f;
    public float carSpeedMax = 7f;
    public float carSpawnInterval = 0.5f;

    private List<Transform> roadRows = new List<Transform>();
    private Dictionary<Transform, Vector3> rowDirections = new Dictionary<Transform, Vector3>();
    private Dictionary<Transform, float> nextSpawnTimePerRow = new Dictionary<Transform, float>();
    private Dictionary<Transform, GameObject> lastCarPerRow = new Dictionary<Transform, GameObject>();

    void Start()
    {
        ApplyDifficultySettings();

        GameObject[] roadRowObjects = GameObject.FindGameObjectsWithTag("RoadRow");
        foreach (GameObject rowObj in roadRowObjects)
        {
            Transform row = rowObj.transform;
            roadRows.Add(row);

            // Assign fixed direction per row
            if (!rowDirections.ContainsKey(row))
            {
                bool goRight = Random.value > 0.5f;
                rowDirections[row] = goRight ? Vector3.right : Vector3.left;
            }

            nextSpawnTimePerRow[row] = Time.time;
        }

        InvokeRepeating(nameof(SpawnCars), 1f, carSpawnInterval);
    }

    void ApplyDifficultySettings()
    {
        switch (DifficultyManager.currentDifficulty)
        {
            case DifficultyManager.DifficultyLevel.Easy:
                carSpeedMin = 4f;
                carSpeedMax = 6f;
                break;
            case DifficultyManager.DifficultyLevel.Medium:
                carSpeedMin = 6f;
                carSpeedMax = 8f;
                break;
            case DifficultyManager.DifficultyLevel.Hard:
                carSpeedMin = 8f;
                carSpeedMax = 10f;
                break;
        }
    }

    public void RefreshRoadRows()
    {
        roadRows.Clear();
        GameObject[] roadRowObjects = GameObject.FindGameObjectsWithTag("RoadRow");
        foreach (GameObject rowObj in roadRowObjects)
        {
            Transform row = rowObj.transform;
            if (!roadRows.Contains(row))
            {
                roadRows.Add(row);

                if (!rowDirections.ContainsKey(row))
                {
                    bool goRight = Random.value > 0.5f;
                    rowDirections[row] = goRight ? Vector3.right : Vector3.left;
                }

                nextSpawnTimePerRow[row] = Time.time;
            }
        }
    }

    void SpawnCars()
    {
        float currentTime = Time.time;

        foreach (Transform row in roadRows)
        {
            if (currentTime < nextSpawnTimePerRow[row]) continue;

            Vector3 dir = rowDirections[row];
            float spawnX = dir == Vector3.right ? -12f : 12f;
            float y = row.position.y;
            float speed = Random.Range(carSpeedMin, carSpeedMax);

            if (lastCarPerRow.TryGetValue(row, out GameObject lastCar) && lastCar != null)
            {
                float lastCarX = lastCar.transform.position.x;
                float lastCarWidth = lastCar.GetComponent<SpriteRenderer>().bounds.size.x;
                float minSpacing = lastCarWidth * 1.5f; // add safety buffer

                bool tooClose = dir == Vector3.right
                    ? lastCarX < spawnX + minSpacing
                    : lastCarX > spawnX - minSpacing;

                if (tooClose) continue;
            }

            GameObject[] prefabArray = dir == Vector3.right ? rightCarPrefabs : leftCarPrefabs;
            GameObject selectedPrefab = prefabArray[Random.Range(0, prefabArray.Length)];

            GameObject car = Instantiate(selectedPrefab, new Vector3(spawnX, y, 0), Quaternion.identity);
            car.transform.rotation = Quaternion.identity;

            Rigidbody2D rb = car.GetComponent<Rigidbody2D>();
            if (rb == null) rb = car.AddComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.gravityScale = 0;

            BoxCollider2D col = car.GetComponent<BoxCollider2D>();
            if (col == null) col = car.AddComponent<BoxCollider2D>();
            col.isTrigger = true;

            MoveObject mover = car.AddComponent<MoveObject>();
            mover.direction = dir;
            mover.speed = speed;

            float carWidth = car.GetComponent<SpriteRenderer>().bounds.size.x;
            float delay = carWidth / speed;
            nextSpawnTimePerRow[row] = currentTime + delay;

            lastCarPerRow[row] = car;
        }
    }
}
