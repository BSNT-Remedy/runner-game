using UnityEngine;
using UnityEngine.EventSystems;

public class TouchGestureManipulator3D : MonoBehaviour
{
    // ------------------------------
    // Inspector Settings
    // ------------------------------

    [Header("Camera & Picking")]
    [SerializeField] private Camera cam;
    [SerializeField] private LayerMask pickableMask = ~0;
    [SerializeField] private float maxPickDistance = 100f;
    [SerializeField] private bool blockWhenPointerOverUI = true;

    public enum DragPlaneMode { GroundXZ, ScreenPerpendicular }
    [Header("Drag Mode")]
    [SerializeField] private DragPlaneMode planeMode = DragPlaneMode.GroundXZ;

    public enum YLockMode { None, KeepInitialY, FixedY }
    [Header("Y-Lock (easy to change)")]
    [SerializeField] private YLockMode yLock = YLockMode.KeepInitialY;
    [SerializeField] private float fixedY = 0f;   // used when yLock == FixedY

    [Header("Drag Feel")]
    [SerializeField] private float dragLerp = 20f;           // 0 = instant, 10-30 = smooth
    [SerializeField] private float planeDragSensitivity = 4f; // multiplier for plane-delta dragging
    [SerializeField] private float pixelSensitivity = 4f;     // multiplier for pixel → world mapping (screen-perpendicular)

    [Header("Rotate & Scale (Two-Finger)")]
    [SerializeField] private bool enableTwoFingerMove = true;
    [SerializeField] private bool enableTwoFingerRotate = true;
    [SerializeField] private bool enableTwoFingerScale = true;
    [SerializeField] private float rotateSpeed = 1.0f; // twist sensitivity
    [SerializeField] private float scaleSpeed = 1.0f;  // pinch sensitivity
    [SerializeField] private float minScale = 0.2f;
    [SerializeField] private float maxScale = 4.0f;

    [Header("Editor/PC Testing")]
    [SerializeField] private bool enableMouseInEditor = true;

    // ------------------------------
    // Internal State
    // ------------------------------

    private Transform selected;            // object being manipulated
    private Transform selectedRoot;        // for debug/info
    private Plane dragPlane;               // plane used for dragging
    private Vector3 pivot;                 // world point initially touched
    private Vector3 grabOffsetWorld;       // offset from object pivot to grab point
    private int primaryFingerId = -1;      // which finger "owns" the drag
    private float initialY;                // cached when selection begins (for KeepInitialY)

    // For plane-delta dragging
    private Vector3 prevPlaneHit;

    // For pixel → world mapping
    private float depthFromCamera;

    void Awake()
    {
        if (!cam) cam = Camera.main;
    }

    void Update()
    {
        if (!cam)
        {
            Debug.LogError("[TouchManipulator] No Camera assigned. Drag your Main Camera into the 'cam' field.");
            return;
        }

#if UNITY_EDITOR || UNITY_STANDALONE
        if (enableMouseInEditor)
        {
            MouseUpdate();
            return;
        }
#endif
        TouchUpdate();
    }

    // ------------------------------
    // Touch Path
    // ------------------------------
    private void TouchUpdate()
    {
        if (!selected)
        {
            TryBeginTouchSelection();
            return;
        }

        if (Input.touchCount == 0)
        {
            ClearSelection();
            return;
        }

        // Handle gestures
        if (Input.touchCount >= 2)
        {
            if (enableTwoFingerMove || enableTwoFingerRotate || enableTwoFingerScale)
                HandleTwoFingerGesture();
        }
        else
        {
            HandleOneFingerDrag();
        }
    }

    private void TryBeginTouchSelection()
    {
        if (Input.touchCount == 0) return;

        Touch t0 = Input.GetTouch(0);
        if (t0.phase != TouchPhase.Began) return;

        if (blockWhenPointerOverUI && IsOverUI(t0.fingerId)) return;

        Ray ray = cam.ScreenPointToRay(t0.position);
        if (Physics.Raycast(ray, out RaycastHit hit, maxPickDistance, pickableMask, QueryTriggerInteraction.Collide))
        {
            ResolveSelection(hit);

            // Build drag plane
            BuildDragPlane(hit.point);

            // Cache initial Y for YLock
            initialY = selected.position.y;

            // Cache depth for pixel-mapping (screen-perpendicular mode)
            CacheDepthFromCamera();

            // Init plane-delta baseline
            RayToPlane(cam.ScreenPointToRay(t0.position), dragPlane, out prevPlaneHit);

            primaryFingerId = t0.fingerId;
        }
    }

