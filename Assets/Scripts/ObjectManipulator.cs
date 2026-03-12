using UnityEngine;
using UnityEngine.EventSystems;

public class ObjectManipulator : MonoBehaviour
{
    [Header("References")]
    public Camera cam;
    public ObjectSelection selector;   // must expose: public Transform selectedObject;
    public ModeManager modeManager;    // ModeManager with Mode { None, Grab, Rotate, Scale }

    [Header("Grab (one finger, XZ plane)")]
    [SerializeField] private float grabLerp = 20f;             // 0 = instant, higher = smoother
    [SerializeField] private float planeDragSensitivity = 4f;  // multiplier for plane movement

    [Header("Rotate (one finger, horizontal)")]
    [SerializeField, Tooltip("Degrees per horizontal pixel.")]
    private float rotateSensitivityDegPerPixel = 0.4f;
    [SerializeField, Tooltip("Ignore tiny finger jitter (in pixels).")]
    private float rotateDeadzonePixels = 1.0f;
    [SerializeField, Tooltip("Rotate around world up (true) or object up (false).")]
    private bool rotateAroundWorldUp = true;

    [Header("Scale (one finger, horizontal)")]
    [SerializeField, Tooltip("Exponent coefficient: factor = exp(k * totalDx).")]
    private float scaleSensitivity = 0.0035f;
    [SerializeField] private float minScale = 0.2f;
    [SerializeField] private float maxScale = 4.0f;
    [SerializeField, Tooltip("Ignore tiny finger jitter (in pixels).")]
    private float scaleDeadzonePixels = 1.0f;

    // --- State: Grab ---
    private Plane dragPlane;           
    private Vector3 prevPlaneHit;      
    private int grabFingerId = -1;

    // --- State: Rotate ---
    private int rotateFingerId = -1;

    // --- State: Scale ---
    private int scaleFingerId = -1;
    private Vector3 startScale;
    private float accumDx;             // accumulated horizontal pixels for scale

    // Track last mode to reset state when switching
    private ModeManager.Mode lastMode = ModeManager.Mode.None;

    void Awake()
    {
        if (!cam) cam = Camera.main;
    }

    void Update()
    {
        if (!cam || selector == null || modeManager == null) return;

        Transform obj = selector.selectedObject;
        if (!obj) { ResetAllGestures(); return; }

        // Reset per-mode state if mode changed
        if (modeManager.currentMode != lastMode)
        {
            ResetAllGestures();
            lastMode = modeManager.currentMode;
        }

        if (Input.touchCount == 0)
        {
            ResetAllGestures();
            return;
        }

        switch (modeManager.currentMode)
        {
            case ModeManager.Mode.Grab:   HandleGrabXZ(obj);        break;
            case ModeManager.Mode.Rotate: HandleRotateOneFinger(obj); break;
            case ModeManager.Mode.Scale:  HandleScaleOneFinger(obj);  break;
            default: ResetAllGestures(); break;
        }
    }

    // ---------------------------
    // GRAB (G) - XZ plane drag
    // ---------------------------
    private void HandleGrabXZ(Transform obj)
    {
        // Begin
        if (grabFingerId == -1)
        {
            for (int i = 0; i < Input.touchCount; i++)
            {
                var t = Input.GetTouch(i);
                if (t.phase != TouchPhase.Began) continue;
                if (IsOverUI(t.fingerId)) continue;

                // XZ drag plane through current object position
                dragPlane = new Plane(Vector3.up, obj.position);

                if (RayToPlane(cam.ScreenPointToRay(t.position), dragPlane, out prevPlaneHit))
                {
                    grabFingerId = t.fingerId;
                    break;
                }
            }
            return;
        }

        // Continue with same finger
        if (!TryGetTouchById(grabFingerId, out Touch touch))
        {
            ResetGrab();
            return;
        }

        if (touch.phase == TouchPhase.Canceled || touch.phase == TouchPhase.Ended)
        {
            ResetGrab();
            return;
        }

        if (touch.phase != TouchPhase.Moved && touch.phase != TouchPhase.Stationary) return;

        if (RayToPlane(cam.ScreenPointToRay(touch.position), dragPlane, out Vector3 hitNow))
        {
            Vector3 delta = (hitNow - prevPlaneHit) * planeDragSensitivity;
            prevPlaneHit = hitNow;
            MoveLerped(obj, obj.position + delta, grabLerp);
        }
    }

