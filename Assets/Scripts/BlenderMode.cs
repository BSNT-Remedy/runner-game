using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlenderMode : MonoBehaviour
{
    [SerializeField] GameObject inputSystem;
    [SerializeField] GameObject thePlayer;
    [SerializeField] GameObject buttonPanel;
    public bool hasEntered;
    public int lessonNumber = 1;

    public void OnTriggerEnter(Collider other)
    {
        if(hasEntered) return;

        if(other.gameObject.CompareTag("BlenderMode"))
        {
            DisableSegmentMovement();
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
            inputSystem.SetActive(true);
            hasEntered = true;
        }
    }

    public void OnTriggerExit(Collider other) {
        if(other.gameObject.CompareTag("BlenderExit")) {
            buttonPanel.SetActive(false);
            thePlayer.GetComponent<LaneSwipeController>().enabled = true;
            inputSystem.SetActive(false);
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
        var movers = FindObjectsOfType<SegmentMovement>(includeInactive: true);
        foreach (var m in movers)
        {
            // m.StopAllCoroutines();
            m.GetComponent<SegmentMovement>().enabled = true;
        }
        // StartCoroutine(ExitTrigger());
        
        
    }

    IEnumerator ExitTrigger() {
        yield return new WaitForSeconds(2);
        hasEntered = false;
    }
}