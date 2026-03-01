using UnityEngine;

public class CollisionDetect : MonoBehaviour
{
    [SerializeField] GameObject thePlayer;
    [SerializeField] GameObject playerAnimation;
    void OnTriggerEnter(Collider other)
    {
        thePlayer.GetComponent<PlayerMovement>().enabled = false;
        playerAnimation.GetComponent<Animator>().Play("Stumble Backwards");
    }
}
