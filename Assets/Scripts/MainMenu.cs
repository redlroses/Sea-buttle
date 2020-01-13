using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject menuPlane;
    [SerializeField] private GameObject settingsPlane;

    public void NewGame()
    {
        SceneManager.LoadScene("MainScene", LoadSceneMode.Single);
    }

    public void Settings()
    {
        menuPlane.SetActive(false);
        settingsPlane.SetActive(true);
    }

    public void Back()
    {
        menuPlane.SetActive(true);
        settingsPlane.SetActive(false);
    }

    public void Exit()
    {
        Application.Quit();
    }
}