    private void HandleOneFingerDrag()
    {
        Touch t = GetTouchById(primaryFingerId);
        if (t.fingerId != primaryFingerId) { ClearSelection(); return; }

        if (t.phase == TouchPhase.Canceled || t.phase == TouchPhase.Ended)
        {
            ClearSelection();
            return;
        }

        if (t.phase != TouchPhase.Moved && t.phase != TouchPhase.Stationary) return;

        Vector3 target;

        if (planeMode == DragPlaneMode.GroundXZ)
        {
            // Plane-delta with sensitivity (XZ plane)
            if (RayToPlane(cam.ScreenPointToRay(t.position), dragPlane, out Vector3 hitNow))
            {
                Vector3 delta = (hitNow - prevPlaneHit) * planeDragSensitivity;
                prevPlaneHit = hitNow;
                target = selected.position + delta;
                target = ApplyYLock(target);
                MoveSelected(target);
            }
        }
        else
        {
            // Screen-perpendicular: pixel → world mapping with sensitivity
            Vector2 screenDelta = t.deltaPosition;
            Vector3 worldDelta = PixelDeltaToWorldDelta(screenDelta, depthFromCamera) * pixelSensitivity;
            target = selected.position + worldDelta;
            target = ApplyYLock(target);
            MoveSelected(target);
        }
    }

    private void HandleTwoFingerGesture()
    {
        Debug.Log("MultiTouchEnabled: " + Input.multiTouchEnabled);
        Touch t0 = Input.GetTouch(0);
        Touch t1 = Input.GetTouch(Input.touchCount > 1 ? 1 : 0);
        if (blockWhenPointerOverUI && (IsOverUI(t0.fingerId) || IsOverUI(t1.fingerId))) return;

        // MOVE with midpoint (plane-based)
        if (enableTwoFingerMove)
        {
            Vector2 midNow = 0.5f * (t0.position + t1.position);
            Vector2 midPrev = 0.5f * ((t0.position - t0.deltaPosition) + (t1.position - t1.deltaPosition));

            if (RayToPlane(cam.ScreenPointToRay(midNow), dragPlane, out Vector3 hitNow) &&
                RayToPlane(cam.ScreenPointToRay(midPrev), dragPlane, out Vector3 hitPrev))
            {
                Vector3 delta = (hitNow - hitPrev) * planeDragSensitivity;
                Vector3 target = selected.position + delta;
                target = ApplyYLock(target);
                MoveSelected(target);
            }
        }

        // SCALE (pinch)
        if (enableTwoFingerScale)
        {
            float prevDist = (t0.position - t0.deltaPosition - (t1.position - t1.deltaPosition)).magnitude;
            float nowDist = (t0.position - t1.position).magnitude;
            if (prevDist > 0.001f)
            {
                float rawScale = nowDist / prevDist;
                float scaled = Mathf.Pow(rawScale, scaleSpeed);
                Vector3 newScale = selected.localScale * scaled;
                float uniform = Mathf.Clamp(newScale.x, minScale, maxScale);
                selected.localScale = new Vector3(uniform, uniform, uniform);
            }
        }

        // ROTATE (twist around camera forward at pivot)
        if (enableTwoFingerRotate)
        {
            float prevAngle = Mathf.Atan2(
                (t1.position - t1.deltaPosition).y - (t0.position - t0.deltaPosition).y,
                (t1.position - t1.deltaPosition).x - (t0.position - t0.deltaPosition).x) * Mathf.Rad2Deg;

            float nowAngle = Mathf.Atan2(
                (t1.position).y - (t0.position).y,
                (t1.position).x - (t0.position).x) * Mathf.Rad2Deg;

            float deltaAngle = Mathf.DeltaAngle(prevAngle, nowAngle);
            selected.RotateAround(pivot, cam.transform.forward, deltaAngle * rotateSpeed);
        }
    }

