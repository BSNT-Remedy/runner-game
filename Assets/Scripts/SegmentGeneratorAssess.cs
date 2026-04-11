using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SegmentGeneratorAssess : MonoBehaviour
{
    public GameObject[] segments;
    public float segmentSpeed = 6;
    public string nextSceneName;
    public LeaderboardManager leaderboardManager;
    public string playerTag = "Player";

    private List<int> segmentOrder; 
    private int currentIndex = 0;  

    private void Start()
    {
        // Initialize and shuffle the segment order
        segmentOrder = new List<int>();
        for (int i = 0; i < segments.Length; i++)
            segmentOrder.Add(i);

        ShuffleList(segmentOrder);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Trigger"))
        {
            if (currentIndex >= segmentOrder.Count)
            {
                // All segments already spawned; wait for player to reach final spawned segment.
                return;
            }

            int segmentNum = segmentOrder[currentIndex];
            currentIndex++;

            GameObject newSeg = Instantiate(segments[segmentNum], new Vector3(0, 0, 100), Quaternion.identity);
            newSeg.SetActive(true);

            Debug.Log("Spawned segment: " + newSeg.name);

            // If this was the last segment spawned, attach a watcher to detect when player finishes it
            if (currentIndex >= segmentOrder.Count)
            {
                var watcher = newSeg.AddComponent<FinalSegmentWatcher>();
                watcher.Setup(leaderboardManager, playerTag, 1f);
                Debug.Log("Attached FinalSegmentWatcher to last segment.");

                // If this segment contains a GateManager, mark it as the final assessment gate so choosing a gate shows the leaderboard
                var gateManager = newSeg.GetComponentInChildren<GateManager>();
                if (gateManager != null)
                {
                    gateManager.isFinalSegment = true;
                    gateManager.leaderboardManager = leaderboardManager;
                    Debug.Log("Marked GateManager in final segment to show leaderboard when gate chosen.");
                }
            }
        }
    }

    private IEnumerator LoadNextSceneAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(nextSceneName);
    }

    private void ShuffleList(List<int> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            int temp = list[i];
            list[i] = list[j];
            list[j] = temp;
        }
    }
}