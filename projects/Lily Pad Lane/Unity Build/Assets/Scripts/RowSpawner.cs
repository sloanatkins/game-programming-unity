using UnityEngine;

public class RowSpawner : MonoBehaviour
{
    public GameObject grassPrefab;         // Prefab for grass row
    public GameObject[] rowPrefabs;        // Random row prefabs (road, water, etc.)
    public int numberOfRandomRows = 10;    // How many to spawn after the grass
    public float startY = 0f;              // Starting Y position (bottom of screen)

    float GetRowHeight(GameObject prefab)
    {
        return prefab.GetComponentInChildren<SpriteRenderer>().bounds.size.y;
    }

        public GameObject SpawnRandomRow(Vector3 position, out GameObject prefabUsed)
        {
            prefabUsed = rowPrefabs[Random.Range(0, rowPrefabs.Length)];
            GameObject row = Instantiate(prefabUsed, position, Quaternion.identity);

            // Ensure tag is copied
            row.tag = prefabUsed.tag;
            if (row.tag == "WaterRow")
            {
                FindObjectOfType<LilyPadAndLogSpawner>().RefreshWaterRowsFor(row.transform);
            }
            else if (row.tag == "RoadRow")
            {
                FindObjectOfType<CarSpawner>().RefreshRoadRows(); // same logic here
            }

            return row;



        }
}
