using UnityEngine;
using System.Collections;

public class CameraFollowFrog : MonoBehaviour
{

    public RowManager rowManager; // Drag RowManager GameObject in the Inspector

    public Transform frog; // Assign in inspector
    public float rowHeight = 2.0429f; // Your row height is 2 units
    public float smoothSpeed = 5f;

    private int cameraRowIndex = 0;
    private Coroutine moveRoutine;

    private Camera mainCamera;

    private bool readyToSpawn = false;



    void Start()
    {
        mainCamera = Camera.main; // Get the main camera
    }

    public void InitializeCameraToFrog()
    {
        if (frog == null) return;

        cameraRowIndex = Mathf.FloorToInt(frog.position.y / rowHeight);
        float initialY = cameraRowIndex * rowHeight;
        transform.position = new Vector3(transform.position.x, initialY, transform.position.z);
    }

    void Update()
    {
        float cameraTopY = mainCamera.ViewportToWorldPoint(new Vector3(0, 1, 0)).y;
        float nextRowY = (rowManager.maxSpawnedRow + 1) * rowHeight;

        if (nextRowY < cameraTopY + (2 * rowHeight)) // Spawns early
        {
            rowManager.SpawnRow(rowManager.maxSpawnedRow + 1);
            rowManager.maxSpawnedRow++;

        }

    }

    public void UpdateCameraTarget(float frogY, float previousFrogY)
    {
        int frogRow = Mathf.FloorToInt(frogY / rowHeight);
        int prevRow = Mathf.FloorToInt(previousFrogY / rowHeight);

        int clampedTargetRow = Mathf.Max(frogRow, 1);


        // Only move if frog jumps vertically and reaches a row where camera needs to move
        if (frogRow > cameraRowIndex - 1)
        {
            cameraRowIndex++;
            float targetY = cameraRowIndex * rowHeight;

            if (moveRoutine != null)
                StopCoroutine(moveRoutine);
            moveRoutine = StartCoroutine(SmoothMove(targetY));
        }
        else if (frogRow < cameraRowIndex - 1)
        {
            cameraRowIndex--;
            float targetY = cameraRowIndex * rowHeight;

            if (moveRoutine != null)
                StopCoroutine(moveRoutine);
            moveRoutine = StartCoroutine(SmoothMove(targetY));
        }
    }

    IEnumerator SmoothMove(float targetY)
    {
        Vector3 start = transform.position;
        Vector3 end = new Vector3(start.x, targetY, start.z);

        while (Mathf.Abs(transform.position.y - targetY) > 0.01f)
        {
            transform.position = Vector3.Lerp(transform.position, end, smoothSpeed * Time.deltaTime);
            yield return null;
        }

        transform.position = end;
    }


}
