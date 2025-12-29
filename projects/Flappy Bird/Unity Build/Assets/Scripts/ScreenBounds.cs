using UnityEngine;

public class ScreenBounds : MonoBehaviour
{
    void Start()
    {
        // Get the Camera's orthographic size (only applies to orthographic cameras)
        Camera cam = Camera.main;
        float orthoSize = cam.orthographicSize;

        // Get the camera's aspect ratio
        float aspectRatio = cam.aspect;

        // Calculate the screen's boundaries (in world space)
        float screenMinX = -orthoSize * aspectRatio;
        float screenMaxX = orthoSize * aspectRatio;
        float screenMinY = -orthoSize;
        float screenMaxY = orthoSize;

        // Output the results to the console
        Debug.Log("Screen Min X: " + screenMinX);
        Debug.Log("Screen Max X: " + screenMaxX);
        Debug.Log("Screen Min Y: " + screenMinY);
        Debug.Log("Screen Max Y: " + screenMaxY);
    }
}
