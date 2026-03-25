using UnityEngine;

public class SwipeJumpSlideController : MonoBehaviour
{
    
    [Header("Swipe Settings")]
    public float minSwipeDistance = 50f;
    private Vector2 startTouch;
    private bool isSwiping = false;

    [Header("Jump Settings (No Physics)")]
    public float jumpHeight = 5f;
    public float jumpDuration = 0.5f;
    private bool isJumping = false;

    [Header("Slide Settings")]
    public float slideDuration = 0.7f;
    private bool isSliding = false;

    private CapsuleCollider col;
    private float originalHeight;
    private Vector3 originalPosition;

    void Start()
    {
        col = GetComponent<CapsuleCollider>();
        originalHeight = col.height;
        originalPosition = transform.localPosition;
    }

    void Update()
    {
        DetectSwipe();
    }

    // -------------------------------------------------
    // SWIPE DETECTION (EDITOR + MOBILE)
    // -------------------------------------------------
    void DetectSwipe()
    {
        // Editor / Mouse
        if (Input.GetMouseButtonDown(0))
        {
            isSwiping = true;
            startTouch = Input.mousePosition;
        }
        else if (Input.GetMouseButtonUp(0) && isSwiping)
        {
            Vector2 delta = (Vector2)Input.mousePosition - startTouch;
            HandleSwipe(delta);
            isSwiping = false;
        }

        // Mobile Touch
        if (Input.touchCount > 0)
        {
            Touch t = Input.GetTouch(0);

            if (t.phase == TouchPhase.Began)
            {
                isSwiping = true;
                startTouch = t.position;
            }
            else if (t.phase == TouchPhase.Ended && isSwiping)
            {
                Vector2 delta = t.position - startTouch;
                HandleSwipe(delta);
                isSwiping = false;
            }
        }
    }

    // -------------------------------------------------
    // SWIPE LOGIC (UP = Jump, DOWN = Slide)
    // -------------------------------------------------
    void HandleSwipe(Vector2 delta)
    {
        if (delta.magnitude < minSwipeDistance)
            return;

        float x = Mathf.Abs(delta.x);
        float y = Mathf.Abs(delta.y);

        if (y > x)
        {
            if (delta.y > 0) Jump();
            else Slide();
        }
    }

    // -------------------------------------------------
    // JUMP (TRANSFORM MOVEMENT, NO PHYSICS)
    // -------------------------------------------------
    void Jump()
    {
        if (isJumping || isSliding) return;

        StartCoroutine(JumpRoutine());
    }

    System.Collections.IEnumerator JumpRoutine()
    {
        isJumping = true;

        float timer = 0f;
        Vector3 startPos = new Vector3(transform.position.x, 2, transform.position.z);
        Vector3 peakPos = new Vector3(transform.position.x, jumpHeight, transform.position.z);

        // UPWARD PHASE
        while (timer < jumpDuration / 2f)
        {
            timer += Time.deltaTime;
            float t = timer / (jumpDuration / 2f);
            transform.localPosition = Vector3.Lerp(startPos, peakPos, t);
            yield return null;
        }

        // DOWNWARD PHASE
        timer = 0f;
        while (timer < jumpDuration / 2f)
        {
            timer += Time.deltaTime;
            float t = timer / (jumpDuration / 2f);
            transform.localPosition = Vector3.Lerp(peakPos, startPos, t);
            yield return null;
        }

        isJumping = false;
    }


    void Slide()
    {
        if (isSliding || isJumping) return;

        StartCoroutine(SlideRoutine());
    }

    System.Collections.IEnumerator SlideRoutine()
    {
        isSliding = true;

        // --- ROTATE TO -90 ---
        float duration = 0.2f; // how fast to tilt down
        float time = 0f;

        float startX = transform.localEulerAngles.x;
        float targetX = -90f;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;

            Vector3 rot = transform.localEulerAngles;
            rot.x = Mathf.LerpAngle(startX, targetX, t);
            transform.localEulerAngles = rot;

            yield return null;
        }

        // stay sliding
        yield return new WaitForSeconds(slideDuration);

        // --- ROTATE BACK TO 0 ---
        duration = 0.2f;
        time = 0f;

        startX = transform.localEulerAngles.x;
        targetX = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;

            Vector3 rot = transform.localEulerAngles;
            rot.x = Mathf.LerpAngle(startX, targetX, t);
            transform.localEulerAngles = rot;

            yield return null;
        }

        isSliding = false;
    }

}