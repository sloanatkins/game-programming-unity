using UnityEngine;

public class Coin : MonoBehaviour
{
    public int value = 1; // Silver coin = 1, Gold coin = 5

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("COLLECTED COIN from: " + other.name);

            CoinManager.instance.AddCoins(value);
//            SFXManager.instance.PlayCoin();

            Destroy(gameObject);
        }
    }
}
