using UnityEngine;
using System.Collections;

public class FrogWaterDetector : MonoBehaviour
{
    private FrogController frog;

    void Start()
    {
        frog = GetComponentInParent<FrogController>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("WaterRow"))
        {
            StartCoroutine(CheckDrownDelay());
        }
    }

    IEnumerator CheckDrownDelay()
    {
        yield return new WaitForEndOfFrame(); // lets log detection happen
        if (!frog.IsOnLog())
        {
            frog.DieInWater();
        }
    }


}
