using UnityEngine;
using UnityEngine.InputSystem;

public class LaneSwipeController : MonoBehaviour
{
    [Header("Lanes")]
    [Tooltip("Number of lanes (use odd numbers for a center lane: 3, 5, etc.)")]
    public int laneCount = 3;

    [Tooltip("Horizontal distance between adjacent lanes, in world units.")]
    public float laneSpacing = 2.5f;

    [Header("Movement")]
    [Tooltip("How fast the player slides to the target lane (units/sec).")]
    public float slideSpeed = 12f;

    [Header("Swipe")]
    [Tooltip("Minimum swipe length in pixels to register.")]
    public float minimumSwipeDistance = 60f;

    // Internal
    private int currentLaneIndex;   // 0..laneCount-1
    private int targetLaneIndex;    // where we’re heading
    private float centerLaneIndex;  // for computing x positions
    private Vector2 swipeStart;

    void Awake()
    {
        // Initialize to center lane
        currentLaneIndex = targetLaneIndex = GetCenterLaneIndex();
        centerLaneIndex = GetCenterLaneIndex();
        SnapToLane(currentLaneIndex);
    }

    void Update()
    {
        ReadSwipe();
        MoveTowardsTargetLane();
    }

    // --- Swipe handling ---
    void ReadSwipe()
    {
        // Prefer touch on mobile
        if (Touchscreen.current != null)
        {
            var touch = Touchscreen.current.primaryTouch;

            if (touch.press.wasPressedThisFrame)
            {
                swipeStart = touch.position.ReadValue();
            }
            else if (touch.press.wasReleasedThisFrame)
            {
                Vector2 end = touch.position.ReadValue();
                TryHandleSwipe(end - swipeStart);
            }
        }
        // Mouse fallback for editor testing
        else if (Mouse.current != null)
        {
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                swipeStart = Mouse.current.position.ReadValue();
            }
            else if (Mouse.current.leftButton.wasReleasedThisFrame)
            {
                Vector2 end = Mouse.current.position.ReadValue();
                TryHandleSwipe(end - swipeStart);
            }
        }
    }

    void TryHandleSwipe(Vector2 delta)
    {
        if (delta.magnitude < minimumSwipeDistance)
            return;

        // Horizontal swipe?
        if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
        {
            if (delta.x > 0)
            {
                MoveRight();
                // Pan the camera to x = 8.17 on right swipe
                if (Camera.main != null)
                {
                    Camera.main.transform.position = new Vector3(8.17f, Camera.main.transform.position.y, Camera.main.transform.position.z);
                }
            }
            else
            {
                MoveLeft();
            }
        }
    }

    // --- Lane movement ---
    void MoveLeft()
    {
        targetLaneIndex = Mathf.Max(0, targetLaneIndex - 1);
        // Debug.Log($"Lane -> {targetLaneIndex}");
    }

    void MoveRight()
    {
        targetLaneIndex = Mathf.Min(laneCount - 1, targetLaneIndex + 1);
        // Debug.Log($"Lane -> {targetLaneIndex}");
    }

    void MoveTowardsTargetLane()
    {
        Vector3 pos = transform.position;
        float targetX = LaneIndexToWorldX(targetLaneIndex);
        float newX = Mathf.MoveTowards(pos.x, targetX, slideSpeed * Time.deltaTime);
        transform.position = new Vector3(newX, pos.y, pos.z);

        // Update current lane when we arrive
        if (Mathf.Approximately(newX, targetX))
            currentLaneIndex = targetLaneIndex;
    }

    void SnapToLane(int laneIndex)
    {
        Vector3 pos = transform.position;
        transform.position = new Vector3(LaneIndexToWorldX(laneIndex), pos.y, pos.z);
    }

    float LaneIndexToWorldX(int laneIndex)
    {
        // Example for 3 lanes (indices 0,1,2):
        // center = 1 → offsets: [-1, 0, +1] * spacing
        float offsetFromCenter = laneIndex - centerLaneIndex;
        return offsetFromCenter * laneSpacing;
    }

    int GetCenterLaneIndex()
    {
        // For odd lane counts, the exact center (e.g., 3→1, 5→2).
        // For even lane counts, this returns the left-of-center lane.
        return Mathf.Clamp(laneCount / 2, 0, Mathf.Max(0, laneCount - 1));
    }
}