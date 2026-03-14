using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // required for scene management

public class SegmentGeneratorAssess : MonoBehaviour
{
    public GameObject[] segments;  // 10 segments
    public float segmentSpeed = 6;
    public string nextSceneName;   // name of the scene to load

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
                Debug.Log("All segments spawned! Loading next scene...");
                StartCoroutine(LoadNextSceneAfterDelay(1.5f));
                return;
            }

            int segmentNum = segmentOrder[currentIndex];
            currentIndex++;

            GameObject newSeg = Instantiate(segments[segmentNum], new Vector3(0, 0, 100), Quaternion.identity);
            newSeg.SetActive(true);

            Debug.Log("Spawned segment: " + newSeg.name);
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