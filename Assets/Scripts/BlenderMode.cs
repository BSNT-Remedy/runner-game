using UnityEngine;

public class BlenderMode : MonoBehaviour
{
    [SerializeField] GameObject inputSystem;
    [SerializeField] GameObject thePlayer;
    [SerializeField] GameObject buttonPanel;
    public bool hasEntered;

    public void OnTriggerEnter(Collider other)
    {
        if(hasEntered) return;

        if(other.gameObject.CompareTag("BlenderMode"))
        {
            DisableSegmentMovement();
            buttonPanel.SetActive(true);
            thePlayer.GetComponent<LaneSwipeController>().enabled = false;
            inputSystem.SetActive(true);
            hasEntered = true;
        }
    }

    public void DisableSegmentMovement() {
        var movers = FindObjectsOfType<SegmentMovement>(includeInactive: true);
        foreach (var m in movers)
        {
            m.StopAllCoroutines();
            m.enabled = false;
        }

    }

    public void ContinueRunning() {
        var movers = FindObjectsOfType<SegmentMovement>(includeInactive: true);
        foreach (var m in movers)
        {
            m.StopAllCoroutines();
            m.enabled = true;
        }

        buttonPanel.SetActive(false);
        thePlayer.GetComponent<LaneSwipeController>().enabled = true;
        inputSystem.SetActive(false);
        hasEntered = false;
    }
}