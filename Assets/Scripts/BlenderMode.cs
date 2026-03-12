using UnityEngine;

public class BlenderMode : MonoBehaviour
{
    public CollisionDetect collisionDetect;
    [SerializeField] GameObject touchManager;
    [SerializeField] GameObject thePlayer;
    [SerializeField] GameObject manipulatableObject;
    public bool hasEntered;

    public void OnTriggerEnter(Collider other)
    {
        if(hasEntered) return;

        if(other.gameObject.CompareTag("BlenderMode"))
        {
            collisionDetect.DisableSegmentMovement();
            thePlayer.GetComponent<LaneSwipeController>().enabled = false;
            // touchManager.GetComponent<TouchGestureManipulator3D>().enabled = true;
            touchManager.GetComponent<Rotate>().enabled = true;
            // manipulatableObject.GetComponent<LaneSwipeController>().enabled = true;
            hasEntered = true;
        }
    }


}