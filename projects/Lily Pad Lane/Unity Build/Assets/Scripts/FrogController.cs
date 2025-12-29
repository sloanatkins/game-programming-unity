using UnityEngine;
using System.Collections;


public class FrogController : MonoBehaviour
{
    public float jumpDistance = 1.0f;
    public float jumpDuration = 0.25f;
    public float[] allowedX = { -8f, -6f, -4f, -2f, 0f, 2f, 4f, 6f, 8f };

    private Animator animator;
    private bool isJumping = false;
    private Transform logParent = null;

    private Vector3 startPos;
    private Vector3 targetPos;
    private float jumpTimer;

    private float lastAllowedBackY;

    public Sprite deadForward;
    public Sprite deadBackward;
    public Sprite deadLeft;
    public Sprite deadRight;

    private SpriteRenderer sr;
    private string lastDirection;

    private bool isDead = false;

    public GameOverManager gameOverManager;

    public static FrogController instance;

    private bool cancelJumpSound = false;




    void Start()
    {
        instance = this;

        if (gameOverManager == null)
            {
                gameOverManager = FindObjectOfType<GameOverManager>();
            }

        animator = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();

        gameObject.tag = "Player";

        // Align frog on the first row at start
        float rowHeight = 2.0429f;
        float cameraBottom = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 0)).y;
        float firstRowY = cameraBottom + rowHeight / 2f;

        // Snap X to nearest allowed position (center or one of 9 lanes)
        float closestX = allowedX[0];
        foreach (float x in allowedX)
        {
            if (Mathf.Abs(x - transform.position.x) < Mathf.Abs(closestX - transform.position.x))
                closestX = x;
        }

        transform.position = new Vector3(closestX, firstRowY, 0f);

        lastAllowedBackY = transform.position.y;

        ScoreManager.instance.ResetScore();



    }


    void Update()
    {

        if (isDead) return;
        // Move with log
        if (!isJumping && logParent != null)
        {
            MoveObject moveScript = logParent.GetComponent<MoveObject>();
            if (moveScript != null)
            {
                transform.position += moveScript.direction * moveScript.speed * Time.deltaTime;
            }
        }
        // Smooth jump
        if (isJumping)
        {
            jumpTimer += Time.deltaTime;
            float t = Mathf.Clamp01(jumpTimer / jumpDuration);
            transform.position = Vector3.Lerp(startPos, targetPos, t);

            if (t >= 1f)
            {
                isJumping = false;
                StartCoroutine(DelayedPostJumpCheck());
            }


            return;
        }

        // Jump input
        if (Input.GetKeyDown(KeyCode.W)) StartJump(Vector3.up, "JumpForward");
        else if (Input.GetKeyDown(KeyCode.S)) StartJump(Vector3.down, "JumpBackward");
        else if (Input.GetKeyDown(KeyCode.A)) StartJump(Vector3.left, "JumpLeft");
        else if (Input.GetKeyDown(KeyCode.D)) StartJump(Vector3.right, "JumpRight");
    }

    void StartJump(Vector3 direction, string trigger)
    {
        bool wasOnRideable = logParent != null;
        logParent = null;
        lastDirection = trigger;

        float nextY = transform.position.y + direction.y * jumpDistance;
        float rowHeight = 2.0429f;

        int currentRow = Mathf.FloorToInt(transform.position.y / rowHeight);
        int nextRow = Mathf.FloorToInt(nextY / rowHeight);

        if (currentRow < -2)
        {
            if (direction == Vector3.down)
                return; // block back until reaching row 2

            // if jumping forward and haven't set it yet, set lastAllowedBackY
            if (direction == Vector3.up && lastAllowedBackY < transform.position.y)
                lastAllowedBackY = transform.position.y;
        }

        // Prevent jumping back more than 1 row
        if (direction == Vector3.down && nextY < lastAllowedBackY - jumpDistance && !wasOnRideable)
        {
            return;
        }

        isJumping = true;
        jumpTimer = 0f;
        startPos = transform.position;

        // Snap X to closest allowed position
        float newX = Mathf.Round(startPos.x + direction.x * jumpDistance);
        float closestX = allowedX[0];
        foreach (float x in allowedX)
        {
            if (Mathf.Abs(x - newX) < Mathf.Abs(closestX - newX))
                closestX = x;
        }

        targetPos = new Vector3(closestX, nextY, 0);

        // Update allowed back Y only if moving forward
        if (direction == Vector3.up)
            lastAllowedBackY = targetPos.y;

        // Pre-check for rideable objects at landing spot (helps especially with backward jumps)
        Collider2D[] landingHits = Physics2D.OverlapBoxAll(
            new Vector2(targetPos.x, targetPos.y - 0.1f),
            new Vector2(0.9f, 0.4f),
            0f
        );

        foreach (Collider2D hit in landingHits)
        {
            if (hit.CompareTag("Log") || hit.CompareTag("LilyPad"))
            {
                logParent = hit.transform; // Assign before PostJumpCheck
                break;
            }
        }


        animator.SetTrigger(trigger);
        Camera.main.GetComponent<CameraFollowFrog>().UpdateCameraTarget(targetPos.y, startPos.y);

//        // Play jump sound
        cancelJumpSound = false;

        StartCoroutine(PlayJumpUnlessDead());


    }


    IEnumerator PlayJumpUnlessDead()
    {
        yield return new WaitForSeconds(0.05f); // small delay to allow death detection
        if (!isDead && !cancelJumpSound)
        {
            SFXManager.instance.PlayJump();
        }
    }

    private void SnapToNearestLogSpot()
    {
        Collider2D hit = Physics2D.OverlapCircle(transform.position, 0.1f, LayerMask.GetMask("Log")); // Make sure logs are on a 'Log' layer
        if (hit != null)
        {
            LogJumpSpots spots = hit.GetComponent<LogJumpSpots>();
            if (spots != null)
            {
                Transform closest = spots.GetClosestSpot(transform.position);
                transform.position = new Vector3(closest.position.x, transform.position.y, 0f); // Snap X only
            }
        }
    }

    private void CheckIfStandingOnLog()
    {
        Collider2D hit = Physics2D.OverlapCircle(transform.position, 0.1f, LayerMask.GetMask("Log")); // Use 'Log' layer or tag
        if (hit != null && hit.CompareTag("Log"))
        {
            logParent = hit.transform;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if ((other.CompareTag("Log") || other.CompareTag("LilyPad")) && logParent == other.transform)
        {
            logParent = null;
        }

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Coin"))
        {
            Coin coin = other.GetComponent<Coin>();
            if (coin != null && coin.value > 0)
            {
                CoinManager.instance.CollectCoin(coin.value);
                Destroy(other.gameObject);
            }

        }


        if (other.CompareTag("Car"))
            {
                FrogHitByCar();
            }

        if (other.CompareTag("Log") || other.CompareTag("LilyPad"))
        {
            logParent = other.transform;
        }

    }

    public bool IsOnLog()
    {
        return logParent != null;
    }

    public GameObject splashPrefab;

    public void DieInWater()
    {
        cancelJumpSound = true;
        isDead = true;

        float rowHeight = 2.0429f;
        float correctedY = Mathf.Round(targetPos.y / rowHeight) * rowHeight;

        Vector3 splashPosition = new Vector3(targetPos.x, correctedY, -1f);
        Instantiate(splashPrefab, splashPosition, Quaternion.identity);

        SFXManager.instance.PlaySplash();
        StartCoroutine(FallAndRespawn());
    }

    IEnumerator FallAndRespawn()
    {
        float sinkTime = 1f;
        float timer = 0f;
        Vector3 start = transform.position;
        Vector3 end = start + Vector3.down * 2f;

        // Optional: hide frog sprite or disable movement
        GetComponent<SpriteRenderer>().enabled = false;

        while (timer < sinkTime)
        {
            transform.position = Vector3.Lerp(start, end, timer / sinkTime);
            timer += Time.deltaTime;
            yield return null;
        }

        ScoreManager.instance.SaveCurrentScoreToLeaderboard();


        gameOverManager.ShowGameOver();

        Debug.Log("Frog drowned 💧");
    }

    public IEnumerator PostJumpCheck()
    {
        yield return new WaitForFixedUpdate();

        // Use a shared box under the frog for log + water detection
        Vector2 boxSize = new Vector2(0.8f, 0.5f);
        Vector2 boxCenter = new Vector2(transform.position.x, transform.position.y - 0.1f);

        bool isOnLogOrLilyPad = false;
        bool isOnWater = false;

        Collider2D[] hits = Physics2D.OverlapBoxAll(boxCenter, boxSize, 0f);


        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("Log"))
            {
                isOnLogOrLilyPad = true;
                logParent = hit.transform;
            }
            else if (hit.CompareTag("WaterRow"))
            {
                isOnWater = true;
            }
        }


        if (isOnWater && !isOnLogOrLilyPad)
        {
            yield return new WaitForSeconds(0.1f); // short delay
            Collider2D[] retryHits = Physics2D.OverlapBoxAll(boxCenter, boxSize, 0f);
            foreach (Collider2D hit in retryHits)
            {
                if (hit.CompareTag("Log"))
                {
                    isOnLogOrLilyPad = true;
                    logParent = hit.transform;
                }
            }
        }

        foreach (Collider2D hit in hits)
        {
            Debug.Log($"Hit: {hit.name}, Tag: {hit.tag}, Layer: {LayerMask.LayerToName(hit.gameObject.layer)}");
        }

         if (isOnWater && !isOnLogOrLilyPad)
         {
             DieInWater();
         }

        ScoreManager.instance.CheckForScoreIncrease(transform.position.y);

    }

    public void FrogHitByCar()
    {
        if (isDead) return;
        cancelJumpSound = true;
        isDead = true;
        // Optionally disable movement or play an animation

        isJumping = false;
        StopAllCoroutines();
        animator.enabled = false; // Prevent animation from overriding
        sr.sortingOrder = -1;

        transform.position = new Vector3(transform.position.x, transform.position.y, 1);

        switch (lastDirection)
        {
            case "JumpForward":
                sr.sprite = deadForward;
                break;
            case "JumpBackward":
                sr.sprite = deadBackward;
                break;
            case "JumpLeft":
                sr.sprite = deadLeft;
                break;
            case "JumpRight":
                sr.sprite = deadRight;
                break;
            default:
                sr.sprite = deadForward;
                break;
        }

        ScoreManager.instance.SaveCurrentScoreToLeaderboard();

        SFXManager.instance.PlayCarHit();

        gameOverManager.ShowGameOver();

    }



    public void ResetFrog()
    {
        isDead = false;
        isJumping = false;
        animator.enabled = true;
        sr.enabled = true;

        float rowHeight = 2.0429f;
        float cameraBottom = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 0)).y;
        float firstRowY = cameraBottom + rowHeight / 2f;

        transform.position = new Vector3(0, firstRowY, 0); // this line now works

        ScoreManager.instance.ResetScore();


    }


    IEnumerator DelayedPostJumpCheck()
    {
        yield return new WaitForFixedUpdate();
        yield return StartCoroutine(PostJumpCheck());
    }


    IEnumerator InitialStandingCheck()
    {
        // Wait 3 frames for rows + logs/lilypads to be ready
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();

        // Copy the same logic from PostJumpCheck
        Vector2 boxSize = new Vector2(0.9f, 0.4f);
        Vector2 boxCenter = new Vector2(transform.position.x, transform.position.y - 0.1f);

        bool isOnLogOrLilyPad = false;
        bool isOnWater = false;

        Collider2D[] hits = Physics2D.OverlapBoxAll(boxCenter, boxSize, 0f);

        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("Log") || hit.CompareTag("LilyPad"))
            {
                isOnLogOrLilyPad = true;
                logParent = hit.transform;
            }
            else if (hit.CompareTag("WaterRow"))
            {
                isOnWater = true;
            }
        }

        if (isOnWater && !isOnLogOrLilyPad)
        {
            DieInWater();
        }
    }

    IEnumerator InitialPostJumpCheckDelay()
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();

        StartCoroutine(PostJumpCheck());
    }



}

