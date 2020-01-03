using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{

    GameObject options;
    GameObject main;
    bool initializedMain = false;

    private void Start()
    {
        main = GameObject.Find("Main");
        options = GameObject.Find("Options");

        main.SetActive(false);
        options.SetActive(false);
    }


    public void Pause()
    {
        if (!initializedMain)
        {
            main.SetActive(true);
            initializedMain = true;
        }
    }

    public void Options()
    {
        main.SetActive(false);
        options.SetActive(true);

        
    }

    public void UnPause()
    {
        main.SetActive(false);
        options.SetActive(false);

        initializedMain = false;
    }

    public void Back()
    {
        main.SetActive(true);
        options.SetActive(false);
    }


    public void Disconnect()
    {
        BoltLauncher.Shutdown();
        SceneManager.LoadScene(0);
    }
}
