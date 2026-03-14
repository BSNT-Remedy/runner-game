using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GateManager : MonoBehaviour
{
    [Header("Gate Setup")]
    public GameObject gate1;
    public GameObject gate2;
    public GameObject gate3;
    [Tooltip("Set the correct gate: 1, 2, or 3")]
    public int correctGate = 1;

    [Header("UI")]
    public TMP_Text scoreText;

    private bool hasPassed = false;
    private static int score = 0;

    private void Start()
    {
        UpdateScoreUI();
    }

    // Call this function when player chooses a gate
    public void ChooseGate(int gateNumber)
    {
        if (hasPassed)
        {
            Debug.Log("You have already chosen a gate!");
            return;
        }

        hasPassed = true; // only allow one choice per attempt

        if (gateNumber == correctGate)
        {
            Debug.Log("Correct Gate!");
            score++;
            UpdateScoreUI();
            // You can add effects, animations, or move player here
        }
        else
        {
            Debug.Log("Wrong Gate!");
            // Optionally reset or give feedback
        }

        // Optional: Disable gates after choice
        gate1.SetActive(false);
        gate2.SetActive(false);
        gate3.SetActive(false);

        // Optional: Restart or load next attempt after delay
        // StartCoroutine(RestartAfterDelay(2f));
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null)
            scoreText.text = score.ToString();
    }

    // Optional: restart the level after a delay
    private IEnumerator RestartAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        hasPassed = false;
        gate1.SetActive(true);
        gate2.SetActive(true);
        gate3.SetActive(true);
    }
}