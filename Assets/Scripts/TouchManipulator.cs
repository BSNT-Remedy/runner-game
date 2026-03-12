using UnityEngine;
using UnityEngine.EventSystems;

public class TouchManipulator : MonoBehaviour
{
    [Header("Camera & Picking")]
    [SerializeField] private Camera cam;
    [SerializeField] private LayerMask pickableMask = ~0;
    [SerializeField] private float maxPickDistance = 100f;
    [SerializeField] private bool blockWhenPointerOverUI = true;

    [Header("Drag Feel")]
    [SerializeField] private float dragLerp = 20f;            // 0 = instant, higher = smoother
    [SerializeField] private float planeDragSensitivity = 4f; // multiplier for movement on plane

    // State
    private Transform selected;
    private Plane dragPlane;        // XZ plane (Vector3.up normal) through initial hit point
    private Vector3 prevPlaneHit;   // last intersection on the drag plane
    private int primaryFingerId = -1;

    void Awake()
    {
        if (!cam) cam = Camera.main;
    }

    void Update()
    {
        if (!cam)
        {
            Debug.LogError("[TouchDragXZOnly] No Camera assigned.");
            return;
        }

        TouchUpdate();
    }

    private void TouchUpdate()
    {
        if (selected == null)
        {
            TryBeginTouchSelection();
            return;
        }

        if (Input.touchCount == 0)
        {
            ClearSelection();
            return;
        }

        HandleOneFingerDrag();
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
            // Select a sensible transform (renderer root if available)
            var tr = hit.collider.transform;
            var mr = tr.GetComponentInParent<MeshRenderer>();
            selected = mr ? mr.transform : tr.root;

            // Build XZ drag plane through the hit point
            dragPlane = new Plane(Vector3.up, hit.point);

            // Initialize baseline for plane-delta dragging
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

        // Plane-delta with sensitivity on XZ
        if (RayToPlane(cam.ScreenPointToRay(t.position), dragPlane, out Vector3 hitNow))
        {
            Vector3 delta = (hitNow - prevPlaneHit) * planeDragSensitivity;
            prevPlaneHit = hitNow;
            MoveSelected(selected.position + delta);
        }
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
        primaryFingerId = -1;
    }

    private bool IsOverUI(int fingerId)
    {
        if (!blockWhenPointerOverUI) return false;
        if (EventSystem.current == null) return false;
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