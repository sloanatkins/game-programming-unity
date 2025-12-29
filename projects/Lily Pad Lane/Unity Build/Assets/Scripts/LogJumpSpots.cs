using UnityEngine;

public class LogJumpSpots : MonoBehaviour
{
    public Transform leftSpot;
    public Transform rightSpot;

    public Transform GetClosestSpot(Vector3 frogPos)
    {
        float distLeft = Vector3.Distance(frogPos, leftSpot.position);
        float distRight = Vector3.Distance(frogPos, rightSpot.position);
        return distLeft < distRight ? leftSpot : rightSpot;
    }
}
