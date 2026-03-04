using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CollisionDetect : MonoBehaviour
{
    [SerializeField] GameObject segment1;
    [SerializeField] GameObject segment2;
    [SerializeField] GameObject segment3;
    [SerializeField] GameObject thePlayer;
    [SerializeField] GameObject playerAnimation;
    [SerializeField] AudioSource collisionFX;
    [SerializeField] GameObject mainCam;
    [SerializeField] GameObject fadeOut;
    public bool hasCollided = false;

    public void OnTriggerEnter(Collider other)
    {
        if(hasCollided) return;

        if(other.gameObject.CompareTag("Obstacle")) {
            DisableSegmentMovement();
            StartCoroutine(CollisionEnd());

            hasCollided = true;
        }
    }

    IEnumerator CollisionEnd()
    {
        // segment1.GetComponent<SegmentMovement>().enabled = false;
        // segment2.GetComponent<SegmentMovement>().enabled = false;
        // segment3.GetComponent<SegmentMovement>().enabled = false;
        collisionFX.Play();
        thePlayer.GetComponent<PlayerMovement>().enabled = false;
        playerAnimation.GetComponent<Animator>().Play("Stumble Backwards");
        mainCam.GetComponent<Animator>().Play("CollisionCam");
        yield return new WaitForSeconds(2);
        fadeOut.SetActive(true);
        yield return new WaitForSeconds(3);
        SceneManager.LoadScene(0);
    }

    public void DisableSegmentMovement() {
        var movers = FindObjectsOfType<SegmentMovement>(includeInactive: true);
        foreach (var m in movers)
        {
            m.StopAllCoroutines();
            m.enabled = false;
        }

    }
}
