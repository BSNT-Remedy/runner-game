using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    public static bool isPaused = false;
    public GameObject pauseMenuUI;
    [SerializeField] GameObject fadeOut;
    [SerializeField] GameObject Audio;
    [SerializeField] GameObject audioOff;
    [SerializeField] GameObject audioOn;
    public bool isMute = false;
    void Awake()
    {
        // Ensure the pause menu is hidden when the scene starts so it doesn't overlap any instruction UIs
        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(false);
        }
        Time.timeScale = 1f;
        isPaused = false;
    }

    public void PauseGame()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
    }

    public void ResumeGame()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }

    public void QuitGame()
    {
        Debug.Log("QUIT!");
        Application.Quit();
    }

    public void GoToMenu()
    {
        StartCoroutine(Menu());
        // SceneManager.LoadScene(0);
    }

    IEnumerator Menu()
    {
        fadeOut.SetActive(true);
        yield return new WaitForSecondsRealtime(0);

        Time.timeScale = 1f;
        SceneManager.LoadScene(0);

    }

    public void AudioToggle()
    {
        if(isMute == false)
        {
            Audio.SetActive(false);
            audioOn.SetActive(false);
            audioOff.SetActive(true);
            isMute = true;
        }else
        {
            Audio.SetActive(true);
            audioOff.SetActive(false);
            audioOn.SetActive(true);
            isMute = false;
        }
    }
}