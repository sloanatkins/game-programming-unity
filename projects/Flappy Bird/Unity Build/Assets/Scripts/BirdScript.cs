using UnityEngine;

public class BirdScript : MonoBehaviour
{
    public Rigidbody2D myRigidbody;
    public float flapStrength;
    public LogicScript logic;
    public bool birdIsAlive = true;
    public AudioSource dieSFX;
    public AudioSource flapSFX;

    void Start()
    {
        logic = GameObject.FindGameObjectWithTag("Logic").GetComponent<LogicScript>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && birdIsAlive)
        {
            myRigidbody.linearVelocity = Vector2.up * flapStrength;
            flapSFX.Play();
        }

        // Check if the bird has fallen out of bounds and is still alive
        if ((transform.position.y > 17 || transform.position.y < -15) && birdIsAlive)
        {
            Die();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (birdIsAlive)
        {
            Die();
        }
    }

    private void Die()
    {
        birdIsAlive = false;
        dieSFX.Play();
        logic.gameOver();

        // Delay scene reload slightly to let the sound finish playing
        Invoke("RestartGame", 0.75f);
    }

}
