using UnityEngine;

public class CoinSpawner : MonoBehaviour
{
    public GameObject goldCoinPrefab;
    public GameObject silverCoinPrefab;
    public float goldSpawnChance = 0.1f;
    public float silverSpawnChance = 0.4f;
    public float[] allowedX = { -8f, -6f, -4f, -2f, 0f, 2f, 4f, 6f, 8f };

    public void TrySpawnCoin(GameObject row)
    {
        string tag = row.tag;
        if (tag == "WaterRow") return; // Skip water

        float chance = Random.value;

        if (chance < goldSpawnChance)
        {
            SpawnCoin(goldCoinPrefab, row.transform.position.y);
        }
        else if (chance < goldSpawnChance + silverSpawnChance)
        {
            SpawnCoin(silverCoinPrefab, row.transform.position.y);
        }
    }

    private void SpawnCoin(GameObject coinPrefab, float rowY)
    {
        float x = allowedX[Random.Range(0, allowedX.Length)];
        Vector3 spawnPos = new Vector3(x, rowY, 0f);
        Instantiate(coinPrefab, spawnPos, Quaternion.identity);
    }
}
