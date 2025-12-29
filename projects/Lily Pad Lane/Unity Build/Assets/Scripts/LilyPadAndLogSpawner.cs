using UnityEngine;
using System.Collections.Generic;

public class LilyPadAndLogSpawner : MonoBehaviour
{
    public GameObject[] lilyPadPrefabs;
    public GameObject[] logPrefabs;
    public float logSpawnInterval = 3f;
    public float logSpeedMin = 3f;
    public float logSpeedMax = 4.5f;
    public int lilyPadsPerRow = 3;

    private List<Transform> waterRows = new List<Transform>();
    private HashSet<Transform> lilyPadRows = new HashSet<Transform>();
    private Dictionary<Transform, GameObject> lastLogPerRow = new Dictionary<Transform, GameObject>();
    private Dictionary<Transform, float> nextSpawnTimePerRow = new Dictionary<Transform, float>();
    private Dictionary<Transform, Vector3> rowDirections = new Dictionary<Transform, Vector3>();

    private HashSet<Transform> processedRows = new HashSet<Transform>();

    // Frog’s X jump positions (9 total)
    private readonly float[] frogJumpPositions = new float[]
    {
        -8f, -6f, -4f, -2f, 0f, 2f, 4f, 6f, 8f
    };

    void Start()
        {
            ApplyDifficultySettings();
            InvokeRepeating(nameof(SpawnLog), 1f, logSpawnInterval);
        }

    void ApplyDifficultySettings()
    {
        switch (DifficultyManager.currentDifficulty)
        {
            case DifficultyManager.DifficultyLevel.Easy:
                logSpeedMin = 3f;
                logSpeedMax = 4.5f;
                break;
            case DifficultyManager.DifficultyLevel.Medium:
                logSpeedMin = 4.5f;
                logSpeedMax = 6f;
                break;
            case DifficultyManager.DifficultyLevel.Hard:
                logSpeedMin = 6f;
                logSpeedMax = 7.5f;
                break;
        }
    }

    public void RefreshWaterRowsFor(Transform newRow)
    {
        // Skip if not a WaterRow
        if (!newRow.CompareTag("WaterRow")) return;

        // Skip if we already processed this row
        if (processedRows.Contains(newRow)) return;
        processedRows.Add(newRow);

        // Add to waterRows list only once
        if (!waterRows.Contains(newRow))
            waterRows.Add(newRow);

        // Guard against duplicate assignment
        if (lilyPadRows.Contains(newRow) || rowDirections.ContainsKey(newRow))
            return;

        // Assign either lilypads or logs
        bool useLilypads = Random.value > 0.5f;

        if (useLilypads)
        {
            lilyPadRows.Add(newRow);
            SpawnLilyPads(newRow);
        }
        else
        {
            ScheduleLogSpawning(newRow);
        }
    }


    void SpawnLilyPads(Transform row)
    {
        List<float> availableX = new List<float>(frogJumpPositions);
        Shuffle(availableX);

        for (int i = 0; i < Mathf.Min(lilyPadsPerRow, availableX.Count); i++)
        {
            float x = availableX[i];
            Vector3 spawnPos = new Vector3(x, row.position.y, 0);
            int index = Random.Range(0, lilyPadPrefabs.Length);
            GameObject lilyPad = Instantiate(lilyPadPrefabs[index], spawnPos, Quaternion.identity);
            lilyPad.transform.rotation = Quaternion.identity;
            SetSortingOrder(lilyPad, 1);
        }
    }

    void ScheduleLogSpawning(Transform row)
    {
        bool isEven = row.GetSiblingIndex() % 2 == 0;
        rowDirections[row] = isEven ? Vector3.right : Vector3.left;
        nextSpawnTimePerRow[row] = Time.time;

        InvokeRepeating(nameof(SpawnLog), 1f, logSpawnInterval);
    }

    void SpawnLog()
    {
        float currentTime = Time.time;

        foreach (Transform row in waterRows)
        {
            if (lilyPadRows.Contains(row)) continue;

            if (!nextSpawnTimePerRow.ContainsKey(row)) continue;
            if (currentTime < nextSpawnTimePerRow[row]) continue;

            Vector3 dir = rowDirections[row];
            float spawnX = dir == Vector3.right ? -12f : 12f;
            float y = row.position.y;
            float speed = Random.Range(logSpeedMin, logSpeedMax);

            // Check spacing from last log
            if (lastLogPerRow.TryGetValue(row, out GameObject lastLog) && lastLog != null)
            {
                float minSpacing = lastLog.GetComponent<SpriteRenderer>().bounds.size.x + 5f;
                if (Mathf.Abs(lastLog.transform.position.x - spawnX) < minSpacing)
                    continue;
            }

            // Spawn log
            GameObject prefab = logPrefabs[Random.Range(0, logPrefabs.Length)];
            GameObject log = Instantiate(prefab, new Vector3(spawnX, y, 0), Quaternion.identity);
            log.transform.rotation = Quaternion.identity;
            SetSortingOrder(log, 1);

            Rigidbody2D rb = log.GetComponent<Rigidbody2D>();
            if (rb == null) rb = log.AddComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.gravityScale = 0;
            rb.freezeRotation = true;

            BoxCollider2D col = log.GetComponent<BoxCollider2D>();
            if (col == null) col = log.AddComponent<BoxCollider2D>();
            col.isTrigger = true;

            MoveObject mover = log.AddComponent<MoveObject>();
            mover.direction = dir;
            mover.speed = speed;

            // Delay next log spawn based on spacing
            float width = log.GetComponent<SpriteRenderer>().bounds.size.x;
            float delay = (width + 0.5f) / speed;
            nextSpawnTimePerRow[row] = currentTime + delay;

            lastLogPerRow[row] = log;
        }
    }

    void SetSortingOrder(GameObject obj, int order)
    {
        SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();
        if (sr != null)
            sr.sortingOrder = order;
    }

    void Shuffle<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int j = Random.Range(i, list.Count);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }
}
