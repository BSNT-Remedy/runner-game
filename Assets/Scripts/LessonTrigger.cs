using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LessonTrigger : MonoBehaviour
{
    [SerializeField] GameObject[] lessonPanel;
    [SerializeField] GameObject thePlayer;
    public int lessonPanelIndex = 0;

    public void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Lesson")) {
            StartCoroutine(DisplayLesson());
        }
    }

    IEnumerator DisplayLesson()
    {
        GameObject newPanel = lessonPanel[lessonPanelIndex];
        newPanel.SetActive(true);
        lessonPanelIndex += 1;
        yield return new WaitForSeconds(5);
        newPanel.SetActive(false);
    }

   
}
