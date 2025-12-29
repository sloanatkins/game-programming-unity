using UnityEngine;

public class FrogStartAligner : MonoBehaviour
{
    public float rowHeight = 2.0429f; // Set this to match your actual row height

    void Start()
    {
        float cameraBottom = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 0)).y;
        float firstRowY = cameraBottom + rowHeight / 2f;

        // Snap to center X position (e.g., 0) and first row Y
        transform.position = new Vector3(0f, firstRowY, 0f);
    }
}
