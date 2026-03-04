using UnityEngine;

public class SegmentTrigger : MonoBehaviour
{
    public GameObject roadSegment;

    private void OnTriggerEnter(Collider other) {
        if(other.gameObject.CompareTag("Trigger")) {
            Instantiate(roadSegment, new Vector3(0, 0, 150), Quaternion.identity);
        }
    }
}
