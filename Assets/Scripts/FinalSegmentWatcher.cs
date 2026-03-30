using UnityEngine;

// Attach this to the final spawned segment. It creates a small child trigger at the forward end
// and fires the leaderboard when the player crosses it.
public class FinalSegmentWatcher : MonoBehaviour
{
    public string playerTag = "Player";
    public LeaderboardManager leaderboardManager;
    public float triggerDepth = 1f;

    private bool triggered = false;

    // Call this after instantiating the segment to configure the watcher
    public void Setup(LeaderboardManager lbm, string playerTagOverride = "Player", float depth = 1f)
    {
        leaderboardManager = lbm;
        if (!string.IsNullOrEmpty(playerTagOverride)) playerTag = playerTagOverride;
        triggerDepth = Mathf.Max(0.1f, depth);
        CreateFinishTrigger();
    }

    private void CreateFinishTrigger()
    {
        // Compute bounds from renderers or colliders
        Bounds bounds = new Bounds(transform.position, Vector3.zero);
        var rends = GetComponentsInChildren<Renderer>();
        if (rends.Length > 0)
        {
            bounds = rends[0].bounds;
            for (int i = 1; i < rends.Length; i++) bounds.Encapsulate(rends[i].bounds);
        }
        else
        {
            var cols = GetComponentsInChildren<Collider>();
            if (cols.Length > 0)
            {
                bounds = cols[0].bounds;
                for (int i = 1; i < cols.Length; i++) bounds.Encapsulate(cols[i].bounds);
            }
            else
            {
                // fallback size
                bounds = new Bounds(transform.position, new Vector3(10, 5, 30));
            }
        }

        // Create child trigger
        GameObject trigger = new GameObject("FinalFinishTrigger");
        trigger.transform.SetParent(transform, false);

        // Determine local center and size relative to the segment transform
        Vector3 worldCenter = bounds.center;
        Vector3 localCenter = transform.InverseTransformPoint(worldCenter);

        // Place trigger at the forward end of the bounds
        float forwardExtent = bounds.extents.z;
        Vector3 localPos = localCenter + Vector3.forward * (forwardExtent - triggerDepth * 0.5f);
        trigger.transform.localPosition = localPos;

        BoxCollider bc = trigger.AddComponent<BoxCollider>();
        bc.isTrigger = true;

        // Use segment width/height, small depth
        Vector3 sizeLocal = new Vector3(bounds.size.x / transform.lossyScale.x, bounds.size.y / transform.lossyScale.y, triggerDepth / transform.lossyScale.z);
        bc.size = sizeLocal;

        Rigidbody rb = trigger.AddComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;

        var finish = trigger.AddComponent<FinalTriggerHandler>();
        finish.Initialize(this);
    }

    internal void OnPlayerCross()
    {
        if (triggered) return;
        triggered = true;

        if (leaderboardManager != null)
        {
            leaderboardManager.ShowLeaderboard();
        }
        else
        {
            Time.timeScale = 0f;
        }
    }
}

// Helper component placed on the child trigger to forward events to FinalSegmentWatcher
public class FinalTriggerHandler : MonoBehaviour
{
    FinalSegmentWatcher parent;

    public void Initialize(FinalSegmentWatcher p)
    {
        parent = p;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (parent == null) return;
        if (string.IsNullOrEmpty(parent.playerTag) || other.CompareTag(parent.playerTag))
        {
            parent.OnPlayerCross();
            // optional: destroy trigger to avoid repeated calls
            Destroy(gameObject);
        }
    }
}
