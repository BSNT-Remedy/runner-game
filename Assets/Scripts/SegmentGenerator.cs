using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SegmentGenerator : MonoBehaviour
{
    public GameObject[] segment;
    public float segmentSpeed = 6;
    // [SerializeField] int zPos = 100;
    [SerializeField] int segmentNum;

    
    private void OnTriggerEnter(Collider other) {
        if(other.gameObject.CompareTag("Trigger")) {
             Debug.Log("SEGMENT SPAWNED FROM: " + gameObject.name);
             
             
            segmentNum = Random.Range(0, 3);
            GameObject newSeg = Instantiate(segment[segmentNum], new Vector3(0, 0, 100), Quaternion.identity);
            newSeg.SetActive(true);
        }
    }
}
