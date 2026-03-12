using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ObjectSelection : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera cam;

    [Header("Picking")]
    [SerializeField] private LayerMask interactableMask = ~0;
    [SerializeField] private float maxPickDistance = 200f;
    [SerializeField] private bool blockWhenPointerOverUI = true;

    [Header("Highlight")]
    [Tooltip("Material used to highlight the selected object. Applied to ALL renderers under it.")]
    [SerializeField] private Material highlightMaterial;

    [Header("Behavior")]
    [SerializeField, Tooltip("If true, tapping empty space will clear selection.")]
    private bool tapEmptyToDeselect = true;

    // Selected object
    [HideInInspector] public Transform selectedObject;

    // Cache of original sharedMaterials for clean restore
    private class RendererRecord
    {
        public Renderer renderer;
        public Material[] originals;
    }
    private List<RendererRecord> highlighted = new List<RendererRecord>();

    void Awake()
    {
        if (!cam) cam = Camera.main;
    }

    void Update()
    {
        if (Input.touchCount == 0) return;

        Touch t = Input.GetTouch(0);
        if (t.phase != TouchPhase.Began) return;

        if (blockWhenPointerOverUI && IsOverUI(t.fingerId)) return;

        Ray ray = cam.ScreenPointToRay(t.position);
        if (Physics.Raycast(ray, out RaycastHit hit, maxPickDistance, interactableMask, QueryTriggerInteraction.Collide))
        {
            Select(hit.transform);
        }
        else if (tapEmptyToDeselect)
        {
            Deselect();
        }
    }

    public void Select(Transform tr)
    {
        if (tr == selectedObject) return;

        // Prefer moving the renderer root if present
        var mr = tr.GetComponentInParent<MeshRenderer>();
        var smr = mr ? null : tr.GetComponentInParent<SkinnedMeshRenderer>();
        Transform target = mr ? mr.transform : (smr ? smr.transform : tr.root);

        // Clear previous highlight
        Deselect();

        selectedObject = target;

        // Apply highlight
        if (!highlightMaterial)
        {
            Debug.LogWarning("[ObjectSelection] No highlightMaterial assigned. Selection will not be visually highlighted.");
            return;
        }

        var renderers = selectedObject.GetComponentsInChildren<Renderer>(true);
        foreach (var r in renderers)
        {
            var rec = new RendererRecord
            {
                renderer = r,
                originals = r.sharedMaterials
            };

            // Build highlight array of equal length
            var arr = rec.originals != null && rec.originals.Length > 0
                ? new Material[rec.originals.Length]
                : new Material[1];

            for (int i = 0; i < arr.Length; i++) arr[i] = highlightMaterial;

            r.sharedMaterials = arr;
            highlighted.Add(rec);
        }
    }

    public void Deselect()
    {
        // Restore original shared materials
        if (highlighted != null)
        {
            foreach (var rec in highlighted)
            {
                if (rec != null && rec.renderer)
                {
                    rec.renderer.sharedMaterials = rec.originals;
                }
            }
            highlighted.Clear();
        }
        selectedObject = null;
    }

    private bool IsOverUI(int fingerId)
    {
        if (EventSystem.current == null) return false;
        return EventSystem.current.IsPointerOverGameObject(fingerId);
    }
}