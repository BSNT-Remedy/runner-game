using UnityEngine;

public class MouseDragOnXZ : MonoBehaviour
{
    [SerializeField] private Camera cam;
    // public float heightY = 0.5f;       // keep the cube at a fixed Y height
    public LayerMask mask = ~0;
    private Plane ground;
    private bool dragging;
    private Vector3 grabOffset;

    void Awake()
    {
        if (!cam) cam = Camera.main;
        ground = new Plane(Vector3.up, new Vector3(0, 0, 0));
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray r = cam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(r, out var hit, 1000f, mask))
            {
                if (hit.collider.transform.root == transform.root)
                {
                    dragging = true;

                    // compute grab offset on XZ
                    if (ground.Raycast(r, out float t))
                    {
                        var p = r.origin + r.direction * t;
                        grabOffset = transform.position - p;
                    }
                }
            }
        }

        if (dragging && Input.GetMouseButton(0))
        {
            Ray r = cam.ScreenPointToRay(Input.mousePosition);
            if (ground.Raycast(r, out float t))
            {
                var p = r.origin + r.direction * t;
                Vector3 target = p + grabOffset;
                // target.y = heightY;
                transform.position = target;
            }
        }

        if (Input.GetMouseButtonUp(0)) dragging = false;
    }
}