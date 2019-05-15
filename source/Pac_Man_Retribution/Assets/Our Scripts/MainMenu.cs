using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public GameObject loading;
    public GameObject menu;

    private void Start()
    { 
        Screen.fullScreen = !Screen.fullScreen;
        Screen.SetResolution(1980, 1020, true);
        Application.targetFrameRate = 60;
    }
    public void PlayGame(){
        //SceneManager.LoadScene("Loading", LoadSceneMode.Single);
        StartCoroutine(load());
    }

    public void GotoMainMenu()
    {
        SceneManager.LoadScene("Menu", LoadSceneMode.Single);
    }

    public void quit(){
        Application.Quit();
    }
    IEnumerator load()
    {
        menu.SetActive(false);
        loading.SetActive(true);

        AsyncOperation op = SceneManager.LoadSceneAsync("Game", LoadSceneMode.Single);
        //op.allowSceneActivation = false;

        while (!op.isDone)
        {
            yield return null;
        }
    }
}
