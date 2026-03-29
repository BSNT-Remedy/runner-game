using UnityEngine;

public class CameraTrigger : MonoBehaviour
{
    public Transform newCameraPosition;
    [SerializeField] GameObject mainCamera;
    [SerializeField] GameObject subCamera;
    public float transitionTime = 1f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            subCamera.SetActive(true);
            mainCamera.SetActive(false);
        }
    }
}

