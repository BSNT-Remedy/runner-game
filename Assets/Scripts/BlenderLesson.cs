using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BlenderLesson : MonoBehaviour
{
    [SerializeField] GameObject lessonPanel;
    [SerializeField] GameObject[] lessonText;

    public int lessonTextIndex = 0;

    public void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Exercise")) {
            StartCoroutine(DisplayLesson());
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if(other.gameObject.CompareTag("Exercise")) {
            lessonPanel.SetActive(false);
        }
    }
    IEnumerator DisplayLesson()
    {
        lessonPanel.SetActive(true);
        foreach (GameObject text in lessonText)
        {
            text.SetActive(true);
            yield return new WaitForSeconds(4.0f);
            text.SetActive(false);
        }
        
        // lessonText[lessonTextIndex].SetActive(true);
        // lessonTextIndex+=1;
    }
}