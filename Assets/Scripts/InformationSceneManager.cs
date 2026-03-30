using UnityEngine;
using UnityEngine.SceneManagement;

// Attach this to a controller object in your information/choice scene.
// Expose UI buttons to call the methods below.
public class InformationSceneManager : MonoBehaviour
{
    [Tooltip("Build index for Endless run (default 1)")]
    public int endlessIndex = 1;
    [Tooltip("Build index for Learning run (default 4)")]
    public int learningIndex = 4;
    [Tooltip("Build index for Assessment run (default 5)")]
    public int assessmentIndex = 5;

    [Header("Optional fade object on this information scene")]
    public GameObject fadeOut;
    [Header("Auto-advance settings")]
    [Tooltip("If true, the information scene will automatically forward to the pending stage after a delay.")]
    public bool autoProceed = true;
    [Tooltip("Delay in seconds before auto-forwarding from the information screen")]
    public float autoProceedDelay = 1.5f;

    // Static pending stage index set by the caller before loading this info scene.
    // If -1, no pending stage is set.
    public static int pendingStage = -1;

    void Start()
    {
        // If a pending stage was set by the previous scene and autoProceed is enabled,
        // automatically load that stage after a short delay.
        if (pendingStage >= 0 && autoProceed)
        {
            int toLoad = pendingStage;
            pendingStage = -1; // clear
            StartCoroutine(AutoLoad(toLoad));
        }
    }

    private System.Collections.IEnumerator AutoLoad(int index)
    {
        if (fadeOut != null)
            fadeOut.SetActive(true);
        yield return new WaitForSeconds(autoProceedDelay);
        SceneManager.LoadScene(index);
    }

    public void LoadEndless()
    {
        StartCoroutine(DoLoad(endlessIndex));
    }

    public void LoadLearning()
    {
        StartCoroutine(DoLoad(learningIndex));
    }

    public void LoadAssessment()
    {
        StartCoroutine(DoLoad(assessmentIndex));
    }

    public void LoadStage(int buildIndex)
    {
        StartCoroutine(DoLoad(buildIndex));
    }

    private System.Collections.IEnumerator DoLoad(int index)
    {
        if (fadeOut != null)
            fadeOut.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene(index);
    }
}
