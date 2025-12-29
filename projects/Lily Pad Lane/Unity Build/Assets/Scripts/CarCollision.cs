using UnityEngine;

public class CarCollision : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Frog"))
        {
            FrogController frog = other.GetComponent<FrogController>();
            if (frog != null)
            {
                frog.FrogHitByCar();
            }
        }
    }
}
