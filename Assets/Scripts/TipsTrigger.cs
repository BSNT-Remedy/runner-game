using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TipsTrigger : MonoBehaviour
{
    [SerializeField] GameObject[] tipsPanel;
    [SerializeField] GameObject thePlayer;
    public int tipsPanelIndex = 0;

    public void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("BlenderTips")) {
            StartCoroutine(DisplayTips());
        }
    }

    IEnumerator DisplayTips()
    {
        GameObject newPanel = tipsPanel[tipsPanelIndex];
        newPanel.SetActive(true);
        tipsPanelIndex += 1;
        yield return new WaitForSeconds(5);
        newPanel.SetActive(false);
    }

   
}
