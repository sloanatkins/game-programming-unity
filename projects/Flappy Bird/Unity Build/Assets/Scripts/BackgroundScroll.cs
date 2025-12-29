using UnityEngine;

public class BackgroundScroll : MonoBehaviour
{
    public float scrollSpeed = 2.0f;
    private float spriteWidth;
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
        spriteWidth = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    void Update()
    {
        transform.position += Vector3.left * scrollSpeed * Time.deltaTime;

        if (transform.position.x < mainCamera.transform.position.x - spriteWidth)
        {
            RepositionBackground();
        }
    }

    void RepositionBackground()
    {
        Vector3 offset = new Vector3(spriteWidth * 2f, 0, 0);
        transform.position += offset;
    }
}