    // ------------------------------
    // Mouse Path (Editor/Standalone)
    // ------------------------------
#if UNITY_EDITOR || UNITY_STANDALONE
    private void MouseUpdate()
    {
        if (!selected)
        {
            if (Input.GetMouseButtonDown(0))
                TryPickMouse();
            return;
        }

        if (Input.GetMouseButtonUp(0))
        {
            ClearSelection();
            return;
        }

        if (Input.GetMouseButton(0))
        {
            // mimic one-finger drag
            Vector3 target;

            if (planeMode == DragPlaneMode.GroundXZ)
            {
                if (RayToPlane(cam.ScreenPointToRay(Input.mousePosition), dragPlane, out Vector3 hitNow))
                {
                    Vector3 delta = (hitNow - prevPlaneHit) * planeDragSensitivity;
                    prevPlaneHit = hitNow;
                    target = selected.position + delta;
                    target = ApplyYLock(target);
                    MoveSelected(target);
                }
            }
            else
            {
                // Screen-perpendicular with pixel → world mapping
                Vector2 screenDelta = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y")) * 10f; // amplify mouse step
                Vector3 worldDelta = PixelDeltaToWorldDelta(screenDelta, depthFromCamera) * pixelSensitivity;
                target = selected.position + worldDelta;
                target = ApplyYLock(target);
                MoveSelected(target);
            }
        }
    }

    private void TryPickMouse()
    {
        // For Editor we won't block by UI to simplify testing
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, maxPickDistance, pickableMask))
        {
            ResolveSelection(hit);
            BuildDragPlane(hit.point);
            initialY = selected.position.y;
            CacheDepthFromCamera();

            // Init plane delta baseline
            RayToPlane(ray, dragPlane, out prevPlaneHit);
        }
    }
#endif

    // ------------------------------
    // Core Helpers
    // ------------------------------

    private void ResolveSelection(in RaycastHit hit)
    {
        // Prefer manipulation root that actually renders
        var tr = hit.collider.transform;
        var mr = tr.GetComponentInParent<MeshRenderer>();
        selected = mr ? mr.transform : tr.root;
        selectedRoot = tr.root;
        pivot = hit.point;
    }

    private void BuildDragPlane(Vector3 throughPoint)
    {
        if (planeMode == DragPlaneMode.GroundXZ)
        {
            dragPlane = new Plane(Vector3.up, throughPoint);
            // Grab offset for nicer "hold" feel
            grabOffsetWorld = selected.position - throughPoint;
        }
        else
        {
            dragPlane = new Plane(-cam.transform.forward, throughPoint);
            // For screen-plane, offset is still useful when snapping to target
            grabOffsetWorld = pivot - selected.position;
        }
    }

    private void CacheDepthFromCamera()
    {
        depthFromCamera = Vector3.Dot(selected.position - cam.transform.position, cam.transform.forward);
        if (depthFromCamera < 0.01f) depthFromCamera = 0.01f;
    }

    private Vector3 PixelDeltaToWorldDelta(Vector2 pixelDelta, float depth)
    {
        // Estimate world units per pixel at given depth using diagonal step
        Vector3 w0 = cam.ScreenToWorldPoint(new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, depth));
        Vector3 w1 = cam.ScreenToWorldPoint(new Vector3(Screen.width * 0.5f + 1f, Screen.height * 0.5f + 1f, depth));
        float worldPerPixel = (w1 - w0).magnitude / 1.41421356f; // sqrt(2)

        Vector3 dx = cam.transform.right * (pixelDelta.x * worldPerPixel);
        Vector3 dy = cam.transform.up    * (pixelDelta.y * worldPerPixel);
        Vector3 delta = dx + dy;

        if (yLock != YLockMode.None)
        {
            // Project movement onto XZ when Y-locked
            delta = Vector3.ProjectOnPlane(delta, Vector3.up);
        }
        return delta;
    }

    private Vector3 ApplyYLock(Vector3 target)
    {
        switch (yLock)
        {
            case YLockMode.None:
                return target;
            case YLockMode.KeepInitialY:
                target.y = initialY;
                return target;
            case YLockMode.FixedY:
                target.y = fixedY;
                return target;
        }
        return target;
    }

    private void MoveSelected(Vector3 target)
    {
        if (dragLerp <= 0f)
        {
            selected.position = target;
            return;
        }
        selected.position = Vector3.Lerp(
            selected.position,
            target,
            1f - Mathf.Exp(-dragLerp * Time.deltaTime)
        );
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

    private void ClearSelection()
    {
        selected = null;
        selectedRoot = null;
        primaryFingerId = -1;
    }

    private bool IsOverUI(int fingerId)
    {
        if (!blockWhenPointerOverUI) return false;
        if (EventSystem.current == null) return false;

#if UNITY_EDITOR || UNITY_STANDALONE
        if (fingerId == -1) return EventSystem.current.IsPointerOverGameObject();
#endif
        return EventSystem.current.IsPointerOverGameObject(fingerId);
    }

    private Touch GetTouchById(int id)
    {
        for (int i = 0; i < Input.touchCount; i++)
        {
            var t = Input.GetTouch(i);
            if (t.fingerId == id) return t;
        }
        return default;
    }
}