using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class LoadToStage : MonoBehaviour
{
    [SerializeField] GameObject fadeOut;
    [Header("Build Index to Load")] 
    [Tooltip("Build index of the scene to load: 1=Endless, 4=Learning, 5=Assessment")]
    [SerializeField] int stageBuildIndex = 1;

    void Start()
    {
        StartCoroutine(LoadLevel());
    }


    IEnumerator LoadLevel()
    {
        yield return new WaitForSeconds(3);
        if (fadeOut != null)
            fadeOut.SetActive(true);

        yield return new WaitForSeconds(2);

        // Load the configured build index (defaults to 1 = Endless)
        SceneManager.LoadScene(stageBuildIndex);
    }
}
