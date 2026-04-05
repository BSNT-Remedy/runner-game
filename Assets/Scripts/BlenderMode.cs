using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlenderMode : MonoBehaviour
{
    [SerializeField] GameObject inputSystem;
    [SerializeField] GameObject thePlayer;
    [SerializeField] GameObject character;
    [SerializeField] GameObject playerAnimation;
    [SerializeField] GameObject buttonPanel;
    public ModeManager modeManager;
    public bool hasEntered;
    public int lessonNumber = 1;

    public void OnTriggerEnter(Collider other)
    {
        if(hasEntered) return;

        if(other.gameObject.CompareTag("BlenderMode"))
        {
            DisableSegmentMovement();
            playerAnimation.GetComponent<Animator>().CrossFade("Breathing Idle", 0.2f);
            buttonPanel.SetActive(true);

            Button[] allButtons = buttonPanel.GetComponentsInChildren<Button>();
            
            foreach (Button b in allButtons)
            {
                b.interactable = false;

                if(lessonNumber == 1 && b.name == "ButtonGrab") {
                    b.interactable = true;
                }else if(lessonNumber == 2 && b.name == "ButtonRotate") {
                    b.interactable = true;
                }else if(lessonNumber == 3 && b.name == "ButtonScale"){
                    b.interactable = true;
                }else if(b.name == "Run"){
                    b.interactable = true;
                }
            }

            lessonNumber += 1;
            
            thePlayer.GetComponent<LaneSwipeController>().enabled = false;
            thePlayer.GetComponent<PlayerMovement>().enabled = false;
            character.GetComponent<SwipeJumpSlideController>().enabled = false;
            inputSystem.SetActive(true);
            hasEntered = true;
        }
    }

    public void OnTriggerExit(Collider other) {
        if(other.gameObject.CompareTag("BlenderExit")) {
            // playerAnimation.GetComponent<Animator>().Play("Running");
            buttonPanel.SetActive(false);
            thePlayer.GetComponent<LaneSwipeController>().enabled = true;
            thePlayer.GetComponent<PlayerMovement>().enabled = true;
            character.GetComponent<SwipeJumpSlideController>().enabled = true;
            inputSystem.SetActive(false);
            modeManager.ClearMode();
            hasEntered = false;
        }
    }

    public void DisableSegmentMovement() {
        var movers = FindObjectsOfType<SegmentMovement>(includeInactive: true);
        foreach (var m in movers)
        {
            // m.StopAllCoroutines();
            m.GetComponent<SegmentMovement>().enabled = false;
        }

    }

    public void ContinueRunning() {
        GameObject playerAnim = GameObject.FindWithTag("PlayerAnimation"); 
        if (playerAnim != null) {
            playerAnim.GetComponent<Animator>().Play("Running");
        }
        
        var movers = FindObjectsOfType<SegmentMovement>(includeInactive: true);
        foreach (var m in movers)
        {
            // m.StopAllCoroutines();
            m.GetComponent<SegmentMovement>().enabled = true;
        }
        // StartCoroutine(ExitTrigger());
        
        // playerAnimation.GetComponent<Animator>().Play("Running");
    }

    IEnumerator ExitTrigger() {
        yield return new WaitForSeconds(2);
        hasEntered = false;
    }
}