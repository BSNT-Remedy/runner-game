using UnityEngine;

public class DesertRunInstructions : MonoBehaviour
{
    [SerializeField] private GameObject instructionsCanvas;
    [SerializeField] private PauseManager pauseManager;
    [SerializeField] private bool showOnStart = true;
    private bool instructionsShown = false;

    void Start()
    {
        // Hide instructions at start
        if (instructionsCanvas != null)
        {
            instructionsCanvas.SetActive(false);
        }

        // Ensure the pause menu UI is hidden at start to avoid overlapping the instructions
        if (pauseManager != null && pauseManager.pauseMenuUI != null)
        {
            pauseManager.pauseMenuUI.SetActive(false);
        }

        // Optionally show instructions immediately when this scene/obj starts
        if (showOnStart)
        {
            ShowInstructionsAndPause();
        }
    }

    void Update()
    {
        // Hide instructions and resume when player taps/clicks
        if (instructionsShown && (Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)))
        {
            HideInstructionsAndResume();
        }
    }

    public void ShowInstructionsAndPause()
    {
        if (instructionsCanvas != null && pauseManager != null)
        {
            instructionsCanvas.SetActive(true);
            // Pause the game but keep the pause menu UI hidden so it doesn't overlap the instructions
            pauseManager.PauseGame();
            if (pauseManager.pauseMenuUI != null)
            {
                pauseManager.pauseMenuUI.SetActive(false);
            }
            instructionsShown = true;
        }
    }

    private void HideInstructionsAndResume()
    {
        if (instructionsCanvas != null && pauseManager != null)
        {
            instructionsCanvas.SetActive(false);
            pauseManager.ResumeGame();
            instructionsShown = false;
        }
    }
}
