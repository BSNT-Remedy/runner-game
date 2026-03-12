using UnityEngine;

public class BlenderMode : MonoBehaviour
{
    public CollisionDetect collisionDetect;
    [SerializeField] GameObject inputSystem;
    [SerializeField] GameObject thePlayer;
    public bool hasEntered;

    public void OnTriggerEnter(Collider other)
    {
        if(hasEntered) return;

        if(other.gameObject.CompareTag("BlenderMode"))
        {
            collisionDetect.DisableSegmentMovement();
            thePlayer.GetComponent<LaneSwipeController>().enabled = false;
            inputSystem.SetActive(true);
            hasEntered = true;
        }
    }


}