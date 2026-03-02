using UnityEngine;

public class CollectibleRotate : MonoBehaviour
{
    [SerializeField] float rotateSpeed = 1;

    void Update()
    {
        transform.Rotate(0, rotateSpeed, 0, Space.World);
    }
}
