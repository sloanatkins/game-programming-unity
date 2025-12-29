using UnityEngine;

public class MoveObject : MonoBehaviour
{
    public float speed;
    public Vector3 direction;

    void Update()
    {
        transform.position += direction * speed * Time.deltaTime;

        if (Mathf.Abs(transform.position.x) > 20f)
            Destroy(gameObject);
    }
}