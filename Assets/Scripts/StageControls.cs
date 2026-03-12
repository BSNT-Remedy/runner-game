using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using TMPro;

public class StageControls : MonoBehaviour
{
    [Header("Swipe")]
    [Tooltip("Minimum swipe length in pixels to register.")]
    public float minimumSwipeDistance = 60f;

    [Header("Stages")]
    [Tooltip("Camera x positions for each stage.")]
    public float[] stagePositions = { -0.17f, 8.17f, 16.17f };

    [Tooltip("Stage names corresponding to each position.")]
    public string[] stageNames = { "Endless", "Learning", "Assessment" };

    [Header("UI")]
    [Tooltip("Text component to display the current stage name.")]
    public TMP_Text stageNameText;

    private Vector2 swipeStart;
    private int currentStageIndex = 0;

    void Start()
    {
        // Initialize camera to first stage
        if (Camera.main != null)
        {
            Camera.main.transform.position = new Vector3(stagePositions[currentStageIndex], Camera.main.transform.position.y, Camera.main.transform.position.z);
        }
        // Display initial stage name
        UpdateStageName(currentStageIndex);
    }

    void Update()
    {
        ReadSwipe();
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
            if (delta.x > 0) // Swipe right: go to previous stage
            {
                if (currentStageIndex > 0)
                {
                    currentStageIndex--;
                    MoveCameraToStage(currentStageIndex);
                }
            }
            else if (delta.x < 0) // Swipe left: go to next stage
            {
                if (currentStageIndex < stagePositions.Length - 1)
                {
                    currentStageIndex++;
                    MoveCameraToStage(currentStageIndex);
                }
            }
        }
    }

    void MoveCameraToStage(int stageIndex)
    {
        if (Camera.main != null && stageIndex >= 0 && stageIndex < stagePositions.Length)
        {
            Camera.main.transform.position = new Vector3(stagePositions[stageIndex], Camera.main.transform.position.y, Camera.main.transform.position.z);
            UpdateStageName(stageIndex);
        }
    }

    void UpdateStageName(int stageIndex)
    {
        if (stageNameText != null && stageIndex >= 0 && stageIndex < stageNames.Length)
        {
            stageNameText.text = stageNames[stageIndex];
        }
    }

    public void PressPlay()
    {
        SceneManager.LoadScene(3);
    }
}
