using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ExerciseAnimation : MonoBehaviour
{
    [SerializeField] GameObject thePlayer;
    [SerializeField] GameObject playerAnimation;
    [SerializeField] GameObject character;
    [SerializeField] GameObject mainCamera;
    [SerializeField] GameObject subCamera;
    [SerializeField] GameObject cameraPosition;

    public int gymIndex = 0;

    public void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Exercise")) {
            DisableSegmentMovement();
            StartCoroutine(Exercise());
        }
    }

    
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Exercise"))
        {
            cameraPosition.GetComponent<LaneSwipeController>().enabled = true;
            thePlayer.GetComponent<LaneSwipeController>().enabled = true;
            thePlayer.GetComponent<PlayerMovement>().enabled = true;
            character.GetComponent<SwipeJumpSlideController>().enabled = true;
        }
    }


    IEnumerator Exercise()
    {
        cameraPosition.GetComponent<LaneSwipeController>().enabled = false;
        thePlayer.GetComponent<LaneSwipeController>().enabled = false;
        thePlayer.GetComponent<PlayerMovement>().enabled = false;
        character.GetComponent<SwipeJumpSlideController>().enabled = false;

        if(gymIndex == 0)
        {
            yield return PlayAnimation(playerAnimation.GetComponent<Animator>(), "Idle To Push Up");
            yield return PlayAnimation(playerAnimation.GetComponent<Animator>(), "Push Up");
            yield return new WaitForSeconds(6.0f);
            yield return PlayAnimation(playerAnimation.GetComponent<Animator>(), "Push Up To Idle");
        }

        if(gymIndex == 1)
        {
            yield return PlayAnimation(playerAnimation.GetComponent<Animator>(), "Idle To Situp");
            yield return PlayAnimation(playerAnimation.GetComponent<Animator>(), "Situps");
            yield return new WaitForSeconds(7f);
            yield return PlayAnimation(playerAnimation.GetComponent<Animator>(), "Situp To Idle");
        }

        if(gymIndex == 2)
        {
            yield return PlayAnimation(playerAnimation.GetComponent<Animator>(), "Air Squat");
            yield return new WaitForSeconds(11.0f);
            yield return new WaitForSeconds(2.0f);
        }

        
        gymIndex++;
        ContinueRunning();
        yield return PlayAnimation(playerAnimation.GetComponent<Animator>(), "Running"); 
    }

    public void DisableSegmentMovement() {
        var movers = FindObjectsOfType<SegmentMovement>(includeInactive: true);
        foreach (var m in movers)
        {
            m.StopAllCoroutines();
            m.enabled = false;
        }

    }

    public void ContinueRunning() {
        
        var movers = FindObjectsOfType<SegmentMovement>(includeInactive: true);
        foreach (var m in movers)
        {
            m.GetComponent<SegmentMovement>().enabled = true;
        }

        mainCamera.SetActive(true);
        subCamera.SetActive(false);
    }

    IEnumerator PlayAnimation(Animator anim, string stateName, float fadeDuration = 0.1f)
    {
        anim.CrossFade(stateName, fadeDuration);
        yield return new WaitForSeconds(anim.runtimeAnimatorController
                                            .animationClips
                                            .First(x => x.name == stateName).length);
    }
}