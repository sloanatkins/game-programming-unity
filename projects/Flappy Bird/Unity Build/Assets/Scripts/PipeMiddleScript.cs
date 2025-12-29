using UnityEngine;

public class PipeMiddleScript : MonoBehaviour
{
    public LogicScript logic;
    private BirdScript bird; // Reference to the bird script

    void Start()
    {
        logic = GameObject.FindGameObjectWithTag("Logic").GetComponent<LogicScript>();
        bird = GameObject.FindGameObjectWithTag("Player").GetComponent<BirdScript>(); // Assuming your bird has the tag "Player"
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Ensure that the collision object is the bird and that the bird is still alive
        if (collision.gameObject.layer == 3 && bird.birdIsAlive)
        {
            logic.addScore(1);
        }
    }
}