    private void ResetGrab() => grabFingerId = -1;

    // ---------------------------
    // ROTATE (R) - one finger, horizontal swipe -> yaw
    // ---------------------------
    private void HandleRotateOneFinger(Transform obj)
    {
        // Begin
        if (rotateFingerId == -1)
        {
            for (int i = 0; i < Input.touchCount; i++)
            {
                var t = Input.GetTouch(i);
                if (t.phase != TouchPhase.Began) continue;
                if (IsOverUI(t.fingerId)) continue;

                rotateFingerId = t.fingerId;
                break;
            }
            return;
        }

        // Continue
        if (!TryGetTouchById(rotateFingerId, out Touch touch))
        {
            ResetRotate();
            return;
        }

        if (touch.phase == TouchPhase.Canceled || touch.phase == TouchPhase.Ended)
        {
            ResetRotate();
            return;
        }

        if (touch.phase != TouchPhase.Moved && touch.phase != TouchPhase.Stationary) return;

        float dx = touch.deltaPosition.x;
        if (Mathf.Abs(dx) < rotateDeadzonePixels) return;

        float angleDelta = -dx * rotateSensitivityDegPerPixel;
        Vector3 up = rotateAroundWorldUp ? Vector3.up : obj.up;
        obj.Rotate(up, angleDelta, Space.World);
    }

    private void ResetRotate() => rotateFingerId = -1;

    // ---------------------------
    // SCALE (S) - one finger, horizontal swipe -> uniform scale
    // ---------------------------
    private void HandleScaleOneFinger(Transform obj)
    {
        // Begin
        if (scaleFingerId == -1)
        {
            for (int i = 0; i < Input.touchCount; i++)
            {
                var t = Input.GetTouch(i);
                if (t.phase != TouchPhase.Began) continue;
                if (IsOverUI(t.fingerId)) continue;

                scaleFingerId = t.fingerId;
                startScale = obj.localScale;
                accumDx = 0f;
                break;
            }
            return;
        }

        // Continue
        if (!TryGetTouchById(scaleFingerId, out Touch touch))
        {
            ResetScale();
            return;
        }

        if (touch.phase == TouchPhase.Canceled || touch.phase == TouchPhase.Ended)
        {
            ResetScale();
            return;
        }

        if (touch.phase != TouchPhase.Moved && touch.phase != TouchPhase.Stationary) return;

        float dx = touch.deltaPosition.x;
        if (Mathf.Abs(dx) < scaleDeadzonePixels) return;

        accumDx += dx;

        // Smooth exponential mapping: factor = e^(k * Σdx)
        float factor = Mathf.Exp(scaleSensitivity * accumDx);

        // Uniform scale based on initial scale when gesture started
        float baseUniform = startScale.x; // assumes starting uniform; yields uniform result
        float newUniform = Mathf.Clamp(baseUniform * factor, minScale, maxScale);
        obj.localScale = new Vector3(newUniform, newUniform, newUniform);
    }

    private void ResetScale()
    {
        scaleFingerId = -1;
        accumDx = 0f;
    }

    // ---------------------------
    // Helpers
    // ---------------------------
    private void ResetAllGestures()
    {
        ResetGrab();
        ResetRotate();
        ResetScale();
    }

    private void MoveLerped(Transform obj, Vector3 target, float lerp)
    {
        if (lerp <= 0f) { obj.position = target; return; }
        obj.position = Vector3.Lerp(obj.position, target, 1f - Mathf.Exp(-lerp * Time.deltaTime));
    }

    private bool TryGetTouchById(int id, out Touch touch)
    {
        for (int i = 0; i < Input.touchCount; i++)
        {
            var t = Input.GetTouch(i);
            if (t.fingerId == id) { touch = t; return true; }
        }
        touch = default;
        return false;
    }

    private bool RayToPlane(Ray ray, Plane plane, out Vector3 hit)
    {
        if (plane.Raycast(ray, out float enter))
        {
            hit = ray.origin + ray.direction * enter;
            return true;
        }
        hit = Vector3.zero;
        return false;
    }

    private bool IsOverUI(int fingerId)
    {
        if (EventSystem.current == null) return false;
        return EventSystem.current.IsPointerOverGameObject(fingerId);
    }
}