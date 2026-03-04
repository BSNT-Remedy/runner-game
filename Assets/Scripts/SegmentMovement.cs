using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SegmentMovement : MonoBehaviour
{
    public float segmentSpeed = 6;

    void Update()
    {
        transform.Translate(Vector3.back * Time.deltaTime *  segmentSpeed, Space.World);        
    }

    private void OnTriggerEnter(Collider other) {
        if(other.gameObject.CompareTag("Destroy")) {
            Destroy(gameObject);
        }
    }
}
