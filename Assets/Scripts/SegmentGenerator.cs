using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SegmentGenerator : MonoBehaviour
{
    public GameObject[] segment;
    public float segmentSpeed = 6;
    [SerializeField] int zPos = 100;
    // [SerializeField] bool creatingSegment = false;
    [SerializeField] int segmentNum;
    // void Update()
    // {
    //     if(creatingSegment == false)
    //     {
    //         creatingSegment = true;
    //         StartCoroutine(SegmentGen());
    //     }
    // }

    // IEnumerator SegmentGen() {
    //     segmentNum = Random.Range(0, 3);
    //     GameObject newSeg = Instantiate(segment[segmentNum], new Vector3(0, 0, zPos), Quaternion.identity);
    //     newSeg.SetActive(true);
    //     // zPos += 50;
    //     yield return new WaitForSeconds(3);
    //     creatingSegment = false;
    // }

    

    private void OnTriggerEnter(Collider other) {
        if(other.gameObject.CompareTag("Trigger")) {
            segmentNum = Random.Range(0, 3);
            GameObject newSeg = Instantiate(segment[segmentNum], new Vector3(0, 0, zPos), Quaternion.identity);
            newSeg.SetActive(true);
        }
    }
}
