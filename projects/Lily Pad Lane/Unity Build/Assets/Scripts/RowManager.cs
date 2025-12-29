using UnityEngine;
using System.Collections.Generic;
using System.Collections;


public class RowManager : MonoBehaviour
{
    public Transform starterFrogPrefab;
    public Transform ribbityRicoPrefab;
    public Transform captainCroakPrefab;

    private Transform frogInstance;

    public int maxSpawnedRow = 0;
    public CoinSpawner coinSpawner;

    private HashSet<Transform> alreadyProcessed = new HashSet<Transform>();

    public LilyPadAndLogSpawner lilyPadAndLogSpawner;

    public Transform frog;
    public RowSpawner rowSpawner;
    public float rowHeight = 2.0429f;
    public GameObject grassPrefab;

    private float startY;


    void Start()
    {
        float cameraBottom = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 0)).y;
        startY = cameraBottom + rowHeight / 2f;


        for (int i = 0; i < 3; i++)
        {
           Vector3 spawnPos = new Vector3(0, startY + i * rowHeight, 0);

            GameObject grassRow = Instantiate(grassPrefab, spawnPos, Quaternion.identity);
            grassRow.tag = "Untagged";
        }


        // Step 2: Then spawn 2 random rows
        for (int i = 3; i <= 6; i++)
        {
            SpawnRow(i);
        }

        maxSpawnedRow = 6;

        // Step 3: Spawn the frog AFTER grass rows exist
        string currentUser = PlayerPrefs.GetString("currentUser", "Guest");
        string frogChoice = PlayerPrefs.GetString($"selectedFrog_{currentUser}", "starter");

        Transform chosenFrogPrefab = starterFrogPrefab;

        if (frogChoice == "mid")
            chosenFrogPrefab = ribbityRicoPrefab;
        else if (frogChoice == "top")
            chosenFrogPrefab = captainCroakPrefab;

        Vector3 startPos = new Vector3(0f, rowHeight / 2f, 0f); // safe spot on grass row 0
        frogInstance = Instantiate(chosenFrogPrefab, startPos, Quaternion.identity);
        frog = frogInstance;

        CameraFollowFrog cameraFollow = Camera.main.GetComponent<CameraFollowFrog>();
        if (cameraFollow != null)
        {
            cameraFollow.frog = frogInstance;
            cameraFollow.rowManager = this;
            cameraFollow.InitializeCameraToFrog();
        }
    }

    void Update()
    {
        if (frog == null) return;

        int frogRow = Mathf.FloorToInt(frog.position.y / rowHeight);
        int rowsNeededAhead = 4;
        int bufferRows = 3; // Extra invisible rows to avoid flashing

        EnsureRowsUpTo(frogRow + rowsNeededAhead + bufferRows);
    }


    public void SpawnRow(int rowIndex)
    {
            if (rowIndex <= 2) return;  // Don't overwrite rows 0–2

            Vector3 spawnPos = new Vector3(0, startY + rowIndex * rowHeight, 0);

            GameObject prefabUsed;
            GameObject newRow = rowSpawner.SpawnRandomRow(spawnPos, out prefabUsed);

            if (coinSpawner != null && !prefabUsed.CompareTag("WaterRow"))
            {
                coinSpawner.TrySpawnCoin(newRow);
            }

            if (prefabUsed.CompareTag("WaterRow") && !alreadyProcessed.Contains(newRow.transform))
            {
                alreadyProcessed.Add(newRow.transform);
                StartCoroutine(DelayedRefresh(newRow.transform));
            }


            if (newRow.CompareTag("RoadRow"))
            {
                FindObjectOfType<CarSpawner>().RefreshRoadRows();
            }
        }

    IEnumerator DelayedRefresh(Transform row)
    {
        yield return new WaitForEndOfFrame(); // wait for row to fully load

        if (row != null && row.CompareTag("WaterRow"))
        {
            lilyPadAndLogSpawner.RefreshWaterRowsFor(row);
        }
    }

    IEnumerator SpawnInitialRows()
    {
        // First pass: spawn all rows
        for (int i = 0; i < 10; i++)
        {
            SpawnRow(i);
            yield return null; // wait 1 frame per row for prefab setup
        }

        maxSpawnedRow = 9;

        // Second pass (after all prefabs fully instantiated)
        yield return new WaitForSeconds(0.1f); // Give Unity time to initialize tags

        foreach (Transform row in GameObject.FindObjectsOfType<Transform>())
        {
            if (row.CompareTag("WaterRow"))
            {
                if (!alreadyProcessed.Contains(row))
                {
                    alreadyProcessed.Add(row);
                    lilyPadAndLogSpawner.RefreshWaterRowsFor(row);
                }
            }

        }
    }
    public void SpawnSpecificRow(int rowIndex, GameObject prefab)
    {
        Vector3 spawnPos = new Vector3(0, rowIndex * rowHeight, 0);
        GameObject newRow = Instantiate(prefab, spawnPos, Quaternion.identity);

        if (coinSpawner != null && !newRow.CompareTag("WaterRow"))
        {
            coinSpawner.TrySpawnCoin(newRow);
        }

        if (newRow.CompareTag("WaterRow"))
        {
            if (!alreadyProcessed.Contains(newRow.transform))
            {
                alreadyProcessed.Add(newRow.transform);
                lilyPadAndLogSpawner.RefreshWaterRowsFor(newRow.transform);
            }
        }

        if (newRow.CompareTag("RoadRow"))
        {
            FindObjectOfType<CarSpawner>().RefreshRoadRows();
        }
    }

    IEnumerator InitializeGame()
        {
            // Step 1: Spawn 3 grass rows
            for (int i = 0; i < 3; i++)
            {
                Vector3 spawnPos = new Vector3(0, i * rowHeight, 0);
                GameObject grassRow = Instantiate(grassPrefab, spawnPos, Quaternion.identity);
                grassRow.tag = "Untagged";
                yield return null;
            }

            maxSpawnedRow = 2; // Last guaranteed row index (0-based)

            // Step 2: Spawn 2 random rows (starting from row 3)
            for (int i = 3; i < 5; i++)
            {
                SpawnRow(i);
                yield return null;
            }

            maxSpawnedRow = 4;
            yield return new WaitForEndOfFrame(); // Let row tags/objects finish initializing

            // Step 3: Now spawn the frog on row 0
            string frogChoice = PlayerPrefs.GetString("currentFrog", "starter");
            Transform chosenFrogPrefab = starterFrogPrefab;

            if (frogChoice == "ribbity_rico") chosenFrogPrefab = ribbityRicoPrefab;
            else if (frogChoice == "captain_croak") chosenFrogPrefab = captainCroakPrefab;

            Vector3 frogStartPos = new Vector3(0f, rowHeight / 2f, 0f);
            frogInstance = Instantiate(chosenFrogPrefab, frogStartPos, Quaternion.identity);
            frog = frogInstance;

            // Hook up camera after frog is created
            CameraFollowFrog cameraFollow = Camera.main.GetComponent<CameraFollowFrog>();
            if (cameraFollow != null)
            {
                cameraFollow.frog = frogInstance;
                cameraFollow.rowManager = this;
                cameraFollow.InitializeCameraToFrog();
            }

            yield return new WaitForSeconds(0.5f); // Let frog initialize too
            FrogController.instance.StartCoroutine(FrogController.instance.PostJumpCheck());
        }

        public void EnsureRowsUpTo(int targetRow)
        {
            if (targetRow > maxSpawnedRow)
            {
                for (int i = maxSpawnedRow + 1; i <= targetRow; i++)
                {
                    SpawnRow(i);
                }
                maxSpawnedRow = targetRow;
            }
        }


}

