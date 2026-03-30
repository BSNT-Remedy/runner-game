using UnityEngine;
using TMPro;

public class LeaderboardManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject leaderboardUI; // parent panel to show
    public TMP_Text titleText;
    public TMP_Text scoreText;

    [Header("Optional")]
    public PauseManager pauseManager; // optional - not required

    void Start()
    {
        if (leaderboardUI != null)
            leaderboardUI.SetActive(false);
    }

    public void ShowLeaderboard()
    {
        if (leaderboardUI == null)
        {
            Debug.LogWarning("LeaderboardManager: leaderboardUI not assigned.");
            Time.timeScale = 0f;
            return;
        }

        Debug.Log("LeaderboardManager: ShowLeaderboard called for: " + leaderboardUI.name);

        // Log parent chain active states for debugging
        Transform tt = leaderboardUI.transform;
        string chain = "";
        while (tt != null)
        {
            chain = tt.name + "(" + tt.gameObject.activeSelf + ") -> " + chain;
            tt = tt.parent;
        }
        Debug.Log("LeaderboardManager: parent chain: " + chain);

        // Stop gameplay
        Time.timeScale = 0f;

        // Update display values
        if (titleText != null)
            titleText.text = "Assessment Complete";

        if (scoreText != null)
        {
            // Show only the assessment gate score
            scoreText.text = "Score: " + GateManager.Score;
        }

        // Ensure the entire parent chain is active (in case the panel is nested under a disabled object)
        Transform t = leaderboardUI.transform;
        while (t != null)
        {
            if (!t.gameObject.activeSelf)
            {
                Debug.Log("LeaderboardManager: Activating " + t.name);
                t.gameObject.SetActive(true);
            }
            t = t.parent;
        }

        // Enable any Canvas components on parents (and set a high sorting order so it appears on top)
        var canvases = leaderboardUI.GetComponentsInParent<Canvas>(true);
        foreach (var c in canvases)
        {
            if (!c.enabled)
            {
                Debug.Log("LeaderboardManager: Enabling Canvas on " + c.gameObject.name);
                c.enabled = true;
            }
            try
            {
                c.sortingOrder = Mathf.Max(c.sortingOrder, 1000);
            }
            catch { }
        }

        // Also ensure any CanvasGroup on the panel (or parents) is visible and interactive
        var cgs = leaderboardUI.GetComponentsInParent<CanvasGroup>(true);
        foreach (var cg in cgs)
        {
            Debug.Log("LeaderboardManager: Fixing CanvasGroup on " + cg.gameObject.name + " (alpha=" + cg.alpha + ")");
            cg.alpha = 1f;
            cg.interactable = true;
            cg.blocksRaycasts = true;
        }

        leaderboardUI.SetActive(true);

        // Try to bring the panel into view: reset transform and move to top
        var rt = leaderboardUI.GetComponent<RectTransform>();
        if (rt != null)
        {
            Debug.Log("LeaderboardManager: Adjusting RectTransform for " + leaderboardUI.name);
            try
            {
                rt.localScale = Vector3.one;
                rt.anchoredPosition = Vector2.zero;
                rt.localPosition = Vector3.zero;
                rt.SetAsLastSibling();
            }
            catch { }
        }

        // If a PauseManager is provided, ensure its pause menu remains hidden
        if (pauseManager != null && pauseManager.pauseMenuUI != null)
            pauseManager.pauseMenuUI.SetActive(false);
    }

    public void HideLeaderboard()
    {
        if (leaderboardUI != null)
        {
            leaderboardUI.SetActive(false);
            Time.timeScale = 1f;
        }
    }
}
